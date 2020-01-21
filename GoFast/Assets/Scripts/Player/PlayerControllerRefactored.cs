/*
 * Written by Jonas Hack
 * 
 * Better Version of "PlayerController".
 * Is what drives Player movement and genereal player functions, NOT including interaction
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Interactor))]
[RequireComponent(typeof(DSwitcher))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CameraEffectsMaster))]
public class PlayerControllerRefactored : myReset
{
    public enum state { walking, jumping, falling, wallRunning, wallJumping, wallTurning, pullUp, sliding, skipping };
    state currentState = state.walking;

    //Input
    private Camera camera;
    /*[SerializeField]*/ public float mouseSpeed = 1f;//public so pause menu can access
    private float deltaXRot;
    private float deltaYRot;
    private float horiz;
    private float vert;
    /*[SerializeField]*/ private bool useController = false;

    private string moveAxisHoriz = "Horizontal";
    private string moveAxisVerti = "Vertical";

    private string lookAxisHoriz = "Mouse X";
    private string lookAxisVerti = "Mouse Y";
    //private float mouseSpeed = 1f;

    private bool useRawInput = true;
    private bool invertXAxis = false;
    private bool invertYAxis = false;

    private string controllerAxisHoriz = "Horizontal2";
    private string controllerAxisVerti = "Mouse X";
    //private bool useController = false;

    private string jump = "Jump";

    private string reset = "backspace";
    private string quit = "return";

    private string interact = "Interact";
    private string switchButton = "Switch";

    private Interactor interactor;
    private DSwitcher switcher;


    //general variables
    /*[SerializeField]*/
    private float maxSpeed = 100f;
    private float maxAccelarationSpeed = 10f;
    /*[SerializeField]*/
    private float accelarationForce = 60f;
    [Range(0, 1)] /*[SerializeField]*/ private float myDrag = 0.8f;
    private Rigidbody rigid;

    private bool infrontOfThing = false;
    private bool infrontOfThingLow = false;
    private bool infrontOfThingHigh = false;

    //jump
    /*[SerializeField]*/
    private float jumpForce = 400f;
    [Range(0, 1)] /*[SerializeField]*/ private float aerialMultiplier = 0.15f;
    /*[SerializeField]*/
    private float jumpCooldown = 0.15f;

    private float timeSinceLastJump = 0;
    private bool tryingToJump = false;

    //CoyoteTime
    private bool isGrounded = true;
    /*[SerializeField]*/
    private float groundedMargin = 0.15f;
    private float timeSinceGrounded = 0f;

    //wallRun
    [Range(0, 1)] /*[SerializeField]*/ private float wallRunVertivalDrag = 0.82f;
    [Range(0, 1)] /*[SerializeField]*/ private float wallMultiplier = 0.6f;

    /*[SerializeField]*/
    private float wallRunCameraTilt = 20f;
    /*[SerializeField]*/
    private float wallRunCameraTiltSpeed = 0.15f;

    /*[SerializeField]*/
    private float wallRunWallAttraction = 5f;

    /*[SerializeField]*/
    private float wallRunDetectionDistance = 1.55f;
    /*[SerializeField]*/
    private float wallMinVelocity = 4f;
    ///*[SerializeField]*/ private float wallRunMaxAngle = 60f; // redundant because raycast
    private bool leftWallRun = false;
    private bool rightWallRun = false;
    RaycastHit rightHit = new RaycastHit();
    RaycastHit leftHit = new RaycastHit();

    //wallJump
    /*[SerializeField]*/
    private float wallRunJumpNormalForce = 500f;
    /*[SerializeField]*/
    private float wallRunJumpTangentForce = 200f;
    /*[SerializeField]*/
    private float wallRunJumpForwardForce = 600f;
    /*[SerializeField]*/
    private float wallRunJumpVerticalForce = 400f;

    //wallTurn
    /*[SerializeField]*/
    private float wallTurnDetectionDistance = 2f;
    /*[SerializeField]*/
    private float wallTurnVerticalForce = 500f;
    /*[SerializeField]*/
    private float wallTurnNormalForce = 300f;
    /*[SerializeField]*/
    private float wallTurnMinVerticalSpeed = -5f;
    /*[SerializeField]*/
    private float turnSpeed = 500f;


    //TODO: implement a path follow system for dynamic animations instead of the following boosts
    //pullUp
    /*[SerializeField]*/
    private float pullUpVercticalForce = 500;
    /*[SerializeField]*/
    private float pullUpForwardForce = 50;
    /*
    //skip
    /*[SerializeField]*/
    //private float skipVercticalForce = 300;
    /*[SerializeField]*/
    //private float skipForwardForce = 500;

    //slide
    //bool tryingToSlide = false;

    public PlayerData playerData;
    public InputData inputData;

    //Animation
    private Animator animator = new Animator();

    //Check refernces and set initial values
    void Start()
    {
        setValues();//reset

        rigid = GetComponent<Rigidbody>();
        //camera = transform.GetChild(0).gameObject.GetComponent<Camera>();//camera must be very first child
        camera = GetComponentInChildren<Camera>();

        animator = GetComponent<Animator>();
        animator.enabled = true;

        interactor = GetComponent<Interactor>();
        switcher = GetComponent<DSwitcher>();

        if (playerData == null) Debug.LogWarning("assign playerdata to player in inspetor please");
        else StartCoroutine(loadPlayerData());

        if (inputData == null) Debug.LogWarning("assign inputdata to player in inspetor please");
        else StartCoroutine(loadInputData());
    }

    void OnDeath()
    {
        myReset.ResetAll();
        //TODO: reset enemies
    }

    //We dont want to use that "arcade" stuff
    /*void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            Collider col = other.gameObject.GetComponent<Collider>();
            Collider mycol = this.gameObject.GetComponent<Collider>();
            if (enemy.invincible)
            {
                OnDeath();
            }
            else if(mycol.bounds.center.y - mycol.bounds.extents.y > col.bounds.center.y + 0.5f * col.bounds.extents.y)
            {
                GameData.Instance.Score += 10;
                JumpedOnEnemy(enemy.bumpSpeed);
                enemy.OnDeath();
            }
            else
            {
                OnDeath();
            }
        }
    }

    void JumpedOnEnemy(float bumpSpeed)
    {
        rigid.velocity = new Vector3(rigid.velocity.x, bumpSpeed, rigid.velocity.z);
    }*/

    
    void Update()
    {
        getInput();

        //Raycasts, so we know our soroundings (ignoring pass through layer)
        rightWallRun = Physics.Raycast(new Ray(transform.position, transform.right), out rightHit, wallRunDetectionDistance, 9);
        leftWallRun = Physics.Raycast(new Ray(transform.position, -transform.right), out leftHit, wallRunDetectionDistance, 9);

        infrontOfThing = Physics.Raycast(new Ray(transform.position, transform.forward), wallTurnDetectionDistance, 9);
        infrontOfThingLow = Physics.Raycast(new Ray(transform.position + new Vector3(0, -transform.localScale.y, 0), transform.forward), wallTurnDetectionDistance, 9);
        infrontOfThingHigh = Physics.Raycast(new Ray(transform.position + new Vector3(0, +transform.localScale.y, 0), transform.forward), wallTurnDetectionDistance, 9);

    }

   
    void FixedUpdate()//this is where the magic happens
    {
        //Debug.Log(currentState.ToString());

        //basic values well need later
        Vector3 movementForce = new Vector3();
        if(rigid.velocity.magnitude <= maxAccelarationSpeed)
            movementForce= (transform.forward * vert + transform.right * horiz).normalized * accelarationForce;
        Vector3 forceForJump = new Vector3();

        Vector3 cameraRotation = camera.transform.rotation.eulerAngles;

        bool entireInforntOfWall = infrontOfThing && infrontOfThingHigh && infrontOfThingLow;
        Vector3 horizVelocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
        bool canWallRun = (rightWallRun || leftWallRun) && !isGrounded && horizVelocity.magnitude >= wallMinVelocity;

        switch (currentState)//single state machine including all actions and transitions
        {
            case state.walking:
                if (timeSinceGrounded < groundedMargin && tryingToJump)//coyteTime (instead of isGrounded)
                {
                    currentState = state.jumping;
                    forceForJump = (Vector3.up * jumpForce);
                }
                if (!isGrounded)
                {
                    currentState = state.falling;
                }
                break;

            case state.jumping:
                if (rigid.velocity.y < 0)
                {
                    currentState = state.falling;
                }
                /* else if(isGrounded)
                 {
                     currentState = state.walking;
                 }*/
                else if (canWallRun)//wallRun
                {
                    currentState = state.wallRunning;
                }
                else if (tryingToJump && !isGrounded && !canWallRun && entireInforntOfWall && (rigid.velocity.y >= wallTurnMinVerticalSpeed))
                {
                    currentState = state.wallTurning;
                    forceForJump += Vector3.up * wallTurnVerticalForce + -transform.forward * wallTurnNormalForce;
                    StartCoroutine(turnAround());
                }
                else if (tryingToJump && (infrontOfThing && !infrontOfThingHigh) && !canWallRun)
                {
                    currentState = state.pullUp;
                    forceForJump += Vector3.up * pullUpVercticalForce + transform.forward * pullUpForwardForce;
                    //TODO: make this a pullup instead of a boost
                }

                //changeControll
                movementForce *= aerialMultiplier;
                break;

            case state.falling:
                if (isGrounded)
                {
                    currentState = state.walking;
                }
                else if (canWallRun)//wallRun
                {
                    currentState = state.wallRunning;
                }
                else if (tryingToJump && !isGrounded && !canWallRun && entireInforntOfWall && (rigid.velocity.y >= wallTurnMinVerticalSpeed))
                {
                    currentState = state.wallTurning;
                    forceForJump += Vector3.up * wallTurnVerticalForce + -transform.forward * wallTurnNormalForce;
                    StartCoroutine(turnAround());
                }
                else if (tryingToJump && (infrontOfThing && !infrontOfThingHigh) && !canWallRun)
                {
                    currentState = state.pullUp;
                    forceForJump += Vector3.up * pullUpVercticalForce + transform.forward * pullUpForwardForce;
                    //TODO: make this a pullup instead of a boost
                }

                //changeControll
                movementForce *= aerialMultiplier;
                break;

            case state.wallRunning:
                if (!canWallRun) currentState = state.falling;
                else if (isGrounded)
                {
                    currentState = state.walking;
                }

                //changeControll
                movementForce *= wallMultiplier;

                //apply vertical drag
                rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y * wallRunVertivalDrag, rigid.velocity.z);

                if (rightWallRun || leftWallRun)
                {
                    //destinguish bewteen left and right
                    Vector3 rightDirection = Vector3.positiveInfinity;
                    if (rightWallRun) rightDirection = (rightHit.collider.ClosestPoint(transform.position) - transform.position).normalized;
                    Vector3 leftDirection = Vector3.positiveInfinity;
                    if (leftWallRun) leftDirection = (leftHit.collider.ClosestPoint(transform.position) - transform.position).normalized;

                    Vector3 tangent = new Vector3();
                    //TODO: make the tangent actually do stuff properly


                    animator.SetBool("isRightRun", rightDirection.magnitude < leftDirection.magnitude);
                    if (rightDirection.magnitude < leftDirection.magnitude)
                    {
                        tangent = Vector3.ProjectOnPlane(horizVelocity, rightHit.normal).normalized;


                        rigid.AddForce(rightDirection * wallRunWallAttraction);//stay attached to the wall

                        //jump off the wall
                        if (tryingToJump)
                        {
                            Debug.DrawLine(transform.position, transform.position + tangent, Color.white, 5f);

                            currentState = state.wallJumping;
                            forceForJump = -rightDirection * wallRunJumpNormalForce + transform.forward * wallRunJumpForwardForce + Vector3.up * wallRunJumpVerticalForce + tangent * wallRunJumpTangentForce;
                        }

                        cameraRotation.z = wallRunCameraTilt;
                    }
                    else
                    {
                        tangent = Vector3.ProjectOnPlane(rigid.velocity, leftHit.normal).normalized;

                        rigid.AddForce(leftDirection * wallRunWallAttraction);//stay attached to the wall

                        //jump off the wall
                        if (tryingToJump)
                        {
                            Debug.DrawLine(transform.position, transform.position + tangent, Color.white, 5f);

                            currentState = state.wallJumping;
                            forceForJump = -leftDirection * wallRunJumpNormalForce + transform.forward * wallRunJumpForwardForce + Vector3.up * wallRunJumpVerticalForce + tangent * wallRunJumpTangentForce;
                        }

                        cameraRotation.z = -wallRunCameraTilt;
                    }

                    //rigid.AddForce(tangent.normalized * accelarationForce*10);
                    //transform.rotation.SetLookRotation(Vector3.Lerp(transform.forward, tangent.normalized, 1), Vector3.up);
                }

                break;

            case state.wallJumping:
                if (rigid.velocity.y < 0)
                {
                    currentState = state.falling;
                }
                if (isGrounded)
                {
                    currentState = state.walking;
                }

                //changeControll
                movementForce *= aerialMultiplier;
                break;

            case state.wallTurning:
                //the actual turn is done in a coroutine
                if (rigid.velocity.y < 0)
                {
                    currentState = state.falling;
                }
                if (isGrounded)
                {
                    currentState = state.walking;
                }
                if (canWallRun)//wallRun
                {
                    currentState = state.wallRunning;
                }
                //can do stuff again after turn is over
                break;

            case state.pullUp://currently not woring as intended
                if (rigid.velocity.y < 0)
                {
                    currentState = state.falling;
                }
                if (isGrounded)
                {
                    currentState = state.walking;
                }
                if (canWallRun)//wallRun
                {
                    currentState = state.wallRunning;
                }
                break;

            case state.sliding:
                //TODO: redo or copy
                break;

            case state.skipping:
                //TODO: redo or copy
                break;

            default:
                break;
        }

        //applyDrag when not accelartaing
        if (movementForce.magnitude == 0 && currentState != state.wallRunning && isGrounded)
        {
            Vector2 velocity = new Vector2(rigid.velocity.x, rigid.velocity.z);//ignore vertical aspect
            velocity *= myDrag;

            rigid.velocity = new Vector3(velocity.x, rigid.velocity.y, velocity.y);
        }

        //apply camera roll
        if (currentState != state.wallRunning) cameraRotation.z = 0;
        camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, Quaternion.Euler(cameraRotation), wallRunCameraTiltSpeed);

        //apply movement
        rigid.AddForce(movementForce);
        rigid.AddForce(forceForJump);

        //enforce max velocity (ignore vertical)
        Vector2 planeVelocity = new Vector2(rigid.velocity.x, rigid.velocity.z);
        if (planeVelocity.magnitude > maxSpeed)
        {
            planeVelocity = planeVelocity.normalized * maxSpeed;
            rigid.velocity = new Vector3(planeVelocity.x, rigid.velocity.y, planeVelocity.y);
        }

        //do animatons
        animator.SetInteger("state", (int)currentState);
        //isRightRun is done above
        animator.SetFloat("speed", rigid.velocity.magnitude / maxAccelarationSpeed);
        animator.SetFloat("side", Vector3.Project(rigid.velocity, transform.right).magnitude / maxAccelarationSpeed);
        animator.SetFloat("front", Vector3.Project(rigid.velocity, transform.forward).magnitude / maxAccelarationSpeed);

        tryingToJump = false;//checking in update for better responsiveness
        //tryingToSlide = false;
    }

    void getInput()//what is the player trying to do?
    {
        //Input

        //CoyoteTime
        isGrounded = Physics.Raycast(new Ray(transform.position, Vector3.down), transform.localScale.y + 0.2f);
        if (isGrounded) timeSinceGrounded = 0;
        else timeSinceGrounded += Time.deltaTime;

        //Jump
        timeSinceLastJump += Time.deltaTime;
        if (Input.GetButtonDown(jump) && timeSinceLastJump >= jumpCooldown)
        {
            tryingToJump = true;
            timeSinceLastJump = 0f;
        }

        //slide
        //TODO: copy or rework

        //movement
        horiz = Input.GetAxisRaw(moveAxisHoriz);
        vert = Input.GetAxisRaw(moveAxisVerti);

        //LookAround
        if (!useController)
        {
            deltaXRot = Input.GetAxisRaw(lookAxisHoriz) * mouseSpeed * Time.timeScale;
            deltaYRot = -Input.GetAxisRaw(lookAxisVerti) * mouseSpeed * Time.timeScale;

            if (invertXAxis) deltaXRot = -deltaXRot;
            if (invertYAxis) deltaYRot = -deltaYRot;
        }
        else//TODO: rework controller input
        {
            deltaXRot = Input.GetAxisRaw(controllerAxisHoriz) * mouseSpeed * Time.timeScale;
            deltaYRot = Input.GetAxisRaw(controllerAxisVerti) * mouseSpeed * Time.timeScale;

            //FIXME: camera rotates the wrong way when wallrunning and wallTurning
        }//use RotateAround to stay relative to player and not camera
        transform.RotateAround(camera.transform.position, transform.up, deltaXRot); //apply horizontal cam rots
        //dont break your neck (min & max vertical camera rot)
        float angle = camera.transform.rotation.eulerAngles.x + deltaYRot;
        if (camera.transform.rotation.eulerAngles.x + deltaYRot > 90) angle -= 360;
        if (angle > -89 && angle < 89)
        {
            // camera.transform.Rotate(new Vector3(deltaYRot, 0, 0));
            camera.transform.RotateAround(camera.transform.position, transform.right, deltaYRot);
        }

        //Reset
        if (Input.GetKeyDown(reset))
        {
            myReset.ResetAll();
            Debug.Log("Reset");
        }
        if (Input.GetKeyDown(quit))
        {
            #if (UNITY_EDITOR)
                        UnityEditor.EditorApplication.isPlaying = false;
            #elif (UNITY_STANDALONE)
                                Application.Quit();
            #endif
            Debug.Log("Attempted to quit application");
        }
    }

    IEnumerator turnAround()//used for wallturn
    {
        //FIXME: fix camera fuck up during turnaround
        //ARE THREADS MESSING WITH ME?
        float turned = 0;
        while (turned < 180)
        {
            float toTurn = turnSpeed * Time.deltaTime;
            if (turned + toTurn > 180)//dont turn too far
            {
                toTurn = 180 - turned;
            }
            transform.Rotate(new Vector3(0, toTurn, 0));
            turned += toTurn;//apply rotation
            currentState = state.wallTurning;//should be redundant (make sure it doesnt change)
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log("Eded with " + turned + " deg");
        currentState = state.falling;
    }

    public string getCurrentStateName()
    {
        return currentState.ToString();
    }

    public IEnumerator loadPlayerData()//settings for movement etc
    {
        //general variables
        maxSpeed = playerData.maxSpeed;
        maxAccelarationSpeed = playerData.maxAccelarationSpeed;
        accelarationForce = playerData.accelarationForce;
        myDrag = playerData.myDrag;


        //jump
        jumpForce = playerData.jumpForce;
        aerialMultiplier = playerData.aerialMultiplier;
        jumpCooldown = playerData.jumpCooldown;


        //CoyoteTime
        groundedMargin = playerData.groundedMargin;

        //wallRun
        wallRunVertivalDrag = playerData.wallRunVertivalDrag;
        wallMultiplier = playerData.wallMultiplier;

        wallRunCameraTilt = playerData.wallRunCameraTilt;
        wallRunCameraTiltSpeed = playerData.wallRunCameraTiltSpeed;

        wallRunWallAttraction = playerData.wallRunWallAttraction;

        wallRunDetectionDistance = playerData.wallRunDetectionDistance;
        wallMinVelocity = playerData.wallMinVelocity;
        //   wallRunMaxAngle = 60f; // redundant because raycast


        //wallJump
        wallRunJumpNormalForce = playerData.wallRunJumpNormalForce;
        wallRunJumpTangentForce = playerData.wallRunJumpTangentForce;
        wallRunJumpForwardForce = playerData.wallRunJumpForwardForce;
        wallRunJumpVerticalForce = playerData.wallRunJumpVerticalForce;

        //wallTurn
        wallTurnDetectionDistance = playerData.wallTurnDetectionDistance;
        wallTurnVerticalForce = playerData.wallTurnVerticalForce;
        wallTurnNormalForce = playerData.wallTurnNormalForce;
        wallTurnMinVerticalSpeed = playerData.wallTurnMinVerticalSpeed;
        turnSpeed = playerData.turnSpeed;


        //pullUp
        pullUpVercticalForce = playerData.pullUpVercticalForce;
        pullUpForwardForce = playerData.pullUpForwardForce;

        yield return null;
    }

    public IEnumerator loadInputData()//settings for input
    {
        moveAxisHoriz = inputData.moveAxisHoriz;
        moveAxisVerti = inputData.moveAxisVerti;

        lookAxisHoriz = inputData.lookAxisHoriz;
        lookAxisVerti = inputData.lookAxisVerti;
        mouseSpeed = inputData.mouseSpeed;

        useRawInput = inputData.useRawInput;
        invertXAxis = inputData.invertXAxis;
        invertYAxis = inputData.invertYAxis;

        controllerAxisHoriz = inputData.controllerAxisHoriz;
        controllerAxisVerti = inputData.controllerAxisVerti;
        useController = inputData.useController;

        jump = inputData.jump;

        reset = inputData.reset;
        quit = inputData.quit;

        interact = inputData.interact;
        interactor.button = interact;

        switchButton = inputData.switchButton;
        switcher.button = switchButton;

        yield return null;
    }



    private void OnDrawGizmos()
    {
        /* float minX = 10, maxX = 20, minY = 10, maxY = 20, minZ = 10, maxZ = 20;
         Transform target = new GameObject().transform;
         transform.LookAt(target);
         Vector3 myRotation = transform.rotation.eulerAngles;

         if (myRotation.x < minX) myRotation.x = minX;
         else if (myRotation.x < maxX) myRotation.x = maxX;

         if (myRotation.y < minY) myRotation.y = minY;
         else if (myRotation.y < maxY) myRotation.y = maxY;

         if (myRotation.z < minZ) myRotation.z = minZ;
         else if (myRotation.z < maxZ) myRotation.z = maxZ;

         transform.rotation.SetEulerAngles(myRotation);*/


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
