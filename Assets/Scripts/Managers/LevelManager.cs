using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int deathCounter;
    public Dictionary<string, string> transitions;
    public string currentStage;
    public List<string> playedStages;

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void Start()
    {
        transitions = new Dictionary<string, string>();
        loadTransitions();
        playedStages = new List<string>();
    }

    public string getCurrentLevel()
    {
        return Application.loadedLevelName;
    }

    public List<string> getPlayedStages()
    {
        return playedStages;
    }

    public Actions getAllowedAction(string stageName)
    {
        if (stageName == "Escher")
        {
            return Actions.STAIR;
        }
        else if (stageName == "Warhol")
        {
            return Actions.NOODLE;
        }
        else
        {
            return Actions.UMBRELLA;
        }
    }

    void loadTransitions()
    {
        transitions.Add("Title", "World Map");
        transitions.Add("Monet", "World Map");
        transitions.Add("Warhol", "World Map");
        transitions.Add("Escher", "Credits");
    }

	public void nextLevel(string levelName) {
        playedStages.Add(levelName);
		Application.LoadLevel (levelName);
	}

	public void nextLevel()
    {
        string name = Application.loadedLevelName;
		string levelName = transitions [name];

		nextLevel (levelName);
    }

    public void loadMainMenu()
    {
        Application.LoadLevel("Title");
    }

    public void loadCurrentLevel()
    {
        deathCounter++;
        print("Current amount of deaths: " + deathCounter.ToString());
        Application.LoadLevel(Application.loadedLevel);
    }

    public void quitGame()
    {
        print("Quitting game. Bye-bye!");
        Application.Quit();
    }
}
