using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public PursueTargetState pursueTargetState;
    public LayerMask detectionLayer;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnermyAnimationHandler enemyAnimationHandler)
    {
        //Look for a potential target
        //switch to the pursue target state if target is found
        #region target detection
        // Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

        // for (int i = 0; i < colliders.Length; i++)
        // {
        //     CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

        //     if (characterStats != null)
        //     {
        //             //CHECK FOR TEAM ID

        //         Vector3 targetDirection = characterStats.transform.position - transform.position;
        //         float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

        //         if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
        //         {
        //             currentTarget = characterStats;
        //         }
        //     }
        // }
        #endregion
        enemyManager.currentTarget = GameObject.Find("Player").GetComponent<CharacterStats>();
        
        #region handle state switching
        if (enemyManager.currentTarget != null && !enemyManager.enemyStats.isDead)
        {
            // return this;
            return pursueTargetState;
        }
        else
        {
            return this;
        }
        #endregion
    }
}
