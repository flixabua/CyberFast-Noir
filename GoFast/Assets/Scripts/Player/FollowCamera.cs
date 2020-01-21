/*
 * written by Jonas Hack
 * 
 * my version of cinemachine
 * -> differnet camera types all made to follow the player
 * 
 * probably wont get used because of cut features
 */ 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraEffects))]
public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

   public enum types { relative, abolute, path};
    [SerializeField] private types state = types.relative;
    [SerializeField] private float lookSpeed = 0.5f;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float pointSteps = 10f;
    [SerializeField] private Vector3 offset = new Vector3(2,5,2);
    [SerializeField] private Path path;
    private Vector3 lookAt = new Vector3(); 

    private void Start()//check values
    {
        if (target == null)
        {
            Debug.LogError("followCamera was not assigned a target");
        }
        lookAt = target.position;

        if (path == null) path = new Path(transform.position);
    }


    void Update()
    {
        lookAtTarget(lookSpeed);//TODO: normalize Time. time.deltatime doesnt work as smoothing factor
        goToPosition(moveSpeed);

    }

    private void lookAtTarget(float dTime) //look at the desired psotiotin
    {
        lookAt = Vector3.Lerp(lookAt, target.position, lookSpeed * dTime);//smooth
        transform.LookAt(lookAt, Vector3.up);
        //TODO: SLERP
    }

    private void goToPosition(float dTime)
    {
        Vector3 newPos = new Vector3();

        switch (state)
        {
            case types.relative: // go behind player
                newPos = target.position;
                newPos += target.forward * offset.z;
                newPos += target.right * offset.x;
                newPos += target.up * offset.y;
                /*
                //something is between the camera an the player, dont allow it
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(transform.position, transform.position - target.position, out hit, (target.position - transform.position).magnitude))
                {
                    if(hit.transform.gameObject != target.gameObject)
                    {
                        newPos = hit.transform.position;
                        newPos += (target.position - transform.position).normalized * 0.2f;
                    }
                }
                else
                if (Physics.Raycast(transform.position, target.position - transform.position, out hit, 0.2f))
                {
                    newPos += (target.position - transform.position).normalized * 0.2f;
                }
                */

                break;

            case types.abolute: //stay at a fixed position next to the player
                newPos = target.position + offset;
                break;
            case types.path: //follow a premade path (-> cinematic)
                newPos = getPointAlongPath();
                break;
        }

        transform.position = Vector3.Lerp(transform.position, newPos, moveSpeed * dTime);
    }

    Vector3 getPointAlongPath()
    {
        Vector3 point = new Vector3();

        //get two nearest points
        int first = 0;
        int second = 0;
        float gesDistance = float.MaxValue;

        Vector3 firstP = new Vector3();
        Vector3 secondP = new Vector3();


        //cf. get min value ot of array
        for (int i = 0; i < path.Length()-2; i++)//zwischen je zwei punkten
        {
            firstP = path.positions[i];
            secondP = path.positions[i + 1];
            Vector3 middle = new Vector3();

            float distance = float.MaxValue;
            middle = new Vector3();

            float between = Vector3.Distance(firstP, secondP);
            for (float j = 0f; j < 1; j += 1/ (pointSteps * between))//interpolate
            {
                Vector3 temp = Vector3.Lerp(firstP, secondP, j);
                float dis = Vector3.Distance(target.position, temp);

                if(dis < distance)
                {
                    distance = dis;
                    middle = temp;
                }
            }

            if (distance < gesDistance) // is the current point closer to what i want than the current best
            {
                //replace best
                first = i;
                second = i + 1;
                point = middle;
                gesDistance = distance;
            }
        }

        //Debug.DrawLine(target.position, point);

        return point;
    }

    private void OnDrawGizmos()
    {
        if(path != null)
        {
            if (state == types.relative)
            {
                Gizmos.color = Color.black;
                for (int i = 0; i < path.Length() - 1; i++)
                {
                    Gizmos.DrawSphere(path.positions[i], 0.1f);
                    Gizmos.DrawLine(path.positions[i], path.positions[i + 1]);
                }
                Gizmos.DrawSphere(path.positions[path.Length() - 1], 0.1f);
            }
        }
        
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, target.position);
    }
}
