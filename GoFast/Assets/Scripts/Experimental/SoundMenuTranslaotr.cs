/*
 * written by Jonas Hack
 * 
 * imported form an old project of mine
 * has to be adapted!
 * 
 * is supposed to be sound menu
 */ 



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundMenuTranslaotr : MonoBehaviour {
/*
    public Slider music, ambient, effects, voice, misc;
    private SoundProfile custom;

    void Start()//why do the sliders work here but nowehere else?
    {
        Debug.Log("Music: " + music.value + ", Ambient: " + ambient.value + ", Effects: " + effects.value + ", Voice: " + voice.value + ", Misc: " + misc.value);

    }
  
    public void applyCustom()//apply user volume changes
    {
        
        Debug.Log("Music: " + music.value + ", Ambient: " + ambient.value + ", Effects: " + effects.value + ", Voice: " + voice.value + ", Misc: " + misc.value);
        
        SoundProfile custom = ScriptableObject.CreateInstance("SoundProfile") as SoundProfile;
        custom.name = "custom";
        custom.types = new SoundType[5];

        custom.types[0] = new SoundType();
        custom.types[0].volume = music.value;
        custom.types[0].name = "Music";

        custom.types[1] = new SoundType();
        custom.types[1].volume = ambient.value;
        custom.types[1].name = "Ambient";

        custom.types[2] = new SoundType();
        custom.types[2].volume = effects.value;
        custom.types[2].name = "Effects";

        custom.types[3] = new SoundType();
        custom.types[3].volume = voice.value;
        custom.types[3].name = "Voice";

        custom.types[4] = new SoundType();
        custom.types[4].volume = misc.value;
        custom.types[4].name = "Misc";



        AudioManager audio = GameObject.FindObjectOfType<AudioManager>();
        audio.profiles[1] = custom; //weird way to do it...
        audio.changeProfile("custom");
    }

*/
}
