using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    InputManager inputManager;
    Animator anim;
    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;

    PlayerStats playerStats;

    public bool isInteracting;

    [Header("Player Flags")]
    public bool isSprinting;
    public bool isInAir;
    public bool isGrounded;
    public bool canDoCombo;
    public bool isUsingRightHand;
    public bool isUsingLeftHand;

    private void Awake()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();
    }

    void Start()
    {
        inputManager = GetComponent<InputManager>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerStats = GetComponent<PlayerStats>();
    }


    void Update()
    {
        float delta = Time.deltaTime;
        isInteracting = anim.GetBool("isInteracting");
        canDoCombo = anim.GetBool("canDoCombo");
        isUsingRightHand = anim.GetBool("isUsingRightHand");
        isUsingLeftHand = anim.GetBool("isUsingLeftHand");
        inputManager.HandleAllInputs(delta);
        playerLocomotion.IsSprinting();
        playerLocomotion.IsRolling(delta);
        CheckForInteractableObject();
        // playerLocomotion.HandleMoving(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
        playerStats.RegenerateStamina();
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        // there's ongoing bug that make player won't move during animation when turned on interpolated is player rigidbody
        // playerLocomotion.HandleMoving(delta);
        // playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);


    }

    private void LateUpdate()
    {
        inputManager.rollFlag = false;
        inputManager.sprintFlag = false;
        inputManager.hattackInput = false;
        inputManager.lattackInput = false;
        inputManager.d_Pad_Up = false;
        inputManager.d_Pad_Down = false;
        inputManager.d_Pad_Left = false;
        inputManager.d_Pad_Right = false;
        inputManager.Pickup_Input = false;
        float delta = Time.deltaTime;
        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, inputManager.mouseX, inputManager.mouseY);
        }
        // if in air, start counting (can be used to implement falling dmg)
        if (isInAir)
        {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }
    }

    public void CheckForInteractableObject()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        rayOrigin.y = rayOrigin.y + 2f;

        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers) || Physics.SphereCast(rayOrigin, 0.3f, Vector3.down, out hit, 2.5f, cameraHandler.ignoreLayers))
        {
            if (hit.collider.tag == "Interactable")
            {
                // Debug.Log("hello");
                Interactable interactableObject = hit.collider.GetComponent<Interactable>();

                if (interactableObject != null)
                {
                    // Debug.Log("hello1");
                    string interactableText = interactableObject.interactbleText;
                    //SET THE UI TEXT TO THE INTERACTABLE OBJECT'S TEXT
                    //SET THE TEXT POP UP TO TRUE

                    if (inputManager.Pickup_Input)
                    {
                        // Debug.Log("hello2");
                        hit.collider.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        }
    }


}
