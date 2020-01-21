/*
 * written by Jonas Hack
 * 
 * first draft of player controller
 * only here for reference
 * will not be used in build
 * 
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Drag is to be understood of % of current velocity is maintained
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float accelarationForce = 60f;
    [Range(0,1)] [SerializeField] private float aerialMultiplier = 0.2f;
    [SerializeField] private float jumpForce = 400f;
    [SerializeField] private float jumpCooldown = 0.1f;
    private float timeSinceLastJump = 0;
    [Range(0, 1)] [SerializeField] private float myDrag = 0.8f;
    private bool tryingToJump = false;

    [SerializeField] private float wallRunDetectionDistance = 0.8f;
    [SerializeField] private float wallMinVelocity = 2f;
    [Range(0, 1)] [SerializeField] private float wallRunVertivalDrag = 0.4f;
    [SerializeField] private float wallRunWallAttraction = 5f;
    [Range(0, 1)] [SerializeField] private float wallMultiplier = 0.4f;
    [SerializeField] private float wallRunCameraTilt = 30f;
    [SerializeField] private float wallRunCameraTiltSpeed = 0.5f;
    [SerializeField] private float wallRunJumpNormalForce = 900f;
    [SerializeField] private float wallRunJumpForwardForce = 400f;
    [SerializeField] private float wallRunJumpVerticalForce = 300f;
    private bool leftWallRun = false;
    private bool rightWallRun = false;


    [SerializeField] private float wallTurnDetectionDistance = 0.8f;
    [SerializeField] private float wallTurnVerticalForce = 400f;
    [SerializeField] private float wallTurnNormalForce = 300f;
    [SerializeField] private float wallTurnMinVerticalSpeed = 2f;
    [SerializeField] private float turnSpeed = 100f;
    private bool infrontOfThing = false;
    private bool infrontOfThingLow = false;
    private bool infrontOfThingHigh = false;

    [SerializeField] private float pullUpVercticalForce = 500;
    [SerializeField] private float pullUpForwardForce = 200;

    //[SerializeField] private float skipVercticalForce = 500;
    //[SerializeField] private float skipForwardForce = 200;

    bool tryingToSlide = false;

    [SerializeField] private float mouseSpeed = 1f;

    private Rigidbody rigid;
    private bool isGrounded = true;
    [SerializeField] private float groundedMargin = 0.15f;
    private float timeSinceGrounded = 0f;

    private new Camera camera;
    private float horiz;
    private float vert;
    [SerializeField] private bool useController = false;

    Vector3 resetPos;
    
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        camera = transform.GetChild(0).gameObject.GetComponent<Camera>();//camera must be very first child
        //TODO: edge case

        resetPos = transform.position;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(new Ray(transform.position, Vector3.down), transform.localScale.y + 0.2f);
        if (isGrounded) timeSinceGrounded = 0;
        else timeSinceGrounded += Time.deltaTime;

        timeSinceLastJump += Time.deltaTime;
        if (Input.GetButtonDown("Jump") && timeSinceLastJump >= jumpCooldown)
        {
            tryingToJump = true;
            timeSinceLastJump = 0f;
        }
        if (Input.GetButtonDown("Duck") && timeSinceLastJump >= jumpCooldown)
        {
            tryingToSlide = true;
            timeSinceLastJump = 0f;
            //TODO: make destinct cooldown
        }

        //might want to do that with triggers instead
        rightWallRun = Physics.Raycast(new Ray(transform.position, transform.right), wallRunDetectionDistance);
        leftWallRun = Physics.Raycast(new Ray(transform.position, -transform.right), wallRunDetectionDistance);

        infrontOfThing = Physics.Raycast(new Ray(transform.position, transform.forward), wallTurnDetectionDistance);
        infrontOfThingLow = Physics.Raycast(new Ray(transform.position + new Vector3(0,-transform.localScale.y,0), transform.forward), wallTurnDetectionDistance);
        infrontOfThingHigh = Physics.Raycast(new Ray(transform.position + new Vector3(0, +transform.localScale.y, 0), transform.forward), wallTurnDetectionDistance);

        //movement
         horiz = Input.GetAxisRaw("Horizontal");
         vert = Input.GetAxisRaw("Vertical");

        //rotate according to mouse movement
        float deltaXRot;
        float deltaYRot;
        if(!useController)
        {
            deltaXRot = Input.GetAxisRaw("Mouse X") * mouseSpeed;
            deltaYRot = -Input.GetAxisRaw("Mouse Y") * mouseSpeed;
        }
        else
        {
            deltaXRot = Input.GetAxisRaw("Horizontal2") * mouseSpeed;
            deltaYRot = Input.GetAxisRaw("Vertical2") * mouseSpeed;
        }

        transform.Rotate(new Vector3(0, deltaXRot, 0));

        //dont break your neck
        float angle = camera.transform.rotation.eulerAngles.x + deltaYRot;
        if (camera.transform.rotation.eulerAngles.x + deltaYRot > 90) angle -= 360;
        if (angle > -89 && angle < 89)
        {
            camera.transform.Rotate(new Vector3(deltaYRot, 0, 0));
        }


        //Reset
        if (Input.GetButtonDown("Cancel"))
        {
            transform.position = resetPos;
            rigid.velocity = new Vector3();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //movement 
        Vector3 force = transform.forward * vert + transform.right * horiz;
        force = force.normalized * accelarationForce * aerialMultiplier;

        Vector3 forceForJump = new Vector3();

        //TODO: time since last jump -> cooldown
        if (timeSinceGrounded < groundedMargin)//coyote time
        {
            force /= aerialMultiplier;
            
            //jump
            //TODO: proper falltime
            if (tryingToJump)
            {
                forceForJump = (Vector3.up * jumpForce);
            }

            //apply drag only when not in the air
            if (force.magnitude == 0)
            {
                Vector2 velocity = new Vector2(rigid.velocity.x, rigid.velocity.z);//ignore vertical aspect
                velocity *= myDrag;

                rigid.velocity = new Vector3(velocity.x, rigid.velocity.y, velocity.y);
            }
        }

        Vector3 cameraRotation = new Vector3();

        if ((rightWallRun || leftWallRun) && !isGrounded && rigid.velocity.magnitude >= wallMinVelocity)//am i wallrunning?
        {

            force /= aerialMultiplier;
            force *= wallMultiplier;

            //stay attached to the wall
            rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y * wallRunVertivalDrag, rigid.velocity.z);
            if(rightWallRun)//destinguish bewteen left and right
            {
                rigid.AddForce(transform.right * wallRunWallAttraction);

                //jump off the wall
                if (tryingToJump)
                {
                    forceForJump = -transform.right * wallRunJumpNormalForce + transform.forward * wallRunJumpForwardForce + Vector3.up * wallRunJumpVerticalForce;
                }

                cameraRotation.z = wallRunCameraTilt;
            }
            else
            {
                rigid.AddForce(-transform.right * wallRunWallAttraction);

                //jump off the wall
                if (tryingToJump)
                {
                    forceForJump = transform.right * wallRunJumpNormalForce + transform.forward * wallRunJumpForwardForce + Vector3.up * wallRunJumpVerticalForce;
                }
               
                cameraRotation.z = -wallRunCameraTilt;
            }
        }
        else
        {
            cameraRotation.z = 0;
        }

        //walljump
        //TODO: am i moving towards it?
        bool entireInforntOfWall = infrontOfThing && infrontOfThingHigh && infrontOfThingLow;
        bool canWallJump = (rightWallRun || leftWallRun) && !isGrounded;
        if (tryingToJump && !isGrounded && !canWallJump && entireInforntOfWall && (rigid.velocity.y >= wallTurnMinVerticalSpeed))
        {
            forceForJump += Vector3.up * wallTurnVerticalForce + -transform.forward * wallTurnNormalForce;
            //transform.Rotate(0, 180, 0);
            StartCoroutine(turnAround());
        }
        
        //pullup
        if(tryingToJump && (infrontOfThing  && !infrontOfThingHigh) && !canWallJump)
        {
            forceForJump += Vector3.up * pullUpVercticalForce + transform.forward * pullUpForwardForce;
            //Debug.Log("pullup");
            //TODO: make this a pullup instead of a boost
        }
        /*
        //obstacle skip
        if (tryingToJump && (infrontOfThingLow && !infrontOfThing && !infrontOfThingHigh) && !canWallJump && isGrounded)
        {
            forceForJump += Vector3.up * skipVercticalForce + transform.forward * skipForwardForce;
            //Debug.Log("skip");
            //TODO: make this ik instead of a boost
        }
        //slide
        if(isGrounded && tryingToSlide && !infrontOfThing && !infrontOfThingLow && infrontOfThingHigh)
        {
            StartCoroutine(slide());
        }*/

        //apply camera roll
        cameraRotation.x = camera.transform.rotation.eulerAngles.x;
        cameraRotation.y = camera.transform.rotation.eulerAngles.y;
        camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, Quaternion.Euler(cameraRotation), wallRunCameraTiltSpeed);

        //apply movement
        rigid.AddForce(force);
        rigid.AddForce(forceForJump);

        
        //enforce max velocity (ignore vertical)
        Vector2 planeVelocity = new Vector2(rigid.velocity.x, rigid.velocity.z);
        if (planeVelocity.magnitude > maxSpeed)
        {
            planeVelocity = planeVelocity.normalized * maxSpeed;
            rigid.velocity = new Vector3(planeVelocity.x, rigid.velocity.y, planeVelocity.y);
        }


        tryingToJump = false;//checking in update for better responsiveness
        tryingToSlide = false;
    }

    IEnumerator turnAround()
    {
        float turned = 0;
        while(turned < 180)
        {
            float toTurn = turnSpeed * Time.deltaTime;
            if (turned + toTurn > 180)//dont turn to far
            {
                toTurn = 180 - turned;
            }
            transform.Rotate(new Vector3(0, toTurn, 0));
            turned += toTurn;
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log("Eded with " + turned + " deg");
        
    }

    IEnumerator slide()
    {
       CapsuleCollider  col = GetComponent<CapsuleCollider>();
       float colStartSize = col.height;

        float standartDrag = myDrag;
        float standartControll = accelarationForce;

        myDrag = 1.2f;
        accelarationForce = 10;

        while(col.height > colStartSize*0.3f)
        {
            col.height -= 2f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        float waitTime = 1f;
        while(waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while (col.height < colStartSize)
        {
            if(Physics.Raycast(new Ray(transform.position, Vector3.up),transform.localScale.y + 0.1f))
            col.height += 2f * Time.deltaTime;
            if (col.height > colStartSize) col.height = colStartSize;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("end slide");
        myDrag = standartDrag;
        accelarationForce = standartControll;

        yield return null;
    }

    private void OnDrawGizmos()
    {

        if (isGrounded) Gizmos.color = Color.green; else Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (transform.localScale.y + 0.2f));

        if (rightWallRun) Gizmos.color = Color.green; else Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * wallRunDetectionDistance);


        if (leftWallRun) Gizmos.color = Color.green; else Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + -transform.right * wallRunDetectionDistance);

        if (infrontOfThing) Gizmos.color = Color.green; else Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * wallTurnDetectionDistance);

        if (infrontOfThingLow) Gizmos.color = Color.green; else Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(0, -transform.localScale.y, 0), transform.position + new Vector3(0, -transform.localScale.y, 0) + transform.forward * wallTurnDetectionDistance);

        if (infrontOfThingHigh) Gizmos.color = Color.green; else Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(0, +transform.localScale.y, 0), transform.position + new Vector3(0, +transform.localScale.y, 0) + transform.forward * wallTurnDetectionDistance);
    }
}
