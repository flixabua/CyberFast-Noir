/*
 * written by Felix Völk
 * 
 * Script for UI in the playing game, changes Timescale, displays a HUD when slowed and provides a slider
 * 
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public float timepause;
    private bool paused = false;
    public GameObject canvas;
    public GameObject slider;
    void Start()
    {
        canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Pause();
        }
    }

    public void Pause()
    {  
            if (!paused)
            {
                Time.timeScale = timepause;
                canvas.SetActive(true);
                paused = true;
            }
            else
            {
                Time.timeScale = 1f;
                canvas.SetActive(false);
                paused = false;
            }
    }

    public void changeSlider(float newValue)
    {
        //Add mouse sensibility change here
        // float mousesensibility = newValue;
        GameObject.FindObjectOfType<PlayerControllerRefactored>().mouseSpeed = newValue;
    }

    public void LoadLevelByIndex(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadLevelByName(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
