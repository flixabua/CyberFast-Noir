/*
 * written by Jonas Hack
 * 
 * saves data for every dimension
 */ 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " New Dimension", menuName = "Dimensions/Dimension")]
public class Dimension : ScriptableObject
{
    public Material mat;
    public int index = 0;
    public UnityEngine.Rendering.PostProcessing.PostProcessProfile postProcessingVolume;
    public Color fogColor = Color.blue;
}
