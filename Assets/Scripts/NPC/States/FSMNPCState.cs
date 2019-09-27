using UnityEngine;

public abstract class FSMNPCState : FSMState
{
    protected NPCController npc;

    public FSMNPCState(NPCController npc)
    {
        this.npc = npc;
    }

    /// 玩家是否在视野范围内
    protected bool PlayerInView()
    {
        Vector3 dirToPlayer = npc.Player.transform.position - npc.transform.position;

        // 若超出视野距离
        if (dirToPlayer.sqrMagnitude > npc.viewDistance * npc.viewDistance) return false;
        // 是否超出视野范围
        return Vector3.Angle(dirToPlayer, npc.transform.forward) < npc.viewAngle / 2;
    }

    /// 玩家是否在攻击范围内
    protected bool PlayerInAttackRange()
    {
        Vector3 dirToPlayer = npc.Player.transform.position - npc.transform.position;
        return dirToPlayer.sqrMagnitude < npc.attackDistance * npc.attackDistance;
    }

    /// 设置移动目标点
    protected void SetDestination(Vector3 destination)
    {
        npc.Agent.SetDestination(destination);
    }
}