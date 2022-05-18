using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : CharacterManager
{
    EnemyLocomotionManager enemyLocomotionManager;
    EnermyAnimationHandler enemyAnimationManager;
    EnemyStats enemyStats;

    public NavMeshAgent navmeshAgent;
    public State currentState;
    public CharacterStats currentTarget;
    public Rigidbody enemyRigidBody;

    public bool isPreformingAction;
    public bool isInteracting;
    public float stoppingDistance = 1f;
    public float rotationSpeed = 15;
    public float maximumAttackRange = 1.5f;

    // public EnemyAttackAction[] enemyAttacks;
    // public EnemyAttackAction currentAttack;

    [Header("A.I Settings")]
    public float detectionRadius = 20;
    //The higher, and lower, respectively these angles are, the greater detection FIELD OF VIEW (basically like eye sight)
    public float maximumDetectionAngle = 50;
    public float minimumDetectionAngle = -50;

    public float currentRecoveryTime = 0;
    public float viewableAngle;

    private void Awake()
    {
        enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
        enemyAnimationManager = GetComponentInChildren<EnermyAnimationHandler>();
        enemyStats = GetComponent<EnemyStats>();
        enemyRigidBody = GetComponent<Rigidbody>();
        navmeshAgent.enabled = false;
    }

    private void Start()
    {
        enemyRigidBody.isKinematic = false;
    }

    private void Update()
    {
        HandleRecoveryTimer();

        isInteracting = enemyAnimationManager.animator.GetBool("isInteracting");

    }

    private void FixedUpdate()
    {
        HandleStateMachine();
    }

    private void HandleStateMachine()
    {
        if (currentState != null)
        {
            State nextState = currentState.Tick(this, enemyStats, enemyAnimationManager);

            if (nextState != null)
            {
                SwitchToNextState(nextState);
            }
        }
    }

    private void SwitchToNextState(State state)
    {
        currentState = state;
    }

    private void HandleRecoveryTimer()
    {
        if (currentRecoveryTime > 0)
        {
            currentRecoveryTime -= Time.deltaTime;
        }

        if (isPreformingAction)
        {
            if (currentRecoveryTime <= 0)
            {
                isPreformingAction = false;
            }
        }
    }

}