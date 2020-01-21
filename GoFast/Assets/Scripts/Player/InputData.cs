/*
 * written by Jonas Hack
 * 
 * stores input profile
 * 
 * -> possible to store and switch between profiles
 */ 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " New Input Data", menuName = "Player Info/Input Data")]
public class InputData : ScriptableObject
{
    public string moveAxisHoriz = "Horizontal";
    public string moveAxisVerti = "Vertical";

    public string lookAxisHoriz = "Mouse X";
    public string lookAxisVerti = "Mouse Y";
    public float mouseSpeed = 1f;

    public bool useRawInput = true;
    public bool invertXAxis = false;
    public bool invertYAxis = false;

    public string controllerAxisHoriz = "Horizontal2";
    public string controllerAxisVerti = "Mouse X";
    public bool useController = false;

    public string jump = "Jump";

    public string reset = "backspace";
    public string quit = "return";
    public string cancel = "Cancel";

    public string interact = "Interact";
    public string switchButton = "Switch";
}

