/*
 * written by Jonas Hack
 * 
 * add buttons to manually trigger a reload as the player controller does not check for changes automatically
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerControllerRefactored))]
public class PlayerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerControllerRefactored myTarget = (PlayerControllerRefactored)target;
        if (GUILayout.Button("Load Player Data"))
        {
            myTarget.StartCoroutine(myTarget.loadPlayerData());
        }
        if (GUILayout.Button("Load Input Data"))
        {
            myTarget.StartCoroutine(myTarget.loadInputData());
        }
    }

}

