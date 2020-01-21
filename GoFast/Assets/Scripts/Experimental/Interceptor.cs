
/*
* written by Jonas Hack
* 
* Test if it is possible to a moving target, given a projectile speed
* and gives the required velocity vector
* 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Intersection
{
   //returns if to objects will come within margin units off each other in the next timeframe seconds
   public static bool willIntersect(Vector3 pos1, Vector3 vel1, Vector3 pos2, Vector3 vel2,float timeFrame, float margin)
   {
       for (int t = 0; t < timeFrame; t++)
       {
           Vector3 expextedPos1 = pos1 + (vel1 * t);
           Vector3 expextedPos2 = pos2 + (vel2 * t);

           if (Vector3.Distance(expextedPos1, expextedPos2) <= margin)
           {
               return true;
           }
       }
       return false;
   }

   //returns velocity Vector3 (direction) objects2 will need to travel at speed2 to meet object1 which itself travels. returns empty if impossible
   public static Vector3 IntersectionVelocity(Vector3 pos1, Vector3 vel1, Vector3 pos2, float speed2, float timeFrame, float margin, float timeStep)
   {
       Vector3 result = new Vector3();

        //if the target is not moving, dont calculate all that shit
        if (vel1.magnitude < 0.001)//velocity isnt usually truely 0 because floats and physics are weird
        {
            Vector3 delta = pos1 - pos2;

            result = delta.normalized * speed2;

            return result;
        }
        

       //find out it if object1 is in range
       bool inRange = false;
       float timeStamp = -1;
       for (float t = 0; t < timeFrame; t += timeStep)
       {
           float range = t * speed2;//how far can have I gone at t seconds
           Vector3 expectedPos1 = pos1 + (t * vel1);

           Debug.DrawLine(pos1, expectedPos1, Color.green);
           //Debug.Log(vel1);
           //Debug.Log(((expectedPos1 - pos1) - vel1).magnitude <= margin);

           if (Mathf.Abs(Vector3.Distance(expectedPos1, pos2) - range) <= margin)
           {
               Debug.DrawLine(pos2, expectedPos1, Color.red);

               inRange = true;
               timeStamp = t;
               t = timeFrame;
           }
       }

       //get vector between points
       if (inRange)
       {
           result = (pos1 + (timeStamp * vel1)) - pos2;
       }

       return result;
   }
}
