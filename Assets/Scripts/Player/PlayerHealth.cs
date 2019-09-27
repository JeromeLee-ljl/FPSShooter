using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : GameCharacter
{
    private bool isAlive = true;

    public Animator damageMask;
    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0 && isAlive)
        {
            isAlive = false;
            GameProcessManager.Instance.EndGame();
        }
    }

    protected override void OnDamaged(float damage)
    {
        base.OnDamaged(damage);
        damageMask.SetTrigger("Damage");
    }
}
