using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private bool headOff = false;
    private GameObject head;

    public override void Initialize()
    {
        base.Initialize();

        headlessGameObject.SetActive(false);

        head = Instantiate(headObject);
        head.GetComponent<NightmareSkeletonHeadController>().Initialize(this);
        head.SetActive(false);
    }
    protected override Node SetupTree()
    {
        navAgent = GetComponent<NavMeshAgent>();

        // Establises the Behavior Tree and its logic
        Node root = new Selector(new List<Node>()
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerInSight(this, PlayerController.playerInstances[0].gameObject.transform),
                new TaskChasePlayer(transform, navAgent),
                new CheckArea(this, PlayerController.playerInstances[0].gameObject.transform)
            }),
            new Sequence(new List<Node>
            {
                new CheckHunger(this),
                new TaskGoToTarget(transform, navAgent),
                new TaskEat(this)
            }),
            new TaskPatrol(transform, NightmareSpawnController.Instance.skeletonPatrolPoints, navAgent)
        });

        root.SetData("speed", moveSpeed);

        return root;
    }

    private void SetHeadOffStatus(bool b)
    {
        if(b && !headOff)
        {
            head.SetActive(true);
            head.GetComponent<Rigidbody>().isKinematic = false;
            head.transform.position = transform.position + headSpawnOffset;

            head.GetComponent<Rigidbody>().AddForce(PlayerController.playerInstances[0].transform.forward * 10, ForceMode.Force);

            headOff = true;

            headlessGameObject.SetActive(true);
            headGameObject.SetActive(false);
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

        base.PlayerDamageSwing();
    }

    public override bool CheckPath()
    {
        targetLocation = PlayerController.playerInstances[0].transform.position;
        return base.CheckPath();
    }
}
