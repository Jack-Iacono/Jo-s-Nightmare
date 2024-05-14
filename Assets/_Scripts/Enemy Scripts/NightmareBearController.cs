using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.AI;

public class NightmareBearController : NightmareController
{
    [Header("Unique Bear Characteristics")]
    public float restTime = 2;
    private float currentRestTime = -1;
    public float moveTime = 5;
    private float currentMoveTime = -1;

    private bool isResting;
    private PlayerController playerCont;

    public override void RunBehaviorTree()
    {
        base.RunBehaviorTree();
    }

    public override void StartOverrides()
    {
        spawnLocation = NightSpawnController.bearSpawnPosition;
        currentRestTime = restTime;
        currentMoveTime = moveTime;

        base.StartOverrides();

        playerCont = FindObjectOfType<PlayerController>();
    }
    public override void SpawnSetup()
    {
        spawnLocation = NightSpawnController.bearSpawnPosition;
        targetLocation = playerCont.transform.position;

        InterfaceController.Instance.HUDMessage("You hear a feint growling");

        SoundManager.PlaySound(SoundManager.growl);

        base.SpawnSetup();
    }

    #region Target Nodes

    public override Node.Status TargetMove()
    {
        // Sets up the resting cycle
        if (isResting)
        {
            navAgent.isStopped = true;

            if (currentRestTime > 0)
                currentRestTime -= Time.deltaTime;
            else
            {
                currentMoveTime = moveTime;
                isResting = false;
            }
        }
        else
        {
            navAgent.isStopped = false;

            if (currentMoveTime > 0)
                currentMoveTime -= Time.deltaTime;
            else
            {
                currentRestTime = restTime;
                isResting = true;
            }
        }

        // Check if a honey pot is active, if not, go after the player
        if (HoneyPotController.activeHoneyPots.Count > 0)
        {
            targetLocation = HoneyPotController.activeHoneyPots[0].transform.position;

            // Go After the Honey Pot
            if (base.TargetMove() == Node.Status.SUCCESS)
            {
                HoneyPotController.activeHoneyPots[0].HoneyDestroy();
                SoundManager.PlaySound(SoundManager.bearHit);
            }
        }
        else
        {
            // If the move has completed, attack player
            targetLocation = PlayerController.playerInstances[0].transform.position;

            if (base.TargetMove() == Node.Status.SUCCESS)
            {
                //Attack player
                playerCont.BearAttack();

                SoundManager.PlaySound(SoundManager.bearHit);

                navAgent.isStopped = false;

                return Node.Status.SUCCESS;
            }
        }

        return Node.Status.RUNNING;
    }

    #endregion

    #region Bed Nodes

    public override Node.Status BedMove()
    {
        return Node.Status.SUCCESS;
    }
    public override Node.Status BedMoveDelay()
    {
        return Node.Status.SUCCESS;
    }
    public override Node.Status EndBedMove()
    {
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Retreat Nodes

    public override Node.Status RetreatMove()
    {
        return Node.Status.SUCCESS;
    }
    public override Node.Status RetreatMoveDelay()
    {
        return Node.Status.SUCCESS;
    }

    #endregion

    public override Node.Status SpawnMove()
    {
        navAgent.isStopped = false;
        return base.SpawnMove();
    }

    public override void PlayerDamageSwing()
    {
        //Do Nothing
        SoundManager.PlaySound(SoundManager.growl);
    }
    public override void PlayerDamageShoot()
    {
        //Same thing as swing, ignore the stun
        PlayerDamageSwing();
    }

    public override bool CheckPath()
    {
        targetLocation = playerCont.transform.position;
        return base.CheckPath();
    }

}
