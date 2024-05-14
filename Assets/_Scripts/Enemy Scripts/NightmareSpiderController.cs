using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class NightmareSpiderController : NightmareController
{
    [Header("Unique Spider Characteristics")]
    public int numberOfMoves = 4;
    public int movesRemaining = -1;

    public float currentMoveDelay = 0;
    private ParticleSystem moveParticles;

    public override void StartOverrides()
    {
        spawnLocation = NightSpawnController.spiderSpawnPosition;

        base.StartOverrides();

        moveParticles = GetComponent<ParticleSystem>();

        movesRemaining = numberOfMoves;
        currentMoveDelay = Random.Range(targetDelay - 2, targetDelay + 2);
    }
    public override void SpawnSetup()
    {
        spawnLocation = NightSpawnController.spiderSpawnPosition;

        SoundManager.PlaySound(SoundManager.spiderWalking);

        FindNewTarget();

        InterfaceController.Instance.HUDMessage("Your skin begins to crawl");

        base.SpawnSetup();
    }

    public override void RunBehaviorTree()
    {
        base.RunBehaviorTree();
    }

    #region Target Nodes

    public override Node.Status BeginTargetMove()
    {
        FindNewTarget();

        SoundManager.PlaySound(SoundManager.spiderWalking);
        moveParticles.Play();

        return base.BeginTargetMove();
    }
    public override Node.Status TargetMove()
    {
        if (base.TargetMove() == Node.Status.SUCCESS)
        {
            moveParticles.Stop();
            return Node.Status.SUCCESS;
        }

        return base.TargetMove();
    }
    public override Node.Status TargetMoveDelay()
    {
        Node.Status status = Delay(currentMoveDelay);

        if (status == Node.Status.SUCCESS)
        {
            ResetDelay();
        }

        return status;
    }
    public override Node.Status EndTargetMove()
    {
        if (movesRemaining > 0)
        {
            movesRemaining--;
            behaviorTree.StartSequence("Target");
            return Node.Status.RUNNING;
        }
        else
        {
            movesRemaining = numberOfMoves;
            return Node.Status.SUCCESS;
        }
            
    }

    #endregion

    #region Retreat Methods

    public override Node.Status BeginRetreatMove()
    {
        movesRemaining = numberOfMoves;
        return base.BeginRetreatMove();
    }

    #endregion

    public override void PlayerDamageSwing()
    {
        // Do whatever the base class says
        base.PlayerDamageSwing();

        // Force the spider to retreat
        navAgent.isStopped = false;
        behaviorTree.StartSequence("Retreat");
    }

    public void FindNewTarget()
    {
        //Randomizes the Spider's Movement Times
        currentMoveDelay = Random.Range(targetDelay - 2, targetDelay + 2);
        targetLocation = NightSpawnController.Instance.GetRandomSpiderPoint(targetLocation);
    }

    public override bool CheckPath()
    {
        // TLDR: Checks if the spdier can access at least 2 points
        Vector3 destStore = targetLocation;

        int spiderPointAvailable = 0;

        //Checks for route to all spider points
        for(int i = 0; i < NightSpawnController.Instance.spiderPoints.Count; i++)
        {
            targetLocation = NightSpawnController.Instance.spiderPoints[i].transform.position;

            if (base.CheckPath())
            {
                NightSpawnController.Instance.SetSpiderPointValid(i, true);
                spiderPointAvailable++;
            }
            else
                NightSpawnController.Instance.SetSpiderPointValid(i, false);
        }

        if (spiderPointAvailable < 2)
            return false;

        //Checks for route to bed
        targetLocation = NightSpawnController.bedPosition;

        if (!base.CheckPath())
        {
            return false;
        }

        targetLocation = destStore;
        navAgent.destination = targetLocation;
        return true;
    }
}
