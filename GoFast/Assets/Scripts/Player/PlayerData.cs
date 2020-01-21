/*
 * written by Jonas Hack
 * 
 * stores values used for player movement
 * 
 * can be stored and loaded at will
 * 
 */ 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = " New Player Data", menuName = "Player Info/Player Data")]
public class PlayerData : ScriptableObject
{
    //general variables
    public float maxSpeed = 10f;
    public float maxAccelarationSpeed = 10f;

    public float accelarationForce = 60f;
    [Range(0, 1)]  public float myDrag = 0.8f;



    //jump
     public float jumpForce = 400f;
    [Range(0, 1)]  public float aerialMultiplier = 0.15f;
     public float jumpCooldown = 0.15f;


    //CoyoteTime
     public float groundedMargin = 0.15f;

    //wallRun
    [Range(0, 1)]  public float wallRunVertivalDrag = 0.82f;
    [Range(0, 1)]  public float wallMultiplier = 0.6f;

     public float wallRunCameraTilt = 20f;
     public float wallRunCameraTiltSpeed = 0.15f;

     public float wallRunWallAttraction = 5f;

     public float wallRunDetectionDistance = 1.55f;
     public float wallMinVelocity = 4f;
    // public float wallRunMaxAngle = 60f; // redundant because raycast

    //wallJump
     public float wallRunJumpNormalForce = 500f;
     public float wallRunJumpTangentForce = 200f;
     public float wallRunJumpForwardForce = 600f;
     public float wallRunJumpVerticalForce = 400f;

    //wallTurn
     public float wallTurnDetectionDistance = 2f;
     public float wallTurnVerticalForce = 500f;
     public float wallTurnNormalForce = 300f;
     public float wallTurnMinVerticalSpeed = -5f;
     public float turnSpeed = 500f;


    //TODO: implement a path follow system for dynamic animations instead of the following boosts
    //pullUp
     public float pullUpVercticalForce = 500;
     public float pullUpForwardForce = 50;
    /*
    //skip
     public float skipVercticalForce = 300;
     public float skipForwardForce = 500;

    //slide
    bool tryingToSlide = false;
    */

}
