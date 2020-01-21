/*
* written by Jonas Hack
* 
* stores a list of positions
* combined with the custom inpector, it makes it easy to create "paths" in editor
* 
* can be used as camera dolly track, animations follow or possibly for ai
* 
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "Path", menuName = "Custom/Path", order = 1)]
public class Path : MonoBehaviour
{
   [HideInInspector] public List<Point> positions = new List<Point>();
   [SerializeField] private Color gizmoColor = Color.black;
   [SerializeField] private float gizmoSize = 0.1f;



   public Path()
   {
       positions.Add(new Vector3());
   }
   public Path(Vector3 v)
   {
       positions.Add(v);
   }
   public Path(List<Point> v)
   {
       positions = v;
   }

   public Vector3[] positionsArray()//converts to array of vector3
   {
       Vector3[] path = new Vector3[positions.Count];
       for (int i = 0; i < path.Length; i++)
       {
           path[i] = positions[i].get();
       }
       return path;
   }

   public int Length()
   {
       return positions.Count;
   }

   //never tested
   public void addAtNearestPoint(Vector3 v)//inserts a point into the path where it would make most sense regarding position
   {
       float minDistance = float.MaxValue;
       int index = 0;
       foreach (Point p in positions)
       {
           float dis = Vector3.Distance(p, v);
           if (dis < minDistance)
           {
               minDistance = dis;
               index = positions.IndexOf(p);
           }
       }
       positions.Insert(index, v);
   }

   private void OnDrawGizmos()
   {
       Vector3[] path = positionsArray(); 

       Gizmos.color = gizmoColor;
       for (int i = 0; i < path.Length - 1; i++)
       {
           Gizmos.DrawSphere(path[i], gizmoSize);
           Gizmos.DrawLine(path[i], path[i + 1]);
       }
       Gizmos.DrawSphere(path[path.Length - 1], gizmoSize);
   }

}
