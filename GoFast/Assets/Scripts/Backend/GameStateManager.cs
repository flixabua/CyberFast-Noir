/*
 * written by Felix Völk
 * 
 * Used to save highscores and get them
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    // Start is called before the first frame update
    float highscore_1;
    float highscore_2;
    float highscore_3;
    void Start()
    {
        highscore_1 = PlayerPrefs.GetFloat("Score_1", 0f);
        highscore_2 = PlayerPrefs.GetFloat("Score_2", 0f);
        highscore_3 = PlayerPrefs.GetFloat("Score_3", 0f);
    }

    public static void updateHighscore(int index, float score)
    {
        PlayerPrefs.SetFloat("Score_" + index, score);
        Debug.Log(PlayerPrefs.GetInt("Score"));
    }

    public static float getHighscore(int index) {
        return PlayerPrefs.GetFloat("Score_" + index);
    }
}
