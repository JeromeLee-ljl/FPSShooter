using UnityEngine;

/// 巡逻
public class NPCPatrollingState : FSMNPCState
{
    private int currentWayPoint;

    public NPCPatrollingState(NPCController npc) : base(npc)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        npc.Agent.speed = npc.walkSpeed;
        currentWayPoint = 0;
        SetNextDestination();
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
    }

    public override void Reason()
    {
        if (PlayerInView()) //npc看到玩家, 开始追逐玩家
        {
            FsmSystem.ChangeStateTo<NPCChasePlayerState>();
        }
    }

    //重写表现方法
    public override void Update()
    {
        Vector3 moveDir = npc.patrolPath[currentWayPoint].position - npc.transform.position;
        moveDir.y = 0;

        if (moveDir.magnitude > 1) return;
        // 巡逻下一个点
        SetNextDestination();
    }

    private void SetNextDestination()
    {
        if (npc.patrolPath == null || npc.patrolPath.Length == 0) return;
        currentWayPoint = (currentWayPoint + 1) % npc.patrolPath.Length;
        SetDestination(npc.patrolPath[currentWayPoint].position);
    }
}