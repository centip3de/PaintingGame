using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour {

    public Transform teleportExit;

    public void teleportToExit(GameObject player)
    {
        print("Poof!");
        player.transform.position = teleportExit.transform.position;
    }
}
