/*
 * written by Jonas Hack
 * 
 * streamlines platform editor by only drawing what you need right now
 * 
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SpecialPlatform))]
public class PlatformEditor : Editor
{
    
    SerializedProperty speed;
    SerializedProperty path;
    SerializedProperty wait;
    SerializedProperty rot;
    SerializedProperty cent;
    SerializedProperty point;

    void OnEnable()//get refernces to attributes
    {
        speed = serializedObject.FindProperty("moveSpeed");
        path = serializedObject.FindProperty("path");
        wait = serializedObject.FindProperty("endPointWaitTime");
        rot= serializedObject.FindProperty("rotation");
        cent = serializedObject.FindProperty("center");
        point = serializedObject.FindProperty("waitAtEveryPoint");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        SpecialPlatform myTarget = (SpecialPlatform) target;//reference script
        switch(myTarget.behaviour)//choose what to draw
        {
            case SpecialPlatform.type.still:
                break;
            case SpecialPlatform.type.path:
                EditorGUILayout.PropertyField(speed);
                EditorGUILayout.PropertyField(wait);
                EditorGUILayout.PropertyField(point);
                EditorGUILayout.PropertyField(path,true);
                break;
            case SpecialPlatform.type.circle:
                EditorGUILayout.PropertyField(speed);
                EditorGUILayout.PropertyField(cent);
                break;
            case SpecialPlatform.type.rotate:
                EditorGUILayout.PropertyField(rot);
                break;
        }

 
        serializedObject.ApplyModifiedProperties();
    }   
}
