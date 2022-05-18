using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    PlayerManager playerManager;
    public Animator anim;
    public InputManager inputManager;
    public PlayerLocomotion playerLocomotion;
    int vertical;
    int horizontal;
    public bool canRotate;

    public void Initialized()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        anim = GetComponent<Animator>();
        inputManager = GetComponentInParent<InputManager>();
        playerLocomotion = GetComponentInParent<PlayerLocomotion>();

        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorLocomotionValues(float verticalMovement, float horizontalMovement, bool isSprinting)
    {
        /* Change Locomotion animation blentree values related with velocity */
        #region Vertical
        float v = 0;
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            v = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            v = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            v = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            v = -1;
        }
        else
        {
            v = 0;
        }
        #endregion

        #region Horizontal
        float h = 0;
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            h = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            h = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            h = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            h = -1;
        }
        else
        {
            h = 0;
        }
        #endregion
        if (isSprinting)
        {
            v = 2;
            h = horizontalMovement;
        }
        anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
        anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
    }

    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        /* get out from blendtree to play action animation */
        anim.applyRootMotion = isInteracting;
        anim.SetBool("isInteracting", isInteracting);
        anim.CrossFade(targetAnim, 0.2f);
    }
    #region checkforRotatePlayer
    public void CanRotate()
    {
        canRotate = true;
    }
    public void StopRotation()
    {
        canRotate = false;
    }
    #endregion

    public void EnableCombo()
    {
        anim.SetBool("canDoCombo", true);
    }

    public void DisableCombo()
    {
        anim.SetBool("canDoCombo", false);
    }
    private void OnAnimatorMove()
    {
        if (playerManager.isInteracting == false)
            return;

        float delta = Time.deltaTime;
        playerLocomotion.GetComponent<Rigidbody>().drag = 0;
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        playerLocomotion.GetComponent<Rigidbody>().velocity = velocity;
    }
}
