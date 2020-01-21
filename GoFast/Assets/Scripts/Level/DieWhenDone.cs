/*
 * written by jonas hack
 * 
 * destroys temporaray objects for better performance
 * for when you dont boter with object pooling ;P
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieWhenDone : MonoBehaviour
{
    enum type { time, particle, speed, speedAndTime, sight, speedAndTimeAndSight};
    [SerializeField] type myType = type.time;

    [SerializeField] float speedMargin = 0.2f;
    [SerializeField] float timeMargin = 0.2f;

    Rigidbody rigid = null;
    ParticleSystem particle = null;
    
    // Start is called before the first frame update
    void Start()
    {
        if (myType == type.speed || myType == type.speedAndTime || myType == type.speedAndTimeAndSight)//at this point is an enum even sensible?
        {
            rigid = GetComponent<Rigidbody>();
            if (rigid == null)
            {
                Debug.LogWarning(name + " was set to " + myType.ToString() + ", but didnt have a rigidbody");
                Destroy(gameObject);
            }
        }


        if (myType == type.particle)
        {
            particle = GetComponent<ParticleSystem>();
            if (particle == null)
            {
                Debug.LogWarning(name + " was set to " + myType.ToString() + ", but didnt have a particle");
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(myType)
        {
            case type.time:
                if (timeMargin <= 0) Destroy(gameObject);
                break;
            case type.particle:
                if (particle.isStopped) Destroy(gameObject);
                break;
            case type.speed:
                if (rigid.velocity.magnitude <= speedMargin) Destroy(gameObject);
                break;
            case type.speedAndTime:
                if (timeMargin <= 0 || rigid.velocity.magnitude <= speedMargin) Destroy(gameObject);
                break;
            case type.speedAndTimeAndSight:
                if (timeMargin <= 0 || rigid.velocity.magnitude <= speedMargin) Destroy(gameObject);
                break;
            default:
                Debug.LogWarning(name + ", had an impossible type and destroyed itself");
                Destroy(gameObject);
                break;
        }

        timeMargin -= Time.deltaTime * Time.timeScale;
    }

    private void OnBecameInvisible()
    {
        if (myType == type.sight || myType == type.speedAndTimeAndSight) Destroy(gameObject); 
    }
}
