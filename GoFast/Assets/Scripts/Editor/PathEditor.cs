/*
 * written by Jonas Hack
 * 
 * makes editing paths way easier (reorder them)
 * 
 * based on examples from api, not quite sure how it works
 * DO NOT DISTURB!
 */ 

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    
    private ReorderableList list;//use the magic thing that makes editors better

    private void OnEnable()
    {
        //reference to the list i am trying to edit
        list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("positions"),
                true, true, true, true);

        //thats where the magic happens
        list.drawElementCallback =
         (Rect rect, int index, bool isActive, bool isFocused) => //for every element
         {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);//get element
            rect.y += 2;//make box containing it big enough

             //draw every axis using weird editor stuff
            EditorGUI.PropertyField(
                 new Rect(rect.x, rect.y, rect.width/3, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("x"), GUIContent.none);
             EditorGUI.PropertyField(
                 new Rect(rect.x + rect.width / 3, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("y"), GUIContent.none);
             EditorGUI.PropertyField(
                 new Rect(rect.x + rect.width / 3 *2, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("z"), GUIContent.none);
         };

        //add a title
        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Points");
        };
    }
    
    public override void OnInspectorGUI()
    {
        //call the main functionality stuff
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}

