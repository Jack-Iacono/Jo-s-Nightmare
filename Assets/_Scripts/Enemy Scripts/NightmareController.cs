using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NightmareController : MonoBehaviour
{
    [Header("Nightmare Characteristics")]
    public float moveSpeed = 10;
    public float health = 100;

    // Timers for actions
    public float targetDelay = 1;
    public float bedDelay = 1;
    public float retreatDelay = 5;
    public float spawnDelay = 1;
    public float stunDelay = 10;

    protected float currentTime = 0;
    protected string currentTimerOwner = null;

    [Header("NavMesh Objects")]
    public float destinationDist = 0.5f;
    [NonSerialized]
    public NavMeshAgent navAgent;

    protected Vector3 spawnLocation;
    protected Vector3 targetLocation = Vector3.zero;

    protected string bufferedSequence;

    private bool wasPaused = false;

    #region Nodes

    protected Node.Status status = Node.Status.RUNNING;

    protected Tree behaviorTree;

    protected Sequence targetActions;
    protected Sequence retreatActions;
    protected Sequence bedActions;
    protected Sequence stunActions;
    protected Sequence spawnActions;

    #endregion

    #region Initialization
    public void Initialize()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = moveSpeed;

        GameController.RegisterNightmare(this);

        #region Behavior Tree
        //Initialize the behavior tree
        behaviorTree = new Tree("NightmareBehavior");

        //Initializing the nodes
        targetActions = new Sequence("Target");
        targetActions.AddChild(new Action("BeginTargetMove", BeginTargetMove));
        targetActions.AddChild(new Action("TargetMove", TargetMove));
        targetActions.AddChild(new Action("TargetMoveDelay", TargetMoveDelay));
        targetActions.AddChild(new Action("EndTargetMove", EndTargetMove));

        retreatActions = new Sequence("Retreat");
        retreatActions.AddChild(new Action("BeginRetreatMove", BeginRetreatMove));
        retreatActions.AddChild(new Action("RetreatMove", RetreatMove));
        retreatActions.AddChild(new Action("RetreatMoveDelay", RetreatMoveDelay));
        retreatActions.AddChild(new Action("EndRetreatMove", EndRetreatMove));

        bedActions = new Sequence("Bed");
        bedActions.AddChild(new Action("BeginBedMove", BeginBedMove));
        bedActions.AddChild(new Action("BedMove", BedMove));
        bedActions.AddChild(new Action("BedMoveDelay", BedMoveDelay));
        bedActions.AddChild(new Action("EndBedMove", EndBedMove));

        spawnActions = new Sequence("Spawn");
        spawnActions.AddChild(new Action("BeginSpawnMove", BeginSpawnMove));
        spawnActions.AddChild(new Action("SpawnMove", SpawnMove));
        spawnActions.AddChild(new Action("SpawnMoveDelay", SpawnMoveDelay));
        spawnActions.AddChild(new Action("EndSpawnMove", EndSpawnMove));

        stunActions = new Sequence("Stun");
        stunActions.AddChild(new Action("BeginStun", BeginStun));
        stunActions.AddChild(new Action("StunDelay", StunDelay));
        stunActions.AddChild(new Action("EndStun", EndStun));

        //Set the sequences inside the tree
        behaviorTree.AddChild(targetActions);
        behaviorTree.AddChild(retreatActions);
        behaviorTree.AddChild(bedActions);
        behaviorTree.AddChild(spawnActions);
        behaviorTree.AddChild(stunActions);

        #endregion

        behaviorTree.StartSequence("Target");

        StartOverrides();

        navAgent.Warp(spawnLocation);
    }
    public virtual void SpawnSetup()
    {
        //Spawn Stuff
        navAgent.Warp(spawnLocation);
        behaviorTree.StartSequence("Target");
    }
    public virtual void StartOverrides()
    {
        //Child Classes can use this to change the node bahaviors as they need to
    }

    #endregion

    void Update()
    {
        if (!GameController.isPaused)
        {
            RunBehaviorTree();
        }
    }
    public virtual void RunBehaviorTree()
    {
        //controls the behavior of the method
        if (status == Node.Status.RUNNING)
        {
            status = behaviorTree.Check();
        }
        else if (status == Node.Status.SUCCESS)
        {
            // Resets the timer for movement
            currentTime = 0;

            switch (behaviorTree.GetCurrentSequenceName())
            {
                case "Target":
                    behaviorTree.StartSequence("Bed");
                    break;
                case "Bed":
                    behaviorTree.StartSequence("Retreat");
                    break;
                case "Retreat":
                    behaviorTree.StartSequence("Spawn");
                    break;
                case "Spawn":
                    behaviorTree.StartSequence("Target");
                    break;
                case "Stun":
                    behaviorTree.StartSequence(bufferedSequence);
                    break;
                default:
                    behaviorTree.StartSequence("Target");
                    break;
            }

            status = behaviorTree.Check();
        }
    }


    #region Move to Target

    public virtual Node.Status BeginTargetMove()
    {
        // Modify to change the action performed when entering this state
        return Node.Status.SUCCESS;
    }
    public virtual Node.Status TargetMove()
    {
        // Modify to change the actions when moving to the target location
        return MoveTo(targetLocation);
    }
    public virtual Node.Status TargetMoveDelay()
    {
        // Modify this to change the time of delay for this action
        return Delay(targetDelay);
    }
    public virtual Node.Status EndTargetMove()
    {
        // Modify to change the action performed when exiting this state
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Move to Bed

    public virtual Node.Status BeginBedMove()
    {
        // Modify to change action when beginning to move to bed
        return Node.Status.SUCCESS;
    }
    public virtual Node.Status BedMove()
    {
        // Modify to change the actions when moving to the target location
        return MoveTo(NightSpawnController.bedPosition);
    }
    public virtual Node.Status BedMoveDelay()
    {
        // Modify this to change the time of delay for this action
        return Delay(bedDelay);
    }
    public virtual Node.Status EndBedMove()
    {
        // Modify to change the action performed when exiting this state
        GameController.DestroyLight();

        return Node.Status.SUCCESS;
    }

    #endregion

    #region Move to Retreat

    public virtual Node.Status BeginRetreatMove()
    {
        // Modify to change action when beginning to move to bed
        return Node.Status.SUCCESS;
    }
    public virtual Node.Status RetreatMove()
    {
        // Modify to change the actions when moving to the target location
        return MoveTo(targetLocation);
    }
    public virtual Node.Status RetreatMoveDelay()
    {
        // Modify this to change the time of delay for this action
        return Delay(retreatDelay);
    }
    public virtual Node.Status EndRetreatMove()
    {
        // Modify to change the action performed when exiting this state
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Move to Spawn

    public virtual Node.Status BeginSpawnMove()
    {
        // Modify to change action when beginning to move to bed
        return Node.Status.SUCCESS;
    }
    public virtual Node.Status SpawnMove()
    {
        // Modify to change the actions when moving to the target location
        return MoveTo(spawnLocation);
    }
    public virtual Node.Status SpawnMoveDelay()
    {
        // Modify this to change the time of delay for this action
        return Delay(spawnDelay);
    }
    public virtual Node.Status EndSpawnMove()
    {
        // Modify to change the action performed when exiting this state
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Stun

    public virtual Node.Status BeginStun()
    {
        // Do Nothing Yet
        navAgent.isStopped = true;

        return Node.Status.SUCCESS;
    }
    public virtual Node.Status StunDelay()
    {
        return Delay(stunDelay);
    }
    public virtual Node.Status EndStun()
    {
        // Do Nothing Yet
        navAgent.isStopped = false;

        return Node.Status.SUCCESS;
    }

    #endregion

    #region Misc Nodes

    public Node.Status MoveTo(Vector3 location)
    {
        if (MyFunctions.Distance(navAgent.destination, location) > destinationDist)
            navAgent.destination = location;

        if (MyFunctions.Distance(transform.position, location) < destinationDist)
        {
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING;
    }
    public Node.Status Dormant()
    {
        return Node.Status.RUNNING;
    }
    public Node.Status Delay(float f)
    {
        if (currentTimerOwner != behaviorTree.GetCurrentSequenceName())
        {
            currentTime = 0;
            currentTimerOwner = behaviorTree.GetCurrentSequenceName();
        }

        if (currentTime < f)
        {
            currentTime += Time.deltaTime;
            return Node.Status.RUNNING;
        }

        return Node.Status.SUCCESS;
    }
    public void ResetDelay()
    {
        // Resets the delay manually
        currentTimerOwner = "";
        currentTime = -1;
    }

    #endregion

    public virtual void PlayerDamageSwing()
    {
        // Play Hit Noise
        SoundManager.PlaySound(SoundManager.bonk);
    }
    public virtual void PlayerDamageShoot()
    {
        // Play Shoot Noise
        SoundManager.PlaySound(SoundManager.bonk);

        bufferedSequence = behaviorTree.GetCurrentSequenceName();
        behaviorTree.StartSequence("Stun");
    }

    public void GamePause()
    {
        if (GameController.isPaused)
        {
            wasPaused = navAgent.isStopped;
            navAgent.isStopped = true;
        }
        else
        {
            navAgent.isStopped = wasPaused;
        }
    }

    public virtual bool CheckPath()
    {
        NavMeshPath path = new NavMeshPath();

        if (navAgent.CalculatePath(targetLocation, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            return true;
        }

        return false;
    }
}
