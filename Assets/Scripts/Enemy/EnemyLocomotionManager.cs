using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemyLocomotionManager : MonoBehaviour
{
    public float fallingVel;
    public float raycastHeightOffset = 0.5f;
    private float currentHitDistance;
    public LayerMask groundLayer;
    Vector3 raycastOrigin;
    public float rayDistance = 0.2f;
    private new Rigidbody Rigidbody;
    public float inAirTimer;

    public CapsuleCollider characterCollider;
    public CapsuleCollider characterCollisionBlockerCollider;
    Vector3 normalVector;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        raycastOrigin = transform.position;
        RaycastHit hit;
        raycastOrigin.y = raycastOrigin.y + raycastHeightOffset;
        if (Physics.Raycast(raycastOrigin, -Vector3.up, out hit, rayDistance, groundLayer))
        {
            Debug.DrawLine(raycastOrigin, hit.point, Color.red);
            normalVector = hit.normal;
            if (normalVector != Vector3.up)
            {
                Vector3 projectedVelocity = Vector3.ProjectOnPlane(Rigidbody.velocity * 20, normalVector);
                Rigidbody.AddForce(projectedVelocity);
            }
            inAirTimer = 0;
        }
        else
        {
            Debug.DrawLine(raycastOrigin, raycastOrigin - Vector3.up * rayDistance, Color.green);
            inAirTimer = inAirTimer + Time.deltaTime;
            Rigidbody.AddForce(-Vector3.up * fallingVel * inAirTimer);
        }
        // Debug.Log(Rigidbody.velocity);
    }
    private void Start()
    {
        Physics.IgnoreCollision(characterCollider, characterCollisionBlockerCollider, true);

    }
}
