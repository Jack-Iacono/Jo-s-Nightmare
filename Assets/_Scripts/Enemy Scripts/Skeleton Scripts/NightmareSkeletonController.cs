using BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class NightmareSkeletonController : Nightmare
{
    [Header("Unique Skeleton Characteristics")]
    public GameObject headObject;
    public Vector3 headSpawnOffset;

    [Space]
    public GameObject headGameObject;
    public Animator headAnimator;

    [Space]
    public GameObject headlessGameObject;
    public Animator headlessAnimator;

    protected BehaviorTree.Tree behaviorTree;

    private TimerManager timerManager = new TimerManager();
    private Timer headPickupTimer;

    public bool headOff { get; private set; } = false;
    public bool headPickupAllowed { get; private set; } = true;

    public NightmareSkeletonHeadController head { get; private set; }

    public override void Initialize()
    {
        base.Initialize();

        headPickupTimer = new Timer(1, HeadPickupTimerCallback);
        timerManager.Add(headPickupTimer.ToString(), headPickupTimer);

        head = Instantiate(headObject).GetComponent<NightmareSkeletonHeadController>();
        head.GetComponent<NightmareSkeletonHeadController>().Initialize(this);
        head.gameObject.SetActive(false);

        behaviorTree = new SkeletonBT(this);
        behaviorTree.Initialize();
        currentTree = behaviorTree;

        headlessGameObject.SetActive(false);
    }

    protected override void Update()
    {
        if (!GameController.isPaused)
        {
            float dt = Time.deltaTime;

            timerManager.IncrementTimers(dt);
            currentTree.UpdateTree(dt);
        }
    }

    public void SetHeadOffStatus(bool b)
    {
        if(b && !headOff)
        {
            head.transform.position = transform.position + Vector3.up;
            headOff = true;

            headPickupTimer.Start();
            headPickupAllowed = false;

            head.StartHead();

            headlessGameObject.SetActive(true);
            headGameObject.SetActive(false);
        }
        else if(!b)
        {
            SoundManager.PlayRandomSound(SoundManager.skeletonWalking);

            // Make the head on the floor disappear
            head.gameObject.SetActive(false);

            headlessGameObject.SetActive(false);
            headGameObject.SetActive(true);

            //Put head back on
            headOff = false;
        }
    }

    public void HeadPickupTimerCallback(string key)
    {
        headPickupAllowed = true;
    }

    public override void PlayerDamageSwing()
    {
        if (!headOff)
        {
            SoundManager.PlayRandomSound(SoundManager.skeletonWalking);
            SetHeadOffStatus(true);
        }

        base.PlayerDamageSwing();
    }

    public override bool CheckPath()
    {
        targetLocation = PlayerController.playerInstances[0].transform.position;
        return base.CheckPath();
    }
}
