/*
 * Written by Jonas Hack
 * 
 * shoots an object at a target, while accounting for speed
 * 
 * 
 * uses interceptor and clamplookat
 * 
 * 
 * also ugly and hard to read code. oof
 * 
 * //TODO: comments ffs
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(clampLookAt))]
public class Turret : MonoBehaviour
{
    public GameObject target;
    Rigidbody targetRigid;

    [SerializeField] GameObject muzzle;

    [SerializeField] GameObject projectile;
    //[SerializeField] GameObject muzzleFlash;

    
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float range = 20f;

    [SerializeField] float fixedShootFrequency = 5f;
    [SerializeField] float randomShootFrequency = 1f;
    float timeSinceLastShot = 0;//actually counts down, not up

    [SerializeField] float fixedShootDelay = 0.5f;
    [SerializeField] float randomShootDelay = 0.2f;
    float timeSinceSpotted = 0f;//actually counts down, not up

   // [SerializeField] int shootThroughLayer = 9;//pass through layer

    clampLookAt looker;

    // Start is called before the first frame update
    void Start()
    {
        looker = GetComponent<clampLookAt>();

        if (target == null) Debug.LogWarning(name + " was not assigned a target, please do so in inspector");
        target = GameObject.FindObjectOfType<PlayerControllerRefactored>().gameObject;
        if (target == null)
        {
            Debug.LogWarning(name + " could not find a target and destoryed itself, please do so in inspector");
            Destroy(gameObject);
        }

        targetRigid = target.GetComponent<Rigidbody>();

        if (projectile == null) Debug.LogError(name + " was not assigned a projectile. please do so in inspector");
        if (muzzle == null)
        {
            Debug.LogWarning(name + " was not assigned a muzzle. please do so in inspector");
            if (transform.childCount > 0) muzzle = transform.GetChild(0).gameObject;
            else Debug.LogError(name + " could not find a muzzle. make sure it has children and assign them in inspector");
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, range)
            && hit.collider.gameObject == target)
        {
            looker.target = target.transform;
            timeSinceSpotted -= Time.deltaTime * Time.timeScale;
        }
        else
        {
            timeSinceSpotted = fixedShootDelay + Random.Range(-randomShootDelay, randomShootDelay);
            
            looker.target = null;
        }

        if (timeSinceLastShot <= 0 && timeSinceSpotted <= 0)
        {
            shoot();
        }
        timeSinceLastShot -= Time.deltaTime * Time.timeScale;
    }

    void shoot()
    {
        //Debug.Log(timeSinceLastShot + ", " + timeSinceSpotted+ ", " + Vector3.Distance(transform.position, target.transform.position));

        timeSinceLastShot = fixedShootFrequency + Random.Range(-randomShootFrequency, randomShootFrequency);

        Vector3 projectileVelocity = new Vector3();

        if (targetRigid != null)
        {
            projectileVelocity = Intersection.IntersectionVelocity(target.transform.position, targetRigid.velocity, muzzle.transform.position, projectileSpeed, 10, 0.5f, 0.1f);
        }
        else
        {
            projectileVelocity = (target.transform.position - muzzle.transform.position).normalized * projectileSpeed;
        }

        if (projectileVelocity.magnitude > 0)//if the shot is impossible intersection returns 0
        {
            looker.target = target.transform;

            GameObject proj = Instantiate(projectile, muzzle.transform.position, Quaternion.LookRotation(projectileVelocity));
            Rigidbody projRigid = proj.GetComponent<Rigidbody>();
            if (projRigid == null)
            {
               proj.AddComponent<Rigidbody>().velocity = projectileVelocity;
                projRigid.useGravity = false;
            }
            else projRigid.velocity = projectileVelocity;
        }
        else looker.target = null;
    }
        
}
