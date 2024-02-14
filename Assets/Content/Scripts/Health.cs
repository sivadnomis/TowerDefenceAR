using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHP;
    public Renderer healthCircle;

    private float currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    private void Update()
    {
        healthCircle.material.SetFloat("Health_Percentage", Mathf.InverseLerp(0, maxHP, currentHP));
    }

    public float GetCurrentHealth()
    {
        return currentHP;
    }

    public float GetMaxHP()
    {
        return maxHP;
    }

    public bool ModifyHPAndCheckForDeath(int modifier)
    {
        if (currentHP + modifier > maxHP)
        {
            currentHP = maxHP;
        }
        else if (currentHP + modifier <= 0)
        {
            currentHP = 0;
            return true;
        }
        else
        {
            currentHP += modifier;
        }

        return false;
    }

    public void ResetHealth()
    {
        currentHP = maxHP;
    }
}
