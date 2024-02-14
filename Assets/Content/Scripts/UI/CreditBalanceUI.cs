using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditBalanceUI : MonoBehaviour
{
    public UnityEngine.UI.Text balanceLabel;

    private void Update()
    {
        if (BankManager.instance != null)
        {
            balanceLabel.text = BankManager.instance.GetCreditBalance().ToString();
        }
    }
}
