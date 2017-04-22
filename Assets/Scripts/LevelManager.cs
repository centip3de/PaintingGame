using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int deathCounter;
    public Dictionary<string, string> transitions;

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

    public void Start()
    {
        transitions = new Dictionary<string, string>();
        loadTransitions();
    }

    void loadTransitions()
    {
        transitions.Add("Title", "World Map");
        transitions.Add("World Map", "Level2");
        transitions.Add("Level2", "Level3");
        transitions.Add("Level3", "Credits");
    }

    public void nextLevel()
    {
        string name = Application.loadedLevelName;
        Application.LoadLevel(transitions[name]);
    }

    public void loadCurrentLevel()
    {
        deathCounter++;
        print("Current amount of deaths: " + deathCounter.ToString());
        Application.LoadLevel(Application.loadedLevel);
    }
}
