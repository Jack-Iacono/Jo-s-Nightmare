using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareSkeletonController : NightmareController
{
    [Header("Unique Skeleton Characteristics")]
    public GameObject headObject;
    public Vector3 headSpawnOffset;
    public float hitStunTime;
    private float currentHitStunTimer;

    [Space]
    public GameObject headGameObject;
    public Animator headAnimator;

    [Space]
    public GameObject headlessGameObject;
    public Animator headlessAnimator;

    public PlayerController playerCont { get; private set; }
    private bool headOff = false;
    private GameObject head;

    public override void StartOverrides()
    {
        spawnLocation = NightSpawnController.skeletonSpawnPosition;
        head = Instantiate(headObject);
        head.SendMessage("Initialize", this);
        head.SetActive(false);
        base.StartOverrides();

        playerCont = FindObjectOfType<PlayerController>();
    }
    public override void SpawnSetup()
    {
        spawnLocation = NightSpawnController.skeletonSpawnPosition;
        targetLocation = head.transform.position;

        InterfaceController.Instance.HUDMessage("You hear bones rattling");

        headlessGameObject.SetActive(false);
        headGameObject.SetActive(true);

        SoundManager.PlayRandomSound(SoundManager.skeletonWalking);

        base.SpawnSetup();
    }

    #region Target Nodes
    public override Node.Status TargetMove()
    {
        // Will check if the head is on and then go to it if it is not

        if (!headOff)
            return Node.Status.SUCCESS;
        else
            return MoveToHead();
    }
    public override Node.Status TargetMoveDelay()
    {
        // If this head is off, make a delay after getting to the head

        if (headOff)
        {
            head.GetComponent<NightmareSkeletonHeadController>().Stop();
            headlessAnimator.SetBool("headpickup", true);

            Node.Status status = base.TargetMoveDelay();
            
            if(status == Node.Status.SUCCESS)
            {
                SetHeadOffStatus(false);
                headlessAnimator.SetBool("headpickup", false);
            }

            return status;
        }
        else
            return Node.Status.SUCCESS;
    }
    #endregion

    #region Bed Nodes

    public override Node.Status BedMove()
    {
        // Will go back to the target behavior if the head comes off

        if (headOff)
        {
            behaviorTree.StartSequence("Target");
            return Node.Status.RUNNING;
        }
        else
        {
            Node.Status status = base.BedMove();

            if(status == Node.Status.SUCCESS)
            {
                headAnimator.SetBool("isBreaking", true);
            }

            return status;
        }
    }

    #endregion

    #region Retreat Nodes

    public override Node.Status RetreatMove()
    {
        headAnimator.SetBool("isBreaking", false);

        return TargetMove();
    }
    public override Node.Status RetreatMoveDelay()
    {
        return TargetMoveDelay();
    }

    #endregion

    #region Spawn Nodes

    public override Node.Status SpawnMove()
    {
        // Will go back to the retreat behavior if the head comes off

        if (headOff)
        {
            behaviorTree.StartSequence("Retreat");
            return Node.Status.RUNNING;
        }
        else
            return base.SpawnMove();
    }

    #endregion

    private Node.Status MoveToHead()
    {
        if (currentHitStunTimer > 0)
        {
            currentHitStunTimer -= Time.deltaTime;
        }
        else
        {
            currentHitStunTimer = -1;
            navAgent.isStopped = false;

            // Check to see if skeleton is at the head
            if (MoveTo(head.transform.position) == Node.Status.SUCCESS)
            {
                return Node.Status.SUCCESS;
            }
        }

        return Node.Status.RUNNING;
    }
    private void SetHeadOffStatus(bool b)
    {
        if(b && !headOff)
        {
            head.SetActive(true);
            head.GetComponent<Rigidbody>().isKinematic = false;
            head.transform.position = transform.position + headSpawnOffset;

            head.GetComponent<Rigidbody>().AddForce(playerCont.transform.forward * 10, ForceMode.Force);

            currentHitStunTimer = hitStunTime;
            navAgent.isStopped = true;
            headOff = true;

            headlessGameObject.SetActive(true);
            headGameObject.SetActive(false);

            if(behaviorTree.GetCurrentSequenceName() == "Bed")
                behaviorTree.StartSequence("Target");
            else if(behaviorTree.GetCurrentSequenceName() == "Spawn")
                behaviorTree.StartSequence("Retreat");

        }
        else if(!b)
        {
            SoundManager.PlayRandomSound(SoundManager.skeletonWalking);

            // Make the head on the floor disappear
            head.SetActive(false);

            headlessGameObject.SetActive(false);
            headGameObject.SetActive(true);

            //Put head back on
            headOff = false;
            head.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public override void PlayerDamageSwing()
    {
        if (!headOff)
        {
            SoundManager.PlayRandomSound(SoundManager.skeletonWalking);
            SetHeadOffStatus(true);
        }

        if (behaviorTree.GetCurrentSequenceName() == "Stun")
            behaviorTree.StartSequence(bufferedSequence);

        base.PlayerDamageSwing();
    }

    public override bool CheckPath()
    {
        targetLocation = playerCont.transform.position;
        return base.CheckPath();
    }
}
