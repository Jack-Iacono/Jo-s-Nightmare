using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskSkeletonGoToHead : Node
{
    private NightmareSkeletonController owner;
    private NavMeshAgent navAgent;

    public TaskSkeletonGoToHead(NightmareSkeletonController owner, NavMeshAgent navAgent)
    {
        this.owner = owner;
        this.navAgent = navAgent;
    }

    public override Status Check()
    {
        // Get the current target node
        Transform target = owner.head.transform;

        // Check if the agent is still not at the target
        if (Vector3.Distance(owner.transform.position, target.position) > 1.5f)
        {
            parent.SetData("pickingUp", false);

            navAgent.destination = target.position;
            status = Status.RUNNING;
            return status;
        }

        status = Status.SUCCESS;
        return status;
    }
}
