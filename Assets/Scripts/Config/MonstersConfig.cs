using System;
/// <summary>
/// 怪物配置信息的实体类
/// </summary>
[Serializable]
public class MonstersConfig : IJsonConfigEntity
{
    public MonsterData monster1;
    public MonsterData monster2;

    public override string ToString()
    {
        return $@"{GetType()}: {{
    monster1: {monster1},
    monster2: {monster2}
}}";
    }
}

[Serializable]
public class MonsterData
{
    public float maxHealth;
    public float walkSpeed;
    public float runSpeed;
    public float viewDistance;
    public float viewAngle;
    public float attackDistance;
    public float score;
    public override string ToString()
    {
        return $@"{GetType()}: {{
    maxHealth: {maxHealth},
    walkSpeed: {walkSpeed},
    runSpeed: {runSpeed},
    viewDistance: {viewDistance},
    viewAngle: {viewAngle},
    attackDistance: {attackDistance}
    score: {score}
}}";
    }
}