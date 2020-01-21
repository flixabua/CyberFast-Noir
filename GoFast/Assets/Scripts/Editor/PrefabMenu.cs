/*
 * Written by Jonas Hack
 * add menu to create prefabs and more importantly stramline leveldesign using keyboard shortcuts
 * not working with current folder structure
 * might redo later or scrap
 * 
 * /


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using UnityEditor.SceneManagement;


public class PrefabMenu : MonoBehaviour
{
    [MenuItem("Prefabs/Player %1")]
    public static void createPlayer()
    {
        GameObject obj = Resources.Load("Prefabs/Player", typeof(GameObject)) as GameObject;
        Instantiate(obj);
    }

    [MenuItem("Prefabs/Path %2")]
    public static void createPath()
    {
        GameObject obj = Resources.Load("Prefabs/Path", typeof(GameObject)) as GameObject;
        Instantiate(obj);
    }

    [MenuItem("Prefabs/Resetter %3")]
    public static void createResetter()
    {
        GameObject obj = Resources.Load("Prefabs/Path", typeof(GameObject)) as GameObject;
        Instantiate(obj);
    }

    [MenuItem("Prefabs/Drone %4")]
    public static void createDrone()
    {
        GameObject obj = Resources.Load("Prefabs/CameraDrone", typeof(GameObject)) as GameObject;
        Instantiate(obj);
    }

    [MenuItem("Prefabs/Effects %5")]
    public static void createEffects()
    {
        GameObject obj = Resources.Load("Prefabs/AreaEffects", typeof(GameObject)) as GameObject;
        Instantiate(obj);
    }
}
*/