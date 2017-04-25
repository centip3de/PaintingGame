using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWellManager : MonoBehaviour
{
    public Vector2 gravity;
    public Vector3 rotation;
    private bool touched;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && !touched)
        {
            Physics2D.gravity = gravity;
            coll.gameObject.transform.Rotate(rotation);
            Destroy(this);
        }
    }
}

