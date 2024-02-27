using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NightmareControllerNew : MonoBehaviour
{
    [Header("Nightmare Characteristics")]
    public float moveSpeed = 10;
    public float health = 100;

    // Timers for actions
    public float targetDelay = 1;
    public float bedDelay = 1;
    public float retreatDelay = 5;
    public float stunDelay = 10;

    private float currentTime = 0;
    private string currentTimerOwner = null;

    [Header("NavmMesh Objects")]
    public float destinationDist = 0.5f;
    [NonSerialized]
    public NavMeshAgent navAgent;

    //Get rid of public after testing
    public Vector3 spawnLocation;
    private Vector3 targetLocation = Vector3.zero;

    public string bufferedSequence { get; private set; }

    #region Nodes

    Node.Status status = Node.Status.RUNNING;

    public Tree behaviorTree;

    public Sequence targetActions;
    public Sequence retreatActions;
    public Sequence bedActions;
    public Sequence stunActions;

    #endregion

    #region Initialization
    public void Initialize()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = moveSpeed;

        //GameController.RegisterNightmare(this);

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

        stunActions = new Sequence("Stun");
        stunActions.AddChild(new Action("BeginStun", BeginStun));
        stunActions.AddChild(new Action("StunDelay", StunDelay));
        stunActions.AddChild(new Action("EndStun", EndStun));

        //Set the sequences inside the tree
        behaviorTree.AddChild(targetActions);
        behaviorTree.AddChild(retreatActions);
        behaviorTree.AddChild(bedActions);
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
        behaviorTree.StartSequence("Move");
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
                case "Move":
                    behaviorTree.StartSequence("Target");
                    break;
                case "Target":
                    behaviorTree.StartSequence("Escape");
                    break;
                case "Retreat":
                    if (navAgent.isOnNavMesh)
                        behaviorTree.StartSequence("Move");
                    break;
                case "Stun":
                    behaviorTree.StartSequence(bufferedSequence);
                    break;
                default:
                    behaviorTree.StartSequence("Move");
                    break;
            }

            status = behaviorTree.Check();
        }
    }


    #region Move to Target

    public Node.Status BeginTargetMove()
    {
        // Modify to change the action performed when entering this state
        return Node.Status.SUCCESS;
    }
    public Node.Status TargetMove()
    {
        // Modify to change the actions when moving to the target location
        return MoveTo(targetLocation);
    }
    public Node.Status TargetMoveDelay()
    {
        // Modify this to change the time of delay for this action
        return Node.Status.SUCCESS;
    }
    public Node.Status EndTargetMove()
    {
        // Modify to change the action performed when exiting this state
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Move to Bed

    public Node.Status BeginBedMove()
    {
        // Modify to change action when beginning to move to bed
        return Node.Status.SUCCESS;
    }
    public Node.Status BedMove()
    {
        // Modify to change the actions when moving to the target location
        return MoveTo(NightSpawnController.bedPosition);
    }
    public Node.Status BedMoveDelay()
    {
        // Modify this to change the time of delay for this action
        return Delay(bedDelay);
    }
    public Node.Status EndBedMove()
    {
        // Modify to change the action performed when exiting this state
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Move to Retreat

    public Node.Status BeginRetreatMove()
    {
        // Modify to change action when beginning to move to bed
        return Node.Status.SUCCESS;
    }
    public Node.Status RetreatMove()
    {
        // Modify to change the actions when moving to the target location
        return MoveTo(targetLocation);
    }
    public Node.Status RetreatMoveDelay()
    {
        // Modify this to change the time of delay for this action
        return Delay(retreatDelay);
    }
    public Node.Status EndRetreatMove()
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

    #endregion

    public virtual void PlayerDamageSwing()
    {
        behaviorTree.StartSequence("Retreat");
    }
    public virtual void PlayerDamageShoot()
    {
        bufferedSequence = behaviorTree.GetCurrentSequenceName();
        behaviorTree.StartSequence("Stun");
    }

    public void GamePause()
    {
        if (GameController.isPaused)
        {
            navAgent.isStopped = true;
        }
        else
        {
            navAgent.isStopped = false;
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
