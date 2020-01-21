
 //written von Arte
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LevelFadeIn : MonoBehaviour
{
    private Vector3 Endpunkt;
    [Range(0,1)] public float Speed; //zwischen 0 und 1

    public float randomSpeed;
    public float randomOffset;
    public float offset; 

    public float distanz = 30;

    void Start()
    {   
        offset += Random.Range(-randomOffset, randomOffset);
        Speed += Random.Range(-randomSpeed, randomSpeed);

        Endpunkt = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y - offset, transform.position.z);
    
    }

    void Update()
    {
        GameObject player = GameObject.FindObjectOfType<PlayerControllerRefactored> ().gameObject;
        float entfernung = new Vector3(transform.position.x - player.transform.position.x, 0, transform.position.z - player.transform.position.z).magnitude;
        if (entfernung <= distanz ){
            transform.position = Vector3.Lerp(transform.position, Endpunkt, Speed * Time.deltaTime * Time.timeScale);
        if (transform.position == Endpunkt){

            Destroy(this);

        }

        }
        //berechnen position von dem Player object
        //Objekt - Player.position
        //y = 0
        //.magnitude
        //if distanz < 5






    }






}
