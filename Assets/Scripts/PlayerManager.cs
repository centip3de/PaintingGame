using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public enum Actions
{
    NOODLE,
    STAIR,
    FOOD // ... ?
}

public class PlayerManager : MonoBehaviour
{
	private const float SPEED = 7f;

    public Transform playerTransform;
    public bool isGrounded;
    public bool dead;
    public Vector2 gravity;
    public GameObject noodle;
    public GameObject stairs;
    public Vector2 currentDirection;
    public Actions selectedAction;

    public bool isClimbing;
    private bool actionEnabled;
	private CollisionManager collisionManager;
    private GameObject activeNoodle;
    private GameObject activeStair;


    void Start ()
    {
		collisionManager = CollisionManager.builder (this)
			.add (new ClimbableObserver())
			.build ();

        isClimbing = false;
        gravity = Physics2D.gravity;
        activeNoodle = null;
        activeStair = null;
        currentDirection = Vector2.zero;
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

        if (horizontalSign != 0.0)
        {
            currentDirection = new Vector2(horizontalSign, 0);
        }

		Vector3 displacement = new Vector3 (horizontalSign, verticalSign, 0);
		float scale = SPEED * Time.smoothDeltaTime;

		playerTransform.Translate (displacement * scale);
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

            GameObject newObj = Instantiate(noodle, hit.point, Quaternion.identity);
            Vector3 scale = new Vector3(newObj.transform.localScale.x, 0.01f);
            newObj.transform.localScale = scale;
            activeNoodle = newObj;
        }
        else if (hit.collider != null && hit.collider.tag == "Climbable")
        {
            print("Hit another climbable, increasing their height.");
            GameObject obj = hit.collider.gameObject;
            Vector3 scale = obj.transform.localScale;
            scale.y += 0.01f;
            obj.transform.localScale = scale;

            // Scaling is uniform, so we need to shift down each unit increased
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y - 0.03f);
        }
        else
        {
            print("I hit myself/or nothing! ... I think?");
        }
    }

    void spawnStair()
    {
        if(activeStair != null)
        {
            Destroy(activeStair);
        }
        
        activeStair = Instantiate(stairs, new Vector3(transform.position.x + 3, transform.position.y + 2), Quaternion.identity);
    }

    void doFood() { }

    void handleAction()
    {
        switch(selectedAction)
        {
            case Actions.NOODLE:
                launchNoodle();
                break;
            case Actions.STAIR:
                spawnStair();
                break;
            case Actions.FOOD:
                doFood();
                break;
            default:
                break;       
        }
    }


    void handleKeys()
    {
        if (Input.GetKey(KeyCode.E))
        {
            this.actionEnabled = true;
        }
        else
        {
            this.actionEnabled = false;
        }

        if (Input.GetKey(KeyCode.F))
        {
            handleAction();
        }
    }

	void FixedUpdate ()
    {
        handleKeys();
        handleMovements();
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

        if(coll.gameObject.tag == "Teleport")
        {
            coll.gameObject.GetComponent<TeleportManager>().teleportToExit(this.gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D coll)
    {
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "Moveable" && actionEnabled)
        {
            this.actionEnabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
		string tagName = coll.gameObject.tag;
		collisionManager.enterNotify (tagName);
    }

    void OnTriggerExit2D(Collider2D coll)
    {
		string tagName = coll.gameObject.tag;
		collisionManager.exitNotify (tagName);
    }
}
