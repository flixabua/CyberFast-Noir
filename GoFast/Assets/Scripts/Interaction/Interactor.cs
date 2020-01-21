/*
 * written by Jonas Hack
 * 
 * suppements the player controller 
 * handles interaction
 * 
 * by looking for the best interactable around
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform myDirection;
    [SerializeField] private float maxAngle = 30f;
    [SerializeField] private float maxDistance = 2f;
    private List<Interactable> passives = new List<Interactable>();

    public string button = "Interact";

    private void Update()
    {
        if (Input.GetButtonDown(button))
        {
            interact();
        }
    }

    public void register(Interactable passive)
    {
        passives.Add(passive);
        Debug.Log(passive.name + " registered with " + this.name);
    }

    public void interact()//FIXME: angle and distance do NOT get determined correctly!
    {
        /*
        Interactable closest = null;
        float angle = float.MaxValue;
        float dis = float.MaxValue;
        float combined = float.MaxValue;

        foreach (Interactable passive in passives)
        {
            if(passive != null)
            {
                if(passive.isActiveAndEnabled)//ignore disabled stuff
                {
                    Vector3 distance = myDirection.position + passive.transform.position;
                    float between = Vector3.Angle(distance, (transform.position + myDirection.forward));
                    float comb = distance.magnitude + between;
                    if(comb < combined)
                    {
                        angle = between;
                        dis = distance.magnitude;
                        closest = passive;
                    }
                }
            }
            else
            {
                passives.Remove(passive);//clean up
            }
        }
        if(angle <= maxAngle && dis <= maxDistance)
        {
            closest.trigger(gameObject);
            Debug.DrawLine(myDirection.position, closest.transform.position);
        }
        else
        {
            Debug.Log(this.name + " could not find anything to interact with. a: " + angle + ", d: " + dis);
        }        
    }
    */
    //crude fix by just doing something easier (triggering every possible canidate)
        foreach (Interactable passive in passives)
        {
            float dis = Vector3.Distance(transform.position, passive.transform.position);
            float angle = Vector3.Angle(transform.forward, passive.transform.position - transform.position);

            if(angle < maxAngle && dis < maxDistance)
            {
                passive.trigger(gameObject);
                Debug.DrawLine(transform.position, passive.transform.position);
            }
            else
            {
                Debug.Log("did not trigger " + passive.name + "d: " + dis + ", a: " + angle);
            }
        }
    }
}
