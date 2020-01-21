/*
 * written by jonas hack
 * 
 * times player and loads next level when finished
 * 
 * -> maybe add menu?
 */ 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelEndTimer : myReset
{
    [SerializeField] private float restartTime = 0.2f;

    private PlayerControllerRefactored player;
    //private Camera camera;

    [SerializeField] private Text text;
    private float time = 0.0f;
    bool timing = false;


    private float record = 190f;
    private float maxDistance = 1000;
    private float distance = 0;

    void Update()
    {
        if (timing)
        {
            updateTime();
        }
    }

    void Awake()
    {
        /* GameObject objs = GameObject.FindObjectOfType(typeof(masterClass)) as GameObject;

         if (objs != null)
         {
             Destroy(this.gameObject);
         }

         DontDestroyOnLoad(this.gameObject);*/

        player = GameObject.FindObjectOfType(typeof(PlayerControllerRefactored)) as PlayerControllerRefactored;
        //camera = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
        if (text == null) text = GameObject.Find("timeText").GetComponent(typeof(Text)) as Text;

        startTimer();

        maxDistance = Vector3.Distance(player.transform.position, transform.position);
        //TODO: get record!!!
        record = GameStateManager.getHighscore(SceneManager.GetActiveScene().buildIndex);
    }

    public void startTimer()
    {
        timing = true;
        time = 0.0f;
    }

    public void endTimer()
    {
        timing = false;
        if (time < record || record == 0f)
        {
            GameStateManager.updateHighscore(SceneManager.GetActiveScene().buildIndex, time);
            text.color = Color.white;
        }
    }

    public void safeTime()
    {
        //safe time using streamwriter
    }

    private void updateTime()
    {
        time += Time.deltaTime * Time.timeScale;//timescale for pause

        int min = Mathf.RoundToInt(time) / 60;
        float seconds = Mathf.Round((time - min * 60) * 100) / 100;
        string secs = "";

        //formatting
        text.text = "";

        if (seconds < 10) secs = "0";
        secs += seconds.ToString();
        if ((seconds * 100) % 10 == 0)
        {
            secs += "0";
            if ((seconds * 10) % 10 == 0) secs += "0";
        }

        if (min != 0) text.text = min + ":";
        text.text += secs;


        //color
        distance = Vector3.Distance(player.transform.position, transform.position);//wie weit ist es noch
        if (distance < maxDistance - (maxDistance / record * time))//verglichen mit wie weit es noc seien könnte (durchschnittliche geschwindigkeit)
        {
            text.color = Color.green;
        }
        else
        {
            text.color = Color.red;
        }
        //Debug.Log(distance + ": " + (maxDistance - (maxDistance / record * time)));
    }

    public override void ResetMe()//override
    {
        StartCoroutine(restart(false));
    }
    

    public IEnumerator restart(bool delay)
    {
        Time.timeScale = 0.0001f;
        timing = false;
        if (delay) yield return new WaitForSecondsRealtime(restartTime);

        //reset everything;
       // myReset.ResetAll();
       //not my job

        while (!Input.anyKeyDown)
        {
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 1.0f;
        startTimer();
        yield return null;
    }

    public IEnumerator nextLevel(bool delay)
    {
        Time.timeScale = 0.001f;
        endTimer();
        if (delay) yield return new WaitForSecondsRealtime(restartTime);
        Time.timeScale = 1.0f;
        int level = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("lvl: " + level + ", count: " + SceneManager.sceneCountInBuildSettings);
        level++;
        if (level >= SceneManager.sceneCountInBuildSettings) level = 0;
        Debug.Log(level);
        
        SceneManager.LoadScene(level);
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player.gameObject)
        {
            //save highscore
            StartCoroutine(nextLevel(true));
        }
    }
}
