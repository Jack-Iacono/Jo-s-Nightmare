using System;
using UnityEngine;
using UnityEngine.AI;

using BehaviorTree;
using UnityEditor.Timeline;

public abstract class Nightmare : BehaviorTree.Tree
{
    [Header("Nightmare Characteristics")]
    [SerializeField]
    protected float moveSpeed = 10;
    public float fovRange = 10;

    [Header("NavMesh Objects")]
    [NonSerialized]
    public NavMeshAgent navAgent;

    protected Vector3 spawnLocation;
    protected Vector3 targetLocation = Vector3.zero;

    #region Initialization

    public virtual void Initialize()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = moveSpeed;

        GameController.RegisterNightmare(this);
        GameController.OnGamePause += OnGamePause;

        navAgent.Warp(spawnLocation);
    }
    public virtual void Spawn()
    {
        //Spawn Stuff
        navAgent.Warp(spawnLocation);
    }

    #endregion

    protected override void Update()
    {
        if (!GameController.isPaused)
        {
            base.Update();
        }
    }

    public virtual void PlayerDamageSwing() { }
    public virtual void PlayerDamageShoot() { }

    protected virtual void OnGamePause(object sender, bool e)
    {
        if(navAgent.isOnNavMesh)
            navAgent.isStopped = e;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fovRange);
    }
}
