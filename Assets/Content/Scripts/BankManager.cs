using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankManager : MonoBehaviour
{
    #region Singleton
    public static BankManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    private int creditBalance;

    public void OnGameManagerSpawned()
    {
        GameManager.instance.gameStartedEvent.AddListener(InitializeBank);
    }

    public void InitializeBank()
    {
        creditBalance = 0;
    }

    public int GetCreditBalance()
    {
        return creditBalance;
    }

    public void ModifyCreditBalance(int amount)
    {
        if (creditBalance + amount > 9999)
        {
            creditBalance = 9999;
        }
        else if (creditBalance + amount < 0)
        {
            creditBalance = 0;
        }
        else
        {
            creditBalance += amount;
        }
    }
}
