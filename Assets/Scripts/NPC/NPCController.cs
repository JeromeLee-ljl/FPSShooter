using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum MonsterType
{
    Monster1,
    Monster2,
}

[RequireComponent(typeof(Rigidbody))]
public class NPCController : GameCharacter
{
    public Sword sword;
    public Transform[] patrolPath;
    public MonsterType monsterType = MonsterType.Monster1;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float viewDistance = 7;
    public float viewAngle = 120;
    public float attackDistance = 2f;
    public float score;
//    public Color color = Color.white;

    public NavMeshAgent Agent { get; private set; }
    public GameObject Player { get; private set; }
    public Animator Animator { get; private set; }


    protected void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindWithTag("Player");
        Animator = GetComponentInChildren<Animator>();
        
        MakeFsm();
    }
    

    private void Start()
    {
        LoadConfig();
        fsm.ChangeStateTo<NPCPatrollingState>();
    }

    private void LoadConfig()
    {
        var jsonConfig = ConfigsManager.Instance.GetConfig<JsonConfig<MonstersConfig>>();
        MonsterData data;
        if (monsterType == MonsterType.Monster1)
            data = jsonConfig.Data.monster1;
        else
            data = jsonConfig.Data.monster2;

        maxHealth = data.maxHealth;
        walkSpeed = data.walkSpeed;
        runSpeed = data.runSpeed;
        viewDistance = data.viewDistance;
        viewAngle = data.viewAngle;
        attackDistance = data.attackDistance;
        score = data.score;
        
        currentHealth = maxHealth;
    }

    public void SetMonsterType(MonsterType type)
    {
        monsterType = type;
        LoadConfig();

        fsm.ChangeStateTo<NPCPatrollingState>();
    }

    private bool death;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        death = false;
    }
    private void FixedUpdate()
    {
        if (death) return;
        if (currentHealth == 0)
        {
            death = true;
            fsm.ForceChangeState<NPCDeathState>();
            return;
        }

        fsm.CurrentState.Reason();
        fsm.CurrentState.Update();
        Animator.SetFloat("Speed", Agent.speed);
    }

    private FSMSystem fsm;

    private void MakeFsm() //建造状态机
    {
        fsm = new FSMSystem();

        fsm.AddState(new NPCPatrollingState(this))
            .AddTransitionTo<NPCChasePlayerState>();

        fsm.AddState(new NPCChasePlayerState(this))
            .AddTransitionTo<NPCPatrollingState>()
            .AddTransitionTo<NPCAttackPlayerState>();

        fsm.AddState(new NPCAttackPlayerState(this))
            .AddTransitionTo<NPCChasePlayerState>();

        fsm.AddState(new NPCDeathState(this))
            .AddTransitionTo<NPCPatrollingState>();
    }

    protected override void OnDie()
    {
        base.OnDie();
        GameProcessManager.Instance.score += score;
    }
}