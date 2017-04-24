using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform player;
    public bool paused;

    void LateUpdate()
    {
        if (!paused)
        {
            Vector3 pos = new Vector3(player.transform.position.x, player.transform.position.y, -10);
            transform.position = pos;
        }
    }

    void OnPauseGame()
    {
        paused = true;
    }

    void OnResumeGame()
    {
        paused = false;
    }
}
