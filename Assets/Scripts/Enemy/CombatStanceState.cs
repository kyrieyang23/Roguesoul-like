using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStanceState : State
{
    public AttackState attackState;
    public PursueTargetState pursueTargetState;

    public IdleState idleState;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnermyAnimationHandler enemyAnimationManager)
    {
        float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
        //potentially circle player or walk around them

        if(enemyStats.isDead)
        {
            return idleState;
        }
        else
        {
            HandleRotateTowardsTarget(enemyManager);
        }

        if (enemyManager.isPreformingAction)
        {
            enemyAnimationManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
        }

        if (enemyManager.currentRecoveryTime <= 0 && distanceFromTarget <= enemyManager.maximumAttackRange)
        {
            return attackState;
        }
        else if (distanceFromTarget > enemyManager.maximumAttackRange)
        {
            return pursueTargetState;
        }
        else
        {
            return this;
        }
    }
    private void HandleRotateTowardsTarget(EnemyManager enemyManager)
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
}
