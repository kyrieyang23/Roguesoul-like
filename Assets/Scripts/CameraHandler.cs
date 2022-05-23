using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    InputManager inputmanager;
    PlayerManager playerManager;

    public Transform targetTransform;
    public Transform cameraTransform;
    public Transform cameraPivotTransform;
    private Transform myTransform;
    private Vector3 cameraTransformPosition;
    public LayerMask ignoreLayers;
    public LayerMask enviromentLayer;

    private Vector3 cameraFollowVelocity = Vector3.zero;

    public static CameraHandler singleton;

    public float lookSpeed = 0.1f; //actually expected travel distance per sec
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.03f;

    private float targetPosition;
    private float defaultPosition;
    private float lookAngle;
    private float pivotAngle;
    public float minimumPivot = -35;
    public float maximumPivot = 35;

    public float cameraSphereRadius = 0.2f;
    public float cameraCollisionOffSet = 0.2f;
    public float minimumCollisionOffset = 0.2f;

    public float lockedPivotPosition = 2.25f;
    public float unlockedPivotPosition = 1.65f;

    public Transform currentLockOnTarget;

    List<CharacterManager> availableTargets = new List<CharacterManager>();
    public Transform nearestLockOnTarget;
    public Transform leftLockTarget;
    public Transform rightLockTarget;
    public float maximumLockOnDistance = 30;

    private void Awake() 
    {
        singleton = this;
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        inputmanager = FindObjectOfType<InputManager>();
        playerManager = FindObjectOfType<PlayerManager>();

    }

    private void Start()
    {
        enviromentLayer = LayerMask.NameToLayer("Environment");
    }

    private void Update() {
        Debug.DrawLine(transform.position, cameraPivotTransform.position, Color.red);
        Debug.DrawLine(cameraPivotTransform.position, cameraTransform.position, Color.red);
    }

    public void FollowTarget(float delta)
    {
        float t = delta/followSpeed;
        Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, t);
        // Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, followSpeed);
        myTransform.position = targetPosition;

        HandleCameraCollisions(delta);
    }

    private void HandleCameraCollisions(float delta)
    {
       targetPosition = defaultPosition;
       RaycastHit hit;
       Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
       direction.Normalize();

       if (Physics.SphereCast(cameraPivotTransform.position, 
       cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
       {
           float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
           targetPosition = -(dis - cameraCollisionOffSet);
       }
       if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
       {
           targetPosition = -minimumCollisionOffset;
       }

       cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta/0.2f);
       cameraTransform.localPosition = cameraTransformPosition;
    }

    public void HandleLockOn()
    {
        availableTargets.Clear();

        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;
        

        Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager charHandler = colliders[i].GetComponent<CharacterManager>();

                if (charHandler != null)
                {
                    Vector3 lockTargetDir = charHandler.transform.position - targetTransform.position;

                    float lockTargetDis = Vector3.Distance(targetTransform.position, charHandler.transform.position);

                    float viewableAngle = Vector3.Angle(lockTargetDir, transform.forward);
                    RaycastHit hit;

                    if (charHandler.transform.root != targetTransform.root  //Make sure we don't lock onto ourselves and the target is on screen and we are close enough to lock on
                        && viewableAngle > -50 && viewableAngle < 50
                        && lockTargetDis <= maximumLockOnDistance)
                    {
                        if (Physics.Linecast(playerManager.lockOnTransform.position, charHandler.lockOnTransform.position, out hit))
                        {
                            Debug.DrawLine(playerManager.lockOnTransform.position, charHandler.lockOnTransform.position);

                            if (hit.transform.gameObject.layer == enviromentLayer)
                            {
                                //Cannot lock onto target, object in the way
                            }
                            else
                            {
                                availableTargets.Add(charHandler);
                            }
                        }
                    }

                }
            }

            if (availableTargets.Count > 0)
            {
                for (int k = 0; k < availableTargets.Count; k++)
                {
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        nearestLockOnTarget = availableTargets[k].lockOnTransform;
                    }

                    if (inputmanager.lockOnFlag)
                    {
                        Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(availableTargets[k].transform.position);
                        var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTargets[k].transform.position.x;
                        var distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTargets[k].transform.position.x;

                        if (relativeEnemyPosition.x > 0.00 && distanceFromLeftTarget < shortestDistanceOfLeftTarget)
                        {
                            shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                            leftLockTarget = availableTargets[k].lockOnTransform;
                        }

                        if (relativeEnemyPosition.x < 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                        {
                            shortestDistanceOfRightTarget = distanceFromRightTarget;
                            rightLockTarget = availableTargets[k].lockOnTransform;
                        }
                    }
                }

            }
            else
            {
                Debug.Log("no lock on targets found A");
            }
        }
        else
        {
            Debug.Log("no lock on targets found B");
        }
    }

    public void ClearLockOnTargets()
    {
        availableTargets.Clear();
        nearestLockOnTarget = null;
        currentLockOnTarget = null;
    }

    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
        if (inputmanager.lockOnFlag == false && currentLockOnTarget == null)
        {
            lookAngle += (mouseXInput * lookSpeed) / delta; 
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            myTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
        else
        {
            if (currentLockOnTarget == null) {
                inputmanager.lockOnFlag = false;
            }
            else {
                float velocity = 0;

                Vector3 dir = currentLockOnTarget.position - transform.position;
                dir.Normalize();
                dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = targetRotation;

                dir = currentLockOnTarget.position - cameraPivotTransform.position;
                dir.Normalize();


                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                // eulerAngle.x = 6;
                eulerAngle.y = 0;
                cameraPivotTransform.localEulerAngles = eulerAngle;
            }
        }
    }
    public void SetCameraHeight()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
        Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

        if (currentLockOnTarget != null)
        {
            cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
        }
        else
        {
            cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
        }
    }
}
