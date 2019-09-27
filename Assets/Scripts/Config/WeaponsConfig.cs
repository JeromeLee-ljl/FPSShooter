using System;

/// <summary>
/// 武器配置信息的实体类
/// </summary>
[Serializable]
public class WeaponsConfig : IJsonConfigEntity
{
    public GunData autoGun;
    public GunData semiGun;
    public SwordData sword;

    public override string ToString()
    {
        return $@"{GetType()}: {{
    autoGUn: {autoGun},
    semiGun: {semiGun},
    sword: {sword}
}}";
    }
}

[Serializable]
public class GunData
{
    public float fireInterval;
    public float reloadTime;
    public float recoilForce;
    public float shakeForce;
    public int bulletCount;
    public int capacity;
    public float damage;
    public float speed;

    public override string ToString()
    {
        return $@"{GetType()}: {{
    fireInterval: {fireInterval},
    reloadTime: {reloadTime},
    recoilForce: {recoilForce},
    shakeForce: {shakeForce},
    bulletCount: {bulletCount},
    capacity: {capacity},
    damage: {damage},
    speed: {speed}
}}";
    }
}

[Serializable]
public class SwordData
{
    public float damage;

    public override string ToString()
    {
        return $@"{GetType()}: {{
    damage: {damage}
}}";
    }
}