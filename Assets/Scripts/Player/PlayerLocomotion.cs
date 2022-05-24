using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;

    PlayerStats playerStats;

    Transform cameraObject;
    InputManager inputManager;
    public Vector3 moveDirection;

    [HideInInspector]
    public Transform myTransform;
    [HideInInspector]
    public AnimatorHandler animatorHandler;

    public new Rigidbody Rigidbody;
    public GameObject normalCamera;

    [Header("Gorund & Air Detection Stats")]

    [SerializeField]
    float groundDetectionRayStartPoint = 0.5f;
    [SerializeField]
    float minimumDistanceNeededToBeginFall = 1f;
    [SerializeField]
    float groundDirectionRayDistance = 0.2f; //offset where the raycast begin
    [SerializeField]
    float shiftForwardRayDistance = -0.1f;
    LayerMask ignoreForGroundCheck;
    public float inAirTimer;

    [Header("Stats")]
    [SerializeField]
    float movementSpeed = 5;
    [SerializeField]
    float sprintFactor = 2;
    [SerializeField]
    float walkingSpeed = 1;
    [SerializeField]
    float rotationSpeed = 10;
    [SerializeField]
    float fallingSpeed = 45;
    bool isSprinting;

    public CapsuleCollider characterCollider;
    public CapsuleCollider characterCollisionBlockerCollider;

    int rollStaminaCost = 15;
    int sprintStaminaCost = 10;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        Rigidbody = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
        playerStats = GetComponent<PlayerStats>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        cameraObject = Camera.main.transform;
        myTransform = transform;
        animatorHandler.Initialized();

        playerManager.isGrounded = true;
        // not sure what below code do
        ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
        Physics.IgnoreCollision(characterCollider, characterCollisionBlockerCollider, true);
    }
    private void Update()
    {
        float delta = Time.deltaTime;
        // Debug.Log(normalVector);

        // inputManager.HandleAllInputs(delta); //recieve input in InputManager
        IsRolling(delta);
        if (!animatorHandler.anim.GetBool("isInteracting") && inAirTimer == 0) //player don't move while rolling
        {

            IsSprinting();

            HandleMoving(delta);
            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
            animatorHandler.UpdateAnimatorLocomotionValues(inputManager.moveAmount, 0, isSprinting); //Change animation states
        }
    }
    #region Movement Function
    Vector3 normalVector;
    Vector3 targetPosition;

    public void HandleMoving(float delta)
    {
        //Make player move relate with inputs
        moveDirection = cameraObject.forward * inputManager.vertical;
        moveDirection += cameraObject.right * inputManager.horizontal;
        moveDirection.y = 0;
        moveDirection.Normalize();

        float speed = movementSpeed;
        if (isSprinting)
        {
            if (playerStats.currentStamina <= 0 || playerManager.isInAir)
            {
                return;
            }
            speed *= sprintFactor;
            playerStats.TakeStaminaDamage(sprintStaminaCost);

        }
        moveDirection *= speed;

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        GetComponent<Rigidbody>().velocity = projectedVelocity;
    }

    private void HandleRotation(float delta)
    {
        //Rotate player to the moving direction
        Vector3 targetDir = Vector3.zero;
        float moveOverride = inputManager.moveAmount;

        targetDir = cameraObject.forward * inputManager.vertical;
        targetDir += cameraObject.right * inputManager.horizontal;

        targetDir.Normalize();
        targetDir.y = 0;

        if (targetDir == Vector3.zero)
        {
            targetDir = myTransform.forward;
        }
        float rs = rotationSpeed;

        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

        myTransform.rotation = targetRotation;
    }
    #endregion

    public void IsSprinting()
    {
        //check if player stand still
        if (inputManager.vertical == 0 && inputManager.horizontal == 0)
        {
            isSprinting = false;
        }
        else
        {

            if (playerStats.currentStamina <= 0)
            {
                isSprinting = false;
            }
            else
            {
                isSprinting = inputManager.sprintInput;
            }


        }
    }

    public void IsRolling(float delta)
    {
        //Handle rolling and backstep actions
        if (animatorHandler.anim.GetBool("isInteracting"))
            return;

        if (playerStats.currentStamina <= 0 || playerStats.currentStamina <= rollStaminaCost)
            return;

        if (inputManager.rollFlag)
        {
            moveDirection = cameraObject.forward * inputManager.vertical;
            moveDirection += cameraObject.right * (inputManager.horizontal);

            if (inputManager.moveAmount > 0)
            {
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotation;
                animatorHandler.PlayTargetAnimation("Rolling", true);
                playerStats.TakeStaminaDamage(rollStaminaCost);
            }
            else
            {
                animatorHandler.PlayTargetAnimation("Backstep", true);
            }
        }
    }
    public void HandleFalling(float delta, Vector3 moveDirection)
    {
        playerManager.isGrounded = false;
        RaycastHit hit;
        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;
        origin.z += shiftForwardRayDistance;

        // if raycast hit something forward, player won't be moving
        if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
        {
            moveDirection = Vector3.zero;
        }

        if (playerManager.isInAir)
        {
            if(inAirTimer < 2)
            {
                GetComponent<Rigidbody>().AddForce(moveDirection * 20f);
            }
            GetComponent<CapsuleCollider>().height = 1f;
            // falling at set speed
            GetComponent<Rigidbody>().AddForce(-Vector3.up * fallingSpeed);
            // falling forward speed
            GetComponent<Rigidbody>().AddForce(moveDirection * fallingSpeed / 10f);
        }
        else
        {
            GetComponent<CapsuleCollider>().height = 2f;
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        origin = origin + dir * groundDirectionRayDistance;

        targetPosition = myTransform.position;


        Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
        if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
        {
            normalVector = hit.normal;
            Vector3 tp = hit.point;
            playerManager.isGrounded = true;
            targetPosition.y = tp.y;

            if (playerManager.isInAir)
            {
                // Debug.Log("You were in the air for " + inAirTimer);
                animatorHandler.PlayTargetAnimation("Land", true);
                inAirTimer = 0;
                // if in air more than 0.5 sec start falling
                // if (inAirTimer > 0.08f)
                // {
                //     Debug.Log("You were in the air for " + inAirTimer);
                //     animatorHandler.PlayTargetAnimation("Land", true);
                //     inAirTimer = 0;
                // }
                // // if less then play locomotion
                // else
                // {
                //     animatorHandler.PlayTargetAnimation("Rolling", true);
                //     inAirTimer = 0;
                // }

                playerManager.isInAir = false;
            }
        }
        else
        {
            if (playerManager.isGrounded)
            {
                // if grounded while in the air, isGrounded become false
                playerManager.isGrounded = false;
            }

            if (playerManager.isInAir == false)
            {
                if (playerManager.isInteracting == false)
                {
                    animatorHandler.PlayTargetAnimation("Falling", true);
                }

                Vector3 vel = GetComponent<Rigidbody>().velocity;
                vel.Normalize();
                GetComponent<Rigidbody>().velocity = vel * (movementSpeed / 2);
                playerManager.isInAir = true;
            }
        }

        if (playerManager.isGrounded)
        {
            if (playerManager.isInteracting || inputManager.moveAmount > 0)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
            }
            else if (normalVector != Vector3.up)
            {
                // GetComponent<Rigidbody>().useGravity = false;
                return;
            }
            else
            {
                myTransform.position = targetPosition;
            }
        }

    }
}
