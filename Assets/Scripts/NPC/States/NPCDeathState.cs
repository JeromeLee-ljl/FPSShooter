using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDeathState : FSMNPCState
{
    public NPCDeathState(NPCController npc) : base(npc)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        npc.Animator.SetTrigger("Die");
        npc.GetComponent<Collider>().enabled = false;
        npc.Agent.enabled = false;
        npc.StartCoroutine(Recycle(2.5f));
    }

    private IEnumerator Recycle(float time)
    {
        yield return new WaitForSeconds(time);
        
        PoolsManager.Instance.Recycle(npc.gameObject);
    }

    public override void OnStateExit()
    {
        base.OnStateExit();
        npc.Agent.enabled = true;
        npc.GetComponent<Collider>().enabled = true;
        
    }

    // Update is called once per frame
    public override void Reason()
    {
    }

    public override void Update()
    {
    }
}