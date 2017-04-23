using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWellManager : MonoBehaviour
{
    public Vector2 gravity;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            Physics2D.gravity = gravity;
        }
    }
}

