/*
 * written by Jonas Hack
 * 
 * basically just a Vector3, but I can use it in Editor/Inspector
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point
{
    public float x = 0;
    public float y = 0;
    public float z = 0;

    public Point(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Point(Vector3 vec)
    {
        this.x = vec.x;
        this.y = vec.y;
        this.z = vec.z;
    }

    public Vector3 get()
    {
        return new Vector3(x, y, z);
    }

    public void set(Vector3 n)
    {
        x = n.x;
        y = n.y;
        z = n.z;
    }


    //make it possible to cast to other datatypes
    public static implicit operator Vector3 (Point x)
    {
         return x.get();
    }

    public static implicit operator Point(Vector3 x)
    {
        return new Point(x);
    }

    /*
    public static explicit operator Vector3[](Point[] x)
    {
        Vector3[] ret = new Vector3[x.Length];

        for(int i = 0; i < x.Length; i++)
        {
            ret[i] = x[i].get();
        }

        return ret;
    }*/
}
