using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Transform playerTransform;
    public bool isGrounded;
    public float speed = 1f;
    public bool dead;
    public Vector2 gravity;

    private bool spacePressed;
    private bool isClimbing;
    private bool actionEnabled;
    private GameObject moving;

	void Start ()
    {
        isClimbing = false;
        spacePressed = false;
        gravity = Physics.gravity;
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
        Vector2 up = (playerTransform.up * Input.GetAxisRaw("Vertical"));
        Vector2 right = playerTransform.right * Input.GetAxisRaw("Horizontal");
        if(!isClimbing)
        {
            playerTransform.Translate((up + right).normalized * speed * Time.smoothDeltaTime);
        }
        else
        {
            // Gravity begins to act weird when we try to re-climb.
            // Solution is maybe just give the player one chance to climb?
            Physics2D.gravity = Vector2.zero;
            playerTransform.Translate(playerTransform.up * speed * Time.smoothDeltaTime);
        }

        if (moving != null)
        {
            moving.transform.Translate((up + right).normalized * speed * Time.smoothDeltaTime);
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
        if(Input.GetKey(KeyCode.Space))
        {
            this.spacePressed = true;
        }
        else
        {
            this.spacePressed = false;
            this.isClimbing = false;
            Physics2D.gravity = this.gravity;
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
        if(coll.gameObject.tag == "Climbable" && spacePressed)
        {
            this.isClimbing = true;
        }
        else
        {
            this.isClimbing = false;
            Physics2D.gravity = this.gravity;
        }
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "Climbable" && spacePressed)
        {
            this.isClimbing = true;
        }
        else
        {
            this.isClimbing = false;
            Physics2D.gravity = this.gravity;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "Climbale")
        {
            this.isClimbing = false;
            Physics2D.gravity = this.gravity;
        }
    }
}
