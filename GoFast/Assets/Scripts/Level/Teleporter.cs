using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform teleporter;


    void OnTriggerEnter(Collider other){

        other.gameObject.transform.position = teleporter.transform.position;
    }

}
