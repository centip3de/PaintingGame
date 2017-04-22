using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int deathCounter;

    public void Awake()
    {
        DontDestroyOnLoad(this);

        // FIXME: This is a shitty workaround. In fact there may be no fixing for this.
        // Screw you Unity, for having to either choose an object with no state or duplicate objects.
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    public void loadCurrentLevel()
    {
        deathCounter++;
        print("Current amount of deaths: " + deathCounter.ToString());
        Application.LoadLevel(Application.loadedLevel);
    }
}
