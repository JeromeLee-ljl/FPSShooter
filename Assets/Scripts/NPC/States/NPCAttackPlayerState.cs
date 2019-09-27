using System;
using System.Collections;
using UnityEngine;

public class NPCAttackPlayerState : FSMNPCState
{
    public NPCAttackPlayerState(NPCController npc) : base(npc)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        npc.Agent.isStopped = true;
        attacking = false;
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
        npc.Agent.isStopped = false;
    }

    public override void Reason()
    {
        if (!PlayerInAttackRange()) // 超出攻击范围
        {
            FsmSystem.ChangeStateTo<NPCChasePlayerState>();
        }
    }

    public override void Update()
    {
        if (attacking) return;
        if (!PlayerInView())
        {
            npc.transform.Rotate(npc.transform.up * 2);
            return;
        }

        attacking = true;
        npc.StartCoroutine(Attack());
    }


    private bool attacking;

    private IEnumerator Attack()
    {
        npc.Animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.9f);
        npc.sword.Attack(() => { });
        yield return new WaitForSeconds(0.3f);
        npc.sword.SetDotAttack();
        yield return new WaitForSeconds(1.8f);
        attacking = false;
    }
}