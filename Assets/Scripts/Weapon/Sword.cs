using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    public float damage;
    // 激活时可造成伤害
    private bool active;

    private void Start()
    {
        damage = ConfigsManager.Instance.GetConfig<JsonConfig<WeaponsConfig>>().Data.sword.damage;
    }

    private void OnEnable()
    {
        active = false;
    }

    public override void Attack(Action success)
    {
        if(active) return;
        active = true;
        success();
        onceWaveAttack.Clear();
    }

    public void SetDotAttack()
    {
        active = false;
    }


    private HashSet<GameObject> onceWaveAttack = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        
        if (onceWaveAttack.Contains(other.gameObject)) return;
        GameCharacter character = other.GetComponent<GameCharacter>();
        if (character == null) return;
        Debug.Log($"sword attack {other.name}");
        onceWaveAttack.Add(other.gameObject);
        character.Damage(damage);
    }
    
}