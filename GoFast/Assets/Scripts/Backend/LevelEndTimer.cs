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

    [SerializeField] GameObject konfetti;

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

        if (konfetti == null) Debug.LogError("Konfetti of " + gameObject.name + " was null");
    }

    public void startTimer()
    {
        timing = true;
        time = 0.0f;
    }

    public void endTimer()
    {
        timing = false;
    }

    public void safeTime()
    {
        //safe time using streamwriter
    }

    private void updateTime()
    {
        time += Time.deltaTime * Time.timeScale;//timescale for pause

        int min =(int)time / 60;
        float seconds = (time - min * 60);
        string secs = "";

        //formatting
        text.text = "";

        if (seconds < 10) secs = "0";
        secs += seconds.ToString("F2");
        

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
        //Time.timeScale = 0.0001f;

        timing = false;
        if (delay) yield return new WaitForSecondsRealtime(restartTime);

       /*
        while (!Input.anyKeyDown)
        {
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 1.0f;
        */
        startTimer();
        yield return null;
    }

    public IEnumerator nextLevel(bool delay)
    {
        Time.timeScale = 0.001f;
        endTimer();
       
        //int level = SceneManager.GetActiveScene().buildIndex;
        //Debug.Log("lvl: " + level + ", count: " + SceneManager.sceneCountInBuildSettings);
        // level++;
        // if (level >= SceneManager.sceneCountInBuildSettings) level = 0;
        //ebug.Log(level);

        if (time < record || record == 0f)
        {
            GameStateManager.updateHighscore(SceneManager.GetActiveScene().buildIndex, time);
            Debug.Log("New Record: " + time);
            Instantiate(konfetti, player.transform.position + player.transform.forward*2 + player.transform.up, player.transform.rotation);
            text.color = Color.white;
            if (delay) yield return new WaitForSecondsRealtime(restartTime);

        }
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
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
