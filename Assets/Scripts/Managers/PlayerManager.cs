using System;
using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;

public enum Actions
{
    NOODLE,
    STAIR,
    UMBRELLA
}

public class PlayerManager : MonoBehaviour
{
	private const float SPEED = 7f;
    private Vector3 velocity;
    private bool itemPickedUp;

    public GameObject umbrella;
    public GameObject noodle;
    public GameObject stairs;
    public Actions selectedAction;

    public Transform playerTransform;
    public Rigidbody2D playerRigidbody;
    public Vector2 gravity;
    public bool isClimbing;

    public bool paused;
    public Vector2 currentDirection;

	public RawImage selectedToolImage;
    public Canvas pauseMenuCanvas;

    public Canvas notificationCanvas;
    public Text notificationText;

	private CollisionManager collisionManager;
    private LevelManager levelManager;
    private GameObject activeNoodle;
    private GameObject activeStair;
    private GameObject activeUmbrella;

    void Start ()
    {
		collisionManager = CollisionManager.builder (this)
			.add (new ClimbableObserver())
			.add (new PaintingObserver())
			.build ();

		KeyManager keyManager = GameObject.FindWithTag ("KeyManager").GetComponent<KeyManager> ();
		keyManager.onKeyDown += onKeyDown;
		keyManager.onKeyPress += onKeyPress;

        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();

        isClimbing = false;
        gravity = Physics2D.gravity;
        activeNoodle = null;
        activeStair = null;
        currentDirection = Vector2.zero;
        selectedAction = levelManager.getAllowedAction(levelManager.getCurrentLevel());
        pauseMenuCanvas.enabled = false;
        notificationCanvas.enabled = false;
		selectedToolImage.texture.filterMode = FilterMode.Point;

        var playedStages = levelManager.getPlayedStages();

        if (playedStages.Count == 1 && playedStages[0] == "World Map")
        {
            notificationCanvas.enabled = true;
            notificationText.text = "\"Hm, I wonder what those paintings\nare...\"";
        }
        if(playedStages.Count == 2 && levelManager.deathCounter == 0)
        {
            notificationCanvas.enabled = true;
            notificationText.text = "\"Whoa! I've been teleported inside\nthe painting!\"";
        }
    }

    void nextLevel()
    {
        levelManager.nextLevel();
    }

	void nextLevel(string name) {
		levelManager.nextLevel(name);
	}

    void die()
    {
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().loadCurrentLevel();
    }

    void handleMovements()
	{
		int verticalSign = Math.Sign (Input.GetAxisRaw ("Vertical"));
		int horizontalSign = Math.Sign (Input.GetAxisRaw ("Horizontal"));

        if (horizontalSign != 0)
        {
            currentDirection.x = horizontalSign;
        }

		Vector3 displacement = new Vector3 (horizontalSign, verticalSign, 0);
		float scale = SPEED * Time.smoothDeltaTime;

		playerTransform.Translate (displacement * scale);

		SpriteRenderer renderer = GameObject.FindWithTag ("Player").GetComponent<SpriteRenderer> ();
		if (horizontalSign != 0)
		{
			renderer.flipX = horizontalSign < 0;
		}
	}

    void launchNoodle()
    {
        Vector2 rayCastStart = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 1);
        RaycastHit2D hit = Physics2D.Raycast(rayCastStart, Vector2.up);
        if (hit.collider != null && hit.collider.gameObject != this.gameObject && hit.collider.tag == "Ground")
        {
            if(activeNoodle != null)
            {
                Destroy(activeNoodle);
            }

            GameObject newObj = Instantiate(noodle, new Vector3(hit.point.x, hit.point.y, -1), Quaternion.identity);
            Vector3 scale = new Vector3(newObj.transform.localScale.x, 0.01f);
            newObj.transform.localScale = scale;
            activeNoodle = newObj;
        }
        else if (hit.collider != null && hit.collider.tag == "Climbable")
        {
            GameObject obj = hit.collider.gameObject;
            Vector3 scale = obj.transform.localScale;
            scale.y += 0.01f;
            obj.transform.localScale = scale;

            // Scaling is uniform, so we need to shift down each unit increased
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y - 0.03f, -1);
        }
    }

    void spawnStair()
    {
        if(activeStair != null)
        {
            Destroy(activeStair);
        }

        // Spawn it near the player, taking orientation into account.

        Vector3 pos = new Vector3(transform.position.x + (3 * currentDirection.x), transform.position.y + 1, -1);

        // We have to rotate around the Z axis, because fuck logic.
        Vector3 lookDirection = new Vector3(0, 0, currentDirection.x);
        activeStair = Instantiate(stairs, pos, transform.rotation);
    }

    void spawnUmbrella()
    {
        // One "click" opens the umbrella, another "closes" it.
        // FIXME: Because we're doing GetKey, rather than GetKeyDown (or whatever), this is being called multiple times.
        // That's fine for all the others, but has weird behavior for this one. 
        if (activeUmbrella != null)
        {
            Destroy(activeUmbrella);
            playerRigidbody.gravityScale = 1.0f;
        }
        else
        {
            Vector3 pos = new Vector3(transform.position.x + 1, transform.position.y + 1, -1);
            // Instantiate it as a child to the player so it stays attached.
            activeUmbrella = Instantiate(umbrella, pos, Quaternion.identity, playerTransform);
            playerRigidbody.gravityScale = 0.5f;
        }
    }

    void handleAction()
    {
        playerRigidbody.gravityScale = 1;
        if (itemPickedUp)
        {
            switch (selectedAction)
            {
                case Actions.NOODLE:
                    launchNoodle();
                    break;
                case Actions.STAIR:
                    spawnStair();
                    break;
                case Actions.UMBRELLA:
                    spawnUmbrella();
                    break;
                default:
                    break;
            }
        }
    }

    void OnPauseGame()
    {
        paused = true;
        Rigidbody2D rigidBody = gameObject.GetComponent<Rigidbody2D>();
        this.velocity = rigidBody.velocity;
        rigidBody.velocity = Vector3.zero;
        rigidBody.gravityScale = 0;

    }

    void OnResumeGame()
    {
        paused = false;
        Rigidbody2D rigidBody = gameObject.GetComponent<Rigidbody2D>();
        rigidBody.velocity = this.velocity;
        rigidBody.gravityScale = 1;
    }

    void pauseGame()
    {
        if (!paused)
        {
            GameObject[] objects = (GameObject[])FindObjectsOfType(typeof(GameObject));
            foreach (GameObject go in objects)
            {
                go.SendMessage("OnPauseGame", SendMessageOptions.DontRequireReceiver);
            }
        } else {
            GameObject[] objects = (GameObject[])FindObjectsOfType(typeof(GameObject));
            foreach (GameObject go in objects)
            {
                go.SendMessage("OnResumeGame", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

	void onKeyDown(object sender, KeyDownEvent ev) {
		if (ev.keyCode == KeyCode.F && selectedAction == Actions.NOODLE) {
			handleAction ();
		}
	}

	void onKeyPress(object sender, KeyPressEvent ev) {
		if (ev.keyCode == KeyCode.F && selectedAction != Actions.NOODLE) {
			handleAction ();
		}
	}

    void handleKeys()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenuCanvas.enabled = !pauseMenuCanvas.enabled;
            pauseGame();
        }

        if(Input.GetKeyDown(KeyCode.E) && notificationCanvas.enabled)
        {
            notificationCanvas.enabled = false;
        }

        if (!paused)
        {
            if (this.selectedAction == Actions.NOODLE)
            {
				selectedToolImage.texture = Resources.Load ("NoodlesTool") as Texture;
				selectedToolImage.texture.filterMode = FilterMode.Point;
            }

            if (this.selectedAction == Actions.STAIR)
            {
				selectedToolImage.texture = Resources.Load ("StairsTool") as Texture;
				selectedToolImage.texture.filterMode = FilterMode.Point;
            }

            if (this.selectedAction == Actions.UMBRELLA)
            {
				selectedToolImage.texture = Resources.Load ("UmbrellaTool") as Texture;
				selectedToolImage.texture.filterMode = FilterMode.Point;
            }
        }
    }

	void FixedUpdate ()
    {
        handleKeys();
        if (!paused)
        {
            handleMovements();
        }
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "NextLevel")
        {
            nextLevel();
        }
        if(coll.gameObject.tag == "Death")
        {
            die();
        }

        if (coll.gameObject.tag == "Item")
        {
            itemPickedUp = true;
            notificationCanvas.enabled = true;
            string currentLevel = levelManager.getCurrentLevel();
            if (currentLevel == "Warhol")
            {
                notificationText.text = "You've unlocked the noodle!\n\nHold F to create\nand grow a noodle that\nyou can climb on!";
            }
            else if (currentLevel == "Monet")
            {
                notificationText.text = "You've unlocked the umbrella!\n\nPress F to open/close\nthe umbrella and reduce gravity's\n pull on you!";
            }
            else if(currentLevel == "Escher")
            {
                notificationText.text = "You've unlocked the stairs!\n\nPress F to spawn stairs that you\ncan climb on!";
            }
            else
            {
                notificationText.text = "You broke the game, congrats!";
            }
            
            Destroy(coll.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Teleport")
        {
            coll.gameObject.GetComponent<TeleportManager>().teleportToExit(this.gameObject);
        }

        string tagName = coll.gameObject.tag;
		collisionManager.enterNotify (tagName, coll);
    }

    void OnTriggerExit2D(Collider2D coll)
    {
		string tagName = coll.gameObject.tag;
		collisionManager.exitNotify (tagName, coll);
    }
}
