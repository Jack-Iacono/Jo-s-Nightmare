using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class NightmareSpiderController : Nightmare
{
    [Header("Unique Spider Characteristics")]
    public int numberOfMoves = 4;
    public int movesRemaining = -1;

    public float currentMoveDelay = 0;
    private ParticleSystem moveParticles;

    protected override Node SetupTree()
    {
        throw new System.NotImplementedException();
    }
}
