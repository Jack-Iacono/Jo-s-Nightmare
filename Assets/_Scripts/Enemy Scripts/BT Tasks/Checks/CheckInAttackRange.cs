using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckInAttackRange : Node
{
    private float attackRange;
    private float sightAngle;
    private Transform transform;
    private Transform target;
    
    public CheckInAttackRange(Transform target, Transform attacker, float attackRange, float sightAngle)
    {
        this.target = target;
        transform = attacker;
        this.attackRange = attackRange;
        this.sightAngle = sightAngle;
    }

    public override Status Check()
    {
        // Check if the player is close enough to the user
        if (Vector3.Distance(target.position, transform.position) <= attackRange)
        {
            // Check if the player is within the vision arc
            if (Vector3.Dot(transform.forward, (target.position - transform.position).normalized) >= sightAngle)
            {
                Debug.Log("Attack");

                status = Status.SUCCESS;
                return status;
            }
        }

        // If the enemy can't see the player and there is no known last position, then it is  a failure
        status = Status.FAILURE;
        return status;
    }

}
