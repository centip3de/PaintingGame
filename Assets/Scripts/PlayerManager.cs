using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Transform playerTransform;
    public Vector2 velocity;
    public bool isGrounded;
    public float speed = 1f;
    public bool dead;

    private bool actionEnabled;
    private GameObject moving;

	void Start ()
    {
	
	}

    void die()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    void handleMovements()
    {
        Vector2 up = (playerTransform.up * Input.GetAxisRaw("Vertical"));
        Vector2 right = playerTransform.right * Input.GetAxisRaw("Horizontal");
        playerTransform.Translate((up + right).normalized * speed * Time.deltaTime);
        if (moving != null)
        {
            moving.transform.Translate((up + right).normalized * speed * Time.deltaTime);
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
    }
	
	void Update ()
    {
        handleKeys();
        handleMovements();
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        print(actionEnabled);
        if (coll.gameObject.tag == "Moveable" && actionEnabled)
        {
            moving = coll.gameObject;
        }
        if(coll.gameObject.tag == "Death")
        {
            die();
        }
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        print(actionEnabled);
        if(coll.gameObject.tag == "Moveable" && actionEnabled)
        {
            moving = coll.gameObject;
        }
    }
}
