using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class AttackController : MonoBehaviour
{
    [NonSerialized]
    public PlayerController playerCont;

    private const int ATTACK_COUNT = 2;
    [SerializeField]
    public Attack[] attacks = new Attack[ATTACK_COUNT];

    private KeyCode keyAttack1 = KeyCode.Mouse0;
    private KeyCode keyAttack2 = KeyCode.Mouse1;

    private int activeAttack = -1;

    // Start is called before the first frame update
    void Start()
    {
        playerCont = GetComponent<PlayerController>();

        for(int i = 0; i < attacks.Length; i++)
        {
            attacks[i] = Instantiate(attacks[i]);
        }

        foreach(Attack a in attacks)
        {
            a.Initialize(this);
            a.OnAttackEnd += OnAttackEnd;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.isPaused)
        {
            if(activeAttack < 0)
            {
                if (Input.GetKeyDown(keyAttack1))
                {
                    UseAttack(0);
                }
                else if (Input.GetKeyDown(keyAttack2))
                {
                    UseAttack(1);
                }
            }
            else
            {
                attacks[activeAttack].AttackUpdate(Time.deltaTime);
            }
        }
    }

    private void UseAttack(int index)
    {
        playerCont.anim.SetTrigger(attacks[index].animationTrigger);
        attacks[index].BeginAttack();
        activeAttack = index;
    }

    private void OnAttackEnd()
    {
        activeAttack = -1;
    }
    public void SwapAttack(int index, Attack attack)
    {
        attacks[index].OnAttackEnd -= OnAttackEnd;
        attacks[index] = attack;
        attacks[index].OnAttackEnd += OnAttackEnd;
    }
}
