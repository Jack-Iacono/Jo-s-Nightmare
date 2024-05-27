using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.AI;

public class NightmareBearController : Nightmare
{
    [Header("Unique Bear Characteristics")]
    public float restTime = 2;
    private float currentRestTime = -1;
    public float moveTime = 5;
    private float currentMoveTime = -1;

    private bool isResting;
    private PlayerController playerCont;

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

    protected override Node SetupTree()
    {
        throw new System.NotImplementedException();
    }
}
