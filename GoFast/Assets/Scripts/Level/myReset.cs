
/*
* written by Jonas Hack
* 
* handles respawn and level reset
* both as the thing trigerring it and the thing that is reset
* 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class myReset : MonoBehaviour
{
   Vector3 myResetPos = new Vector3();
   Quaternion myResetRot = Quaternion.identity;
   private Vector3[] resetPos = new Vector3[0];
   private Quaternion[] resetRot = new Quaternion[0];

   Vector3 resetVelocity = new Vector3();

   [SerializeField] bool amResetTrigger = false;

   public virtual void setValues()//set the values it will return to when reset
   {
       //my values
       myResetPos = transform.position;
       myResetRot = transform.rotation;

       //values of my children
       //childTrans = new Transform[transform.childCount];
       resetPos = new Vector3[transform.childCount];
       resetRot = new Quaternion[transform.childCount];
       for(int i = 0; i < transform.childCount; i++)
       {
           //childTrans[i] = transform.GetChild(i);
           resetPos[i] = transform.GetChild(i).position;
           resetRot[i] = transform.GetChild(i).rotation;
       }

       //physics
       Rigidbody rigid = GetComponent<Rigidbody>();
       if (rigid != null) resetVelocity = rigid.velocity;
   }

   void Start()
   {
       //DONT FORGET TO CALL SETVALUES IN START WHEN EXTENDING CLASS!
       setValues();
   }

   public virtual void ResetMe()//set values to reset values
   {
        //me
       transform.position = myResetPos;
       transform.rotation = myResetRot;

       //children
       for (int i = 0; i < transform.childCount; i++)
       {
           Transform child = transform.GetChild(i);
           if (child != null && i < resetPos.Length)
           {
               child.position = resetPos[i];
               child.rotation = resetRot[i];
           }

       }

       //physics
       Rigidbody rigid = GetComponent<Rigidbody>();
       if (rigid != null) rigid.velocity = resetVelocity;
   }

   public static void ResetAll()//reload level
   {
       myReset[] others = GameObject.FindObjectsOfType<myReset>();
       for(int i = 0; i < others.Length; i++)
       {
           others[i].ResetMe();
       }
   }

   public static void ResetHard()//reload level
   {
       SceneManager.LoadScene(SceneManager.GetActiveScene().ToString());
   }

   private void OnTriggerEnter(Collider other)
   {
       if(amResetTrigger)//am i a death zone?
       {
           if(other.tag == "Player")//do i need to reload?
           {
               ResetAll();
           }
           else // standart stuff just gets put back
           {
               myReset otherReset = other.GetComponent<myReset>();
               if(otherReset != null)
               {
                   otherReset.ResetMe();
               }
           }
       }
   }

   private void OnDrawGizmos()
   {
       if(amResetTrigger)
       {
           Gizmos.color = new Color(1,0,0,0.2f);
           Gizmos.DrawCube(transform.position, transform.localScale);
       }
       //Gizmos.DrawIcon(myResetPos, gameObject.name);
       Gizmos.DrawSphere(myResetPos, 0.1f);
   }
}