using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueTargetState : State
{
    public CombatStanceState combatStanceState;

    public IdleState idleState;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnermyAnimationHandler enemyAnimationManager)
    {
        if(enemyStats.isDead)
        {
            enemyManager.navmeshAgent.transform.localPosition = Vector3.zero; 
            enemyManager.navmeshAgent.enabled = false;
            return idleState;
        }
        Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
        float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
        float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);
        // Debug.Log(distanceFromTarget);
        if (distanceFromTarget > enemyManager.maximumAttackRange)
        {
            enemyAnimationManager.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
        }
        HandleRotateTowardTarget(enemyManager, distanceFromTarget);
        enemyManager.navmeshAgent.transform.localPosition = Vector3.zero; 
        enemyManager.navmeshAgent.transform.localRotation = Quaternion.identity;
        
        if (distanceFromTarget <= enemyManager.maximumAttackRange)
        {
            enemyManager.navmeshAgent.transform.localPosition = Vector3.zero; 
            enemyManager.navmeshAgent.enabled = false;
            return combatStanceState;
        }
        else
        {
            return this;
        }        
    }

    private void HandleRotateTowardTarget(EnemyManager enemyManager, float distanceFromTarget) 
    {
        //manually
        if(enemyManager.isPreformingAction)
        {
            Vector3 direction = enemyManager.currentTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();
            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
        //navmesh
        else
        {
            Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navmeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRigidBody.velocity;
            enemyManager.navmeshAgent.enabled = true;
            enemyManager.navmeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
            enemyManager.enemyRigidBody.velocity = targetVelocity;
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navmeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
    }
}
