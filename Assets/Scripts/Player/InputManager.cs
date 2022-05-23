using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    PlayerControl inputActions;
    CameraHandler cameraHandler;
    PlayerAttack playerAttack;
    PlayerInventory playerInventory;
    PlayerManager playerManager;

    PlayerStats playerStats;


    public WeaponItem unarmedWeapon;

    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;
    public bool d_Pad_Up;
    public bool d_Pad_Down;
    public bool d_Pad_Left;
    public bool d_Pad_Right;

    // Rolling
    public bool rollInput;
    public bool rollFlag;
    public bool isInteracting;
    public float rollInputTimer;
    Vector2 movementInput;
    Vector2 cameraInput;
    public bool sprintInput;
    public bool sprintFlag;
    public bool comboFlag;
    public bool lattackInput;
    public bool hattackInput;
    public bool lockOnFlag;


    public bool Pickup_Input;
    public bool lockOnInput;
    public bool K_Lock_Left_Input;
    public bool L_Lock_Right_Input;
    AnimatorHandler animatorHandler;
    private void Awake()
    {
        playerAttack = GetComponentInChildren<PlayerAttack>();
        playerInventory = GetComponent<PlayerInventory>();
        // cameraHandler = CameraHandler.singleton;
        cameraHandler = FindObjectOfType<CameraHandler>();
        playerManager = GetComponent<PlayerManager>();
        playerStats = GetComponent<PlayerStats>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }

    // private void FixedUpdate()
    // {
    //     float delta = Time.fixedDeltaTime;

    //     if (cameraHandler != null)
    //     {
    //         cameraHandler.FollowTarget(delta);
    //         cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
    //     }
    // }

    private void OnEnable()
    {
        //Read values from inputActions
        if (inputActions == null)
        {
            inputActions = new PlayerControl();
            inputActions.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            // inputActions.PlayerActions.Sprint.started += ctx => Debug.Log(ctx.ReadValueAsObject());
            inputActions.PlayerActions.LAttack.performed += i => lattackInput = true;
            inputActions.PlayerActions.HAttack.performed += i => hattackInput = true;
            inputActions.PlayerQuickSlots.DPadRight.performed += i => d_Pad_Right = true;
            inputActions.PlayerQuickSlots.DPadLeft.performed += i => d_Pad_Left = true;
            inputActions.PlayerActions.Pickup.performed += i => Pickup_Input = true;


            inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
            inputActions.PlayerMovement.LockOnTargetRight.performed += i => L_Lock_Right_Input = true;
            inputActions.PlayerMovement.LockOnTargetLeft.performed += i => K_Lock_Left_Input = true;

        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void HandleAllInputs(float delta)
    {
        MoveInput(delta);
        SprintInput();
        HandleRollInput(delta);
        AttackInput(delta);
        // HandleQuickSlotsInput();
        HandleInteractingButtonInput();
        HandleLockOnInput();
    }

    public void MoveInput(float delta)
    {
        //Keep values from inputAction in variables
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void SprintInput()
    {
        sprintInput = inputActions.PlayerActions.Sprint.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        // Debug.Log(sprintInput);
    }

    private void HandleRollInput(float delta)
    {
        rollInput = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;

        if (rollInput)
        {
            rollFlag = true;
        }
        else
        {
            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                sprintFlag = false;
                rollFlag = true;
            }

            rollInputTimer = 0;
        }
    }

    private void AttackInput(float delta)
    {

        if (playerInventory.rightWeapon == unarmedWeapon)
        {
            return;
        }
        if (lattackInput)
        {
            playerAttack.HandleRBAction();
        }
        if (hattackInput)
        {
            if (playerManager.canDoCombo)
            {
                comboFlag = true;
                playerAttack.HandleWeaponCombo(playerInventory.rightWeapon);
                comboFlag = false;
            }
            else
            {
                if (playerManager.isInteracting)
                    return;
                if (playerManager.canDoCombo)
                    return;
                playerAttack.HandleHeavyAttack(playerInventory.rightWeapon);

            }
        }
    }

    private void HandleQuickSlotsInput()
    {
        if (d_Pad_Right)
        {
            playerInventory.ChangeRightWeapon();
        }
        else if (d_Pad_Left)
        {
            playerInventory.ChangeLeftWeapon();
        }
    }

    private void HandleInteractingButtonInput()
    {
        if (Pickup_Input)
        {
            Debug.Log("pick up item");
        }
    }
    private void HandleLockOnInput()
    {
        // lockOn on
        if (lockOnInput && lockOnFlag == false)
        {
            lockOnInput = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.nearestLockOnTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                lockOnFlag = true;
            }
        }
        // lockOn off
        else if (lockOnInput && lockOnFlag)
        {
            lockOnInput = false;
            lockOnFlag = false;
            cameraHandler.ClearLockOnTargets();
        }

        if (lockOnFlag && K_Lock_Left_Input)
        {
            K_Lock_Left_Input = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.leftLockTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
            }
        }

        if (lockOnFlag && L_Lock_Right_Input)
        {
            L_Lock_Right_Input = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.rightLockTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
            }
        }
        cameraHandler.SetCameraHeight();
    }
}
