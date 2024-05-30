using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine.AI;

public class TaskSkeletonPickupHead : Node
{
    NightmareSkeletonController owner;
    NavMeshAgent navAgent;
    NightmareSkeletonHeadController headController;

    private float waitTime = 1f;
    private float waitTimer = 1f;

    public TaskSkeletonPickupHead(NightmareSkeletonController owner, NavMeshAgent navAgent, NightmareSkeletonHeadController headController)
    {
        this.owner = owner;
        this.navAgent = navAgent;
        this.headController = headController;
    }

    public override Status Check(float dt)
    {
        if (GetData("pickingUp") == null)
            parent.SetData("pickingUp", false);

        if ((bool)GetData("pickingUp"))
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer > 0)
            {
                status = Status.RUNNING;
                return status;
            }

            owner.SetHeadOffStatus(false);

            parent.SetData("pickingUp", false);

            status = Status.SUCCESS;
            return status;
        }
        else if(owner.headPickupAllowed)
        {
            waitTimer = waitTime;
            parent.SetData("pickingUp", true);

            owner.headlessAnimator.SetTrigger("headPickup");

            navAgent.destination = navAgent.transform.position;
            headController.StopHead();

            status = Status.RUNNING;
            return status;
        }

        status = Status.FAILURE;
        return status;
    }
}
