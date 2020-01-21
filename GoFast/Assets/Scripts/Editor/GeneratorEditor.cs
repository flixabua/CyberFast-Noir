/*
 * written by Jonas Hack
 * 
 * manually generate in inspector
 * 
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileGenerator))]
public class GeneratorEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TileGenerator myTarget = (TileGenerator)target;
        if (GUILayout.Button("Generate"))
        {   
            myTarget.StartCoroutine(myTarget.generate());
        }
    }
    
}
