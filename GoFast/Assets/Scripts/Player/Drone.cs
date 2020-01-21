/*
 * written by Jonas Hack
 * 
 * only visuals
 * drone propellers sty upright (mostly)
 * while the body moves to look at suff
 * 
 */ 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] private GameObject hull;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float tilt = 20f;
    private Vector3 last = new Vector3();

    private void Start()//check references
    {
        if (hull == null) Debug.LogError(name + "s hull was not assigned properly for the drone script, check you set it in inspector");
        last = hull.transform.position;
    }

    void FixedUpdate()
    {
        Vector3 delta = (hull.transform.position - last);
        float x = delta.x;
        float y = delta.y;
        float z = delta.z;
        delta = new Vector3(z, y, x);//flip axis
        Vector3 angle = delta * tilt;
       // Debug.Log(angle);
        hull.transform.rotation = Quaternion.Slerp(hull.transform.rotation, Quaternion.Euler(new Vector3(-90,0,0) + angle), speed);//aply rotation (turned by 90 degrees because model import error)

        last = hull.transform.position;
    }
}
