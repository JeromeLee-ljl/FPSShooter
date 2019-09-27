using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawn : MonoBehaviour
{
    public Transform[] patrolPath;

    [Tooltip("刚开始等待多少秒后生成敌人")] public float startTime = 2f;
    [Tooltip("敌人死亡后等待多少秒后再生成敌人")] public float waitTime = 2f;
    public MonsterType type;
    private NPCController npc;

    // Start is called before the first frame update
    void Start()
    {
        isWaitting = true;
        StartCoroutine(SpawnNPCWait(startTime));
    }

    private bool isWaitting;

    // Update is called once per frame
    void Update()
    {
        if (isWaitting) return;
        if (npc == null || !npc.gameObject.activeSelf)
        {
            isWaitting = true;
            StartCoroutine(SpawnNPCWait(waitTime));
        }
    }

    IEnumerator SpawnNPCWait(float second)
    {
        yield return new WaitForSeconds(second);

        npc = PoolsManager.Instance.Get<NPCController>("Monster");
        npc.transform.position = transform.position;
        npc.patrolPath = patrolPath;
        npc.SetMonsterType(type);

        isWaitting = false;
    }
}