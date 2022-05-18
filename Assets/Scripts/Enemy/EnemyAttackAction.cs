using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackAction", menuName = "EnemyAttackAction.cs/EnemyAttackAction", order = 0)]

public class EnemyAttackAction : EnemyAction
{
    public int attackScore =3;
    public float recoveryTime = 2;
    public float maximumAttackAngle = 35;
    public float minimumAttackAngle = -35;

    public float minimumDistanceNeededToAttack = 0;
    public float maximumDistanceNeededToAttack = 3;


}
