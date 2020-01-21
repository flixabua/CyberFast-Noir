/*
 * written by Felix Völk
 * 
 * Change level with buttons and, close game 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public UnityEngine.UI.Button button1, button2, button3;
    public float timeToUnlockLevel2, timeToUnlockLevel3;
    public static Text playerStats;

    private void Start()
    {
        playerStats = GameObject.Find("Highscore").GetComponent<Text>();
        if (GameStateManager.getHighscore(1) == 0f || GameStateManager.getHighscore(1) >= timeToUnlockLevel2)
        {
            button2.interactable = false;
        }
        else
        {
            button2.interactable = true;
        }
        if (GameStateManager.getHighscore(2) == 0f || GameStateManager.getHighscore(2) >= timeToUnlockLevel3)
        {
            button3.interactable = false;
        }
        else
        {
            button3.interactable = true;
        }
        UpdateStats();
    }
    public void LoadLevelByIndex(int levelIndex)
    {
        UpdateStats();
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadLevelByName(string levelName)
    {
        UpdateStats();
        SceneManager.LoadScene(levelName);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void UpdateStats()
    {
        
        playerStats.text = "Highscores:\n" + 
            "Level 1: " + PlayerPrefs.GetFloat("Score_1").ToString("F2") + "\n" +
            "Level 2: " + PlayerPrefs.GetFloat("Score_2").ToString("F2") + "\n" +
            "Level 3: " + PlayerPrefs.GetFloat("Score_3").ToString("F2") + "\n";
    }

    public void resetHighscore()
    {
        GameStateManager.updateHighscore(1, 0f);
        GameStateManager.updateHighscore(2, 0f);
        GameStateManager.updateHighscore(3, 0f);
        UpdateStats();
    }

}
