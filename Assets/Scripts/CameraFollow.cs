using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform player;
    public Vector3 offset;
    public bool paused;

    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        if (!paused)
        {
            transform.position = player.transform.position + offset;
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
