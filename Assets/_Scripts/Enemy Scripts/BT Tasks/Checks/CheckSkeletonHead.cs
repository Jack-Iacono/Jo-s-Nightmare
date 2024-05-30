using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckSkeletonHead : Node
{
    NightmareSkeletonController owner;

    public CheckSkeletonHead(NightmareSkeletonController owner)
    {
        this.owner = owner;
    }

    public override Status Check(float dt)
    {
        if (owner.headOff)
        {
            ClearData("target");
            ClearData("playerSightBuffer");

            status = Status.SUCCESS;
            return status;
        }
        
        status = Status.FAILURE;
        return status;
    }

}
