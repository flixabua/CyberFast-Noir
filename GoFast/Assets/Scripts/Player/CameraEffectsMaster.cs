/*
 * written by Jonas hack
 *
 * triggers stuff like fov-punch to cameras
 * works with "CameraEffects"
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraEffectsMaster : MonoBehaviour
{
    public static CameraEffectsMaster instance;
    public static CameraEffects[] cameraEffects = new CameraEffects[0];
    public AnimationCurve fovPunchCurve = new AnimationCurve();

    private List<Effect> effects = new List<Effect>();


    void Awake()//gets called before start -> there is an instacne for all the other stuff to register to
    {
        reload();
    }   

    public static void fovPunch(float strength, float duration)
    {
        //Debug.Log(" fov punch");
        if (instance != null) instance.applyFovPunch(strength, duration);
        else Debug.LogWarning("Attempted fov punch, but there was no instance of the master");
    }

    public static int addEffect(Effect effect)
    {
        if (instance != null)
        {
            PostProcessVolume volume = instance.gameObject.AddComponent<PostProcessVolume>();
            volume.profile = effect.ppProfile;
            volume.isGlobal = true;
            volume.priority = 1;
            volume.weight = 0;

            effect.ppVolume = volume;

            instance.effects.Add(effect);
            return instance.effects.Count - 1;
        }
        else
        {
            Debug.LogWarning("Attempted to add PP-Volume, but there was no instance of the master");
            return -1;
        }
    }

    public static void setVolumeweight(int index, float weight)//WHY DO THEY NOT AFFECT THE SCENE ?????? AHHHH
    {
        if (instance != null)
        {
            if (index >= 0 && index < instance.effects.Count)
            {
                instance.effects[index].ppVolume.weight = weight;
                if(weight >= 0.5f) RenderSettings.fogColor = instance.effects[index].fogColor;
            }
            else Debug.LogWarning("Attempted to set volume weight, but the index was out of bounds: " + index + ", max allowed: " + instance.effects.Count);
        }
        else Debug.LogWarning("Attempted to set volume weight, but there was no instance of the master");
    }

    /*
    public static void changeColor(Color color)
    {
        if (instance != null) instance.applyChangeColor(color);
        else Debug.LogWarning("Attempted color change, but there was no instance of the master");
    }
    
    private void applyChangeColor(Color color)//FIXME: doesnt update and does not affect gameplay at all
    {
        Debug.Log("Change Color");
        ColorParameter param = new ColorParameter();
        param.value = color;
        param.overrideState = true;
        colorGrading.colorFilter = param;
        //ppVolume.profile.settings.Clear();
        //ppVolume.profile.settings.Add(colorGrading);
    }
    */
    public void applyFovPunch(float strength, float duration)
    {
        for (int i = 0; i < cameraEffects.Length; i++)
        {
            cameraEffects[i].StartCoroutine(cameraEffects[i].fovPunch(strength, duration, fovPunchCurve));
        }
    }
    /*
    private void OnLevelWasLoaded(int level)
    {
        reload();
    }
    */

    public void reload()
    {

        //not a "real" singleton, because plans change and player doesnt want its master to die

        /*if (instance == null)*/
        instance = this;
        //else Destroy(this);
        
 
        CameraEffectsMaster[] masters = GameObject.FindObjectsOfType<CameraEffectsMaster>();
        if (masters.Length > 1)
        {
            Debug.LogWarning("There are multiple cameraEffectsMasters, disabled all, but " + name);
            for(int i = 0; i < masters.Length; i++)
            {
                if (masters[i] != this) masters[i].enabled = false;
            }
        }

        cameraEffects = GameObject.FindObjectsOfType<CameraEffects>();

        //outdated
        /*
        if (this == instance)
        {
            GameObject gObject = new GameObject("dimensionPostprocessVolume");
            gObject.transform.parent = transform;
            gObject.layer = 8;//post processing
            ppVolume = gObject.AddComponent<PostProcessVolume>();
            colorGrading = ppVolume.profile.AddSettings<ColorGrading>();
            ppVolume.isGlobal = true;
            ppVolume.priority = 2;
            ppVolume.weight = 1;
        }
        */
    }
}
