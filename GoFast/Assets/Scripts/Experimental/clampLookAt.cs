
 /*
 * written by Jonas Hack
 * 
 * Looks at a given point in space limited by max rotation
 * -> turrents dont look behind them
 * 
 */
 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clampLookAt : MonoBehaviour
{
    [SerializeField] private float minX = -90, maxX = 90, minY = -90, maxY = 90, minZ = -90, maxZ = 90;
    public Transform target;


    private void Start()
    {
        GameObject parent = new GameObject();
        parent.transform.position = transform.position;
        parent.transform.rotation = transform.rotation;
        parent.transform.parent = transform.parent;
        parent.name = this.name + "-procedualParent";
        transform.parent = parent.transform;
        //transform.rotation = transform.parent.rotation;//should be redundant, but why not
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target, Vector3.up);
            Vector3 myRotation = transform.localRotation.eulerAngles; //transform.rotation.eulerAngles;

            //make it more intuitive
            if (myRotation.x > 180) myRotation.x -= 360;
            if (myRotation.y > 180) myRotation.y -= 360;
            if (myRotation.z > 180) myRotation.z -= 360;
            if (myRotation.x < -180) myRotation.x += 360;
            if (myRotation.y < -180) myRotation.y += 360;
            if (myRotation.z < -180) myRotation.z += 360;


            //enforce max rotation
            if (myRotation.x < minX) myRotation.x = minX;
            else if (myRotation.x > maxX) myRotation.x = maxX;

            if (myRotation.y < minY) myRotation.y = minY;
            else if (myRotation.y > maxY) myRotation.y = maxY;

            if (myRotation.z < minZ) myRotation.z = minZ;
            else if (myRotation.z > maxZ) myRotation.z = maxZ;

            transform.rotation = Quaternion.Euler(myRotation + transform.parent.rotation.eulerAngles);//apply

        }
    }

    private void OnDrawGizmos()
    {
        if(target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
            float distance = Vector3.Distance(transform.position, target.position);
            Vector3 lookAt = transform.position + transform.forward * distance;
            Gizmos.DrawLine(transform.position, lookAt);
            Gizmos.color = Color.black;
            Gizmos.DrawLine(target.position, lookAt);
        }
    }
}
