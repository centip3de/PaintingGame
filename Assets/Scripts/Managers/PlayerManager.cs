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

	private CollisionManager collisionManager;
    private GameObject activeNoodle;
    private GameObject activeStair;
    private GameObject activeUmbrella;

    void Start ()
    {
		collisionManager = CollisionManager.builder (this)
			.add (new ClimbableObserver())
			.build ();

		KeyManager keyManager = GameObject.FindWithTag ("KeyManager").GetComponent<KeyManager> ();
		keyManager.onKeyDown += onKeyDown;
		keyManager.onKeyPress += onKeyPress;

        isClimbing = false;
        gravity = Physics2D.gravity;
        activeNoodle = null;
        activeStair = null;
        currentDirection = Vector2.zero;
        selectedAction = Actions.NOODLE;
        pauseMenuCanvas.enabled = false;
    }

    void nextLevel()
    {
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().nextLevel();
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
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y - 0.03f);
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
        switch(selectedAction)
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
		print (ev);

		if (ev.keyCode == KeyCode.F && selectedAction == Actions.NOODLE) {
			handleAction ();
		}
	}

	void onKeyPress(object sender, KeyPressEvent ev) {
		print (ev);

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

        if (!paused)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
				selectedToolImage.texture = Resources.Load ("NoodlesTool") as Texture;
                this.selectedAction = Actions.NOODLE;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
				selectedToolImage.texture = Resources.Load ("StairsTool") as Texture;
                this.selectedAction = Actions.STAIR;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
				selectedToolImage.texture = Resources.Load ("UmbrellaTool") as Texture;
                this.selectedAction = Actions.UMBRELLA;
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
        if(coll.gameObject.tag == "Death")
        {
            die();
        }

        if(coll.gameObject.tag == "NextLevel")
        {
            nextLevel();
        }

        if(coll.gameObject.tag == "Item")
        {
            print("Collected an item.");
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
		collisionManager.enterNotify (tagName);
    }

    void OnTriggerExit2D(Collider2D coll)
    {
		string tagName = coll.gameObject.tag;
		collisionManager.exitNotify (tagName);
    }
}
