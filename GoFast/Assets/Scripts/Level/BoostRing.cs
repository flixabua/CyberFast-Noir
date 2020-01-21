/*
 * written by Jonas Hack
 * 
 * shoots any rigidbody inside of it along its forward axis
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoostRing : MonoBehaviour
{
    [SerializeField] private float force = 10000f;
    [SerializeField] private float additionalVerticalForce = 300f;

    [SerializeField] private float fovForceMultiplier = 1;
    [SerializeField] private float fovTimeMultiplier = 1;
    //maybe add min entry velocity or set speed instead of add force

    private void OnTriggerEnter(Collider other)
    {   

        Rigidbody rigid = other.GetComponent<Rigidbody>();

        if (rigid != null)
        {
            Vector3 direction = Vector3.Project(rigid.velocity, transform.forward).normalized;//go forwars or backwards?
            if (direction.magnitude == 0) direction = transform.forward;
            rigid.AddForce(direction * force + Vector3.up *  additionalVerticalForce);

            PlayerControllerRefactored player = other.GetComponent<PlayerControllerRefactored>();
            if (player != null) CameraEffectsMaster.fovPunch((force + additionalVerticalForce) / 2000 * fovForceMultiplier, (force + additionalVerticalForce) / 20000 * fovTimeMultiplier);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position - transform.forward*transform.localScale.z, transform.position + transform.forward * transform.localScale.z);
    }
}
