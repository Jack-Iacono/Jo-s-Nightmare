using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using System;
using UnityEngine.AI;

public class SkeletonBT : BehaviorTree.Tree
{
    private NightmareSkeletonController owner;

    public SkeletonBT(NightmareSkeletonController owner)
    {
        this.owner = owner;
    }

    protected override Node SetupTree()
    {
        owner.navAgent = owner.GetComponent<NavMeshAgent>();

        // Establises the Behavior Tree and its logic
        Node root = new Selector(new List<Node>()
        {
            new Sequence(new List<Node>
            {
                new CheckSkeletonHead(owner),
                new TaskSkeletonGoToHead(owner, owner.navAgent),
                new TaskSkeletonPickupHead(owner, owner.navAgent, owner.head)
            }),
            new Sequence(new List<Node>
            {
                new CheckPlayerInSight(owner, PlayerController.playerInstances[0].gameObject.transform),
                new TaskChasePlayer(owner.transform, owner.navAgent),
                new CheckArea(owner, PlayerController.playerInstances[0].gameObject.transform)
            }),
            new TaskPatrol(owner.transform, NightmareSpawnController.Instance.skeletonPatrolPoints, owner.navAgent)
        });

        root.SetData("speed", owner.moveSpeed);

        return root;
    }

}
