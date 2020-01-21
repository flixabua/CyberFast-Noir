/*
 * written by Jonas Hack
 * 
 * gives lifes to platforms
 * finals state machine
 * move, rotate, etc
 * 
 */ 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPlatform : ButtonObject
{
    private List<Rigidbody> rigidsOnMe = new List<Rigidbody>();

    public enum type {still, path, circle, rotate};
    public type behaviour = type.still;

    ///*[HideInInspector]*/ public List<Vector3> path = new List<Vector3>();
    [HideInInspector] public Path path;
    [HideInInspector] public float endPointWaitTime = 3f;
    [HideInInspector] public bool waitAtEveryPoint = false;
    [HideInInspector] public float moveSpeed = 3f;
    [HideInInspector] public Vector3 rotation = new Vector3(1, 1, 1);
    [HideInInspector] public Vector3 center = new Vector3(0, 0, 0);

    private float radius = 0;
    private int pathIndex = 0;
    private int pathDirection = 1;

    private Vector3 lastPosition = new Vector3();

    private bool halt = false;

    private void Start()//check refernces
    {
        radius = Vector3.Distance(transform.position, center);
        if (path == null) path = new Path(transform.position);

        if(behaviour == type.path)transform.position = path.positions[0];
        lastPosition = transform.position;

    }


    void Update()
    {
        //do my own behaviour stuff
        switch (behaviour)
        {
            case type.still:
                //dont do anything
                break;

            case type.path://follow path and possibly wait at end/certain points
                bool end = end = pathIndex >= path.Length() || pathIndex < 0;
                if(!halt)//if i am not waiting
                {
                    if(!end)
                    {
                        //dont overshoot
                        if (Vector3.Distance(transform.position, path.positions[pathIndex]) > moveSpeed * Time.timeScale * Time.deltaTime)
                        {
                            transform.Translate((path.positions[pathIndex] - transform.position).normalized * moveSpeed * Time.timeScale * Time.deltaTime);
                        }
                        else
                        {
                            pathIndex += pathDirection;
                            if (waitAtEveryPoint) StartCoroutine(waitPath());//wait
                        }
                    }
                    else
                    {
                        StartCoroutine(waitPath());//wait at end
                        //change direction
                        pathDirection = -pathDirection;
                        if (pathDirection <= 0) pathIndex = path.Length() - 1;
                        else pathIndex = 0;

                    }

                }               
                break;

            case type.circle://circle around center point
                //set pos according to standart algebra circle stuff
                float x = Mathf.Cos(Time.time * moveSpeed / 6) * radius + center.x;
                float z = Mathf.Sin(Time.time * moveSpeed / 6) * radius + center.z;
                Vector3 newPos = new Vector3(x, center.y, z);
                transform.position = Vector3.Lerp(transform.position, newPos, Time.timeScale);
                break;

            case type.rotate://rotate along own axis
                transform.Rotate(rotation * Time.deltaTime * Time.timeScale);
                break;
        }
    }

    private void FixedUpdate()
    {
        //move stuff ontop of me
        Vector3 deltaPosition = transform.position - lastPosition;
        //BoxCollider col = GetComponent<BoxCollider>();//TODO: make this less expensive
        foreach (Rigidbody rigid in rigidsOnMe)
        {
            //float influence = Vector3.Project(transform.position - rigid.position, transform.up).magnitude / col.size.y;
            //Debug.Log(influence);
            switch (behaviour)
            {
                case type.still:
                    //dont do anything
                    break;

                case type.path:
                    rigid.transform.position += deltaPosition /** influence*/; // move with me
                    break;

                case type.circle:
                        rigid.transform.position += deltaPosition; //move with me
                    break;

                case type.rotate:
                        Quaternion rigidRot = rigid.transform.rotation;
                        rigid.transform.RotateAround(transform.position, rotation, rotation.magnitude * Time.deltaTime); // rotate with me along my axis
                        rigid.transform.rotation = rigidRot;//dont mess with the rotation itself (camera etc)
                    break;
            }
        }

        lastPosition = transform.position;//stay up to date with delta postion
    }

    //keep track of the stuff that has to move with me
    void OnTriggerEnter(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if (rigid != null)
        {
            rigidsOnMe.Add(rigid);
        }
    }
    void OnTriggerExit(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if (rigid != null)
        {
            rigidsOnMe.Remove(rigid);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        switch (behaviour)
        {
            case type.still:
                //dont do anything
                break;

            case type.path:
                /*for(int i = 0; i < path.Capacity; i++)
                {
                    Vector3 p = path[i];
                    Gizmos.DrawSphere(p, 0.2f);
                    if(i < path.Capacity-1)
                    Gizmos.DrawLine(p, path[i+1]);
                }*/
                break;

            case type.circle:
                Gizmos.DrawSphere(center, 0.2f);
                Gizmos.DrawLine(center, transform.position);
                break;

            case type.rotate:
                
                break;
        }
    }

    public override void trigger()
    {
        change();
        behaviour += 1;
        if (behaviour > type.rotate) behaviour = type.still;
        //Debug.Log(this.name + "-platform was triggered");
    }

    IEnumerator waitPath()
    {
        halt = true;
        yield return new WaitForSeconds(endPointWaitTime);
        halt = false;
    }
}