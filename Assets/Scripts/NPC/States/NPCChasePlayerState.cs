using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 巡逻状态
public class NPCChasePlayerState : FSMNPCState
{
    public NPCChasePlayerState(NPCController npc) : base(npc)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        npc.Agent.speed = npc.runSpeed;
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }

    public override void Reason()
    {
        if (!PlayerInView()) //npc视野失去玩家, 开始巡逻
        {
            FsmSystem.ChangeStateTo<NPCPatrollingState>();
        }
        else if (PlayerInAttackRange()) // 玩家进入npc攻击范围
        {
            FsmSystem.ChangeStateTo<NPCAttackPlayerState>();
        }
    }

    public override void Update()
    {
        SetDestination(npc.Player.transform.position);
    }
}