/*
 * Written by Jonas Hack
 * 
 * imported form an old project of mine
 * has to be adapted
 * corssfades between different songs, that are made out of randomly selected tracks -> lots of low effort non repetetive music
 */ 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static MusicManager instance; //me

    [SerializeField] private AudioClip[] slow;
    [SerializeField] private AudioClip[] fast;
    [SerializeField] private AudioClip[] dramatic;
    [SerializeField] private AudioClip[] happy;

    private AudioSource[] slowS;
    private AudioSource[] fastS;
    private AudioSource[] dramaticS;
    private AudioSource[] happyS;

    [SerializeField] private int index;
    public int state;


    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float Lerpspeed = 0.5f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (slow.Length != 9 || fast.Length != 9 || dramatic.Length != 9 || happy.Length != 9) Debug.LogError("Music Manager was assigned an incorrect number of titles");
        slowS = new AudioSource[9];
        happyS = new AudioSource[9];
        dramaticS = new AudioSource[9];
        fastS = new AudioSource[9];


        for (int i = 0; i < 9; i++)//always have nine tracks in every case!!!!
        {
            slowS[i] = gameObject.AddComponent<AudioSource>();
            slowS[i].clip = slow[i];
            slowS[i].volume = 0f;

            happyS[i] = gameObject.AddComponent<AudioSource>();
            happyS[i].clip = happy[i];
            happyS[i].volume = 0f;


            dramaticS[i] = gameObject.AddComponent<AudioSource>();
            dramaticS[i].clip = dramatic[i];
            dramaticS[i].volume = 0f;


            fastS[i] = gameObject.AddComponent<AudioSource>();
            fastS[i].clip = fast[i];
            fastS[i].volume = 0f;
        }
    }

    void Start()
    {
        slowS[index].Play();
        fastS[index].Play();
        happyS[index].Play();
        dramaticS[index].Play();
    }

    void FixedUpdate()
    {
        switch (/*gsManager.gamestate*/ state)
        {
            case 1://main menu
                slowS[index].volume = Mathf.Lerp(slowS[index].volume, maxVolume, Lerpspeed);

                fastS[index].volume = Mathf.Lerp(fastS[index].volume, 0, Lerpspeed);
                happyS[index].volume = Mathf.Lerp(happyS[index].volume, 0, Lerpspeed);
                dramaticS[index].volume = Mathf.Lerp(dramaticS[index].volume, 0, Lerpspeed);
                break;

            case 2:
                //wait for case 3
                break;

            case 3://action
                fastS[index].volume = Mathf.Lerp(fastS[index].volume, maxVolume, Lerpspeed);

                slowS[index].volume = Mathf.Lerp(slowS[index].volume, 0, Lerpspeed);
                happyS[index].volume = Mathf.Lerp(happyS[index].volume, 0, Lerpspeed);
                dramaticS[index].volume = Mathf.Lerp(dramaticS[index].volume, 0, Lerpspeed);
                break;

            case 4:
                // wair for case 3
                break;

            case 5://win
                happyS[index].volume = Mathf.Lerp(happyS[index].volume, maxVolume, Lerpspeed);

                fastS[index].volume = Mathf.Lerp(fastS[index].volume, 0, Lerpspeed);
                slowS[index].volume = Mathf.Lerp(slowS[index].volume, 0, Lerpspeed);
                dramaticS[index].volume = Mathf.Lerp(dramaticS[index].volume, 0, Lerpspeed);
                break;

            case 6://lose
                dramaticS[index].volume = Mathf.Lerp(dramaticS[index].volume, maxVolume, Lerpspeed);

                happyS[index].volume = Mathf.Lerp(happyS[index].volume, 0, Lerpspeed);
                fastS[index].volume = Mathf.Lerp(fastS[index].volume, 0, Lerpspeed);
                slowS[index].volume = Mathf.Lerp(slowS[index].volume, 0, Lerpspeed);
                break;

            //rest should be taken care of by main menu


            default:
               // Debug.Log(gsManager.gamestate);
                //fuck
                break;
        }

        if(!slowS[index].isPlaying)
        {
            int oldIndex = index;
            index = Random.Range(0,8);

            slowS[index].Play();
            fastS[index].Play();
            happyS[index].Play();
            dramaticS[index].Play();

            slowS[index].volume = slowS[oldIndex].volume;
            fastS[index].volume = fastS[oldIndex].volume;
            happyS[index].volume = happyS[oldIndex].volume;
            dramaticS[index].volume = dramaticS[oldIndex].volume;
            //terribly ineffeicient
        }
    }
   
}
