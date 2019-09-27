using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(0)]
public abstract class GameCharacter : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    public Slider healthBar;

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        healthBar.value = currentHealth / maxHealth;
        healthBar.gameObject.SetActive(true);
    }

    public void Damage(float damage)
    {
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        float oldHealth = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, currentHealth);
        if (currentHealth < oldHealth)
            OnDamaged(oldHealth - currentHealth);
        healthBar.value = currentHealth / maxHealth;
        if (currentHealth == 0) OnDie();
    }

    protected virtual void OnDie()
    {
        Debug.Log($"{gameObject.name} character die");
        healthBar.gameObject.SetActive(false);
    }

    protected virtual void OnDamaged(float damage)
    {
    }
}