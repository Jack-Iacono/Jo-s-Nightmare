using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Attack : ScriptableObject
{
    [Header("Attack Attributes")]
    public LayerMask attackLayers;
    public float attackRange;

    protected float currentAttackTimer;
    public float attackDuration;

    protected int currentUseCount;
    public int useCount;

    public string animationTrigger;

    protected AttackController controller;

    protected bool isExecuting;

    public delegate void AttackEnd();
    public event AttackEnd OnAttackEnd;

    public virtual void Initialize(AttackController controller)
    {
        this.controller = controller;

        currentAttackTimer = attackDuration;
        currentUseCount = useCount;
    }

    public virtual void AttackUpdate(float deltaTime)
    {
        if (isExecuting)
        {
            if (currentAttackTimer > 0)
                currentAttackTimer -= deltaTime;
            else
            {
                currentAttackTimer = attackDuration;
                EndAttack();
            }
        }
    }
    public virtual void BeginAttack() { isExecuting = true; }
    public virtual void EndAttack() 
    { 
        isExecuting= false;
        OnAttackEnd?.Invoke();
    }
}
