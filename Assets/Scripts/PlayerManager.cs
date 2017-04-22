using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class PlayerManager : MonoBehaviour
{
    public Transform playerTransform;
    public bool isGrounded;
    public float speed = 1f;
    public bool dead;
    public Vector2 gravity;
    public GameObject noodle;

    private bool spacePressed;
    public bool isClimbing;
    private bool actionEnabled;
    private GameObject moving;
	private CollisionManager collisionManager;

	void Start ()
    {
		collisionManager = CollisionManager.builder (this)
			.add (new ClimbableObserver())
			.build ();

        isClimbing = false;
        spacePressed = false;
        gravity = Physics2D.gravity;
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

		Vector3 displacement = new Vector3 (horizontalSign, verticalSign, 0);
		float scale = speed * Time.smoothDeltaTime;

		playerTransform.Translate (displacement * scale);

    }

    void launchNoodle()
    {
        Vector2 rayCastStart = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 1);
        RaycastHit2D hit = Physics2D.Raycast(rayCastStart, Vector2.up);
        if (hit.collider != null && hit.collider.gameObject != this.gameObject && hit.collider.tag != "Climbable")
        {
            GameObject newObj = Instantiate(noodle, hit.point, Quaternion.identity);
            Vector3 scale = new Vector3(newObj.transform.localScale.x, 0.01f);
            newObj.transform.localScale = scale;
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


    void handleKeys()
    {
        if (Input.GetKey(KeyCode.E))
        {
            this.actionEnabled = true;
        }
        else
        {
            this.actionEnabled = false;
            this.moving = null;
        }

        if (Input.GetKey(KeyCode.F))
        {
            launchNoodle();
        }

        if(Input.GetKey(KeyCode.Space))
        {
            this.spacePressed = true;
        }
        else
        {
            this.spacePressed = false;
        }
    }

	void FixedUpdate ()
    {
        handleKeys();
        handleMovements();
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Moveable" && actionEnabled)
        {
            moving = coll.gameObject;
        }

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
        if(coll.gameObject.tag == "Moveable" && actionEnabled)
        {
            moving = coll.gameObject;
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "Moveable" && actionEnabled)
        {
            this.actionEnabled = false;
            this.moving = null;
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
		string tagName = coll.gameObject.tag.ToUpper ();
		CollisionType type = (CollisionType) Enum.Parse (typeof(CollisionType), tagName);

		collisionManager.enterNotify (type);
    }

    void OnTriggerExit2D(Collider2D coll)
    {
		string tagName = coll.gameObject.tag.ToUpper ();
		CollisionType type = (CollisionType) Enum.Parse (typeof(CollisionType), tagName);

		collisionManager.exitNotify (type);
    }
}
