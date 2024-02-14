using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretButton : MonoBehaviour
{
    public GameObject turretToSpawn;
    public GameObject padlock;
    public Text nameLabel;
    public Text priceLabel;

    private PlaceTurretButton placeTurretButton;
    private bool buttonUnlocked = false;
    private int turretPrice;

    private void Start()
    {
        Button btn = GetComponent<Button>();
        placeTurretButton = gameObject.GetComponentInParent<PlaceTurretButton>();

        nameLabel.text = turretToSpawn.name;
        turretPrice = turretToSpawn.GetComponent<TurretController>().GetPrice();
        priceLabel.text = turretPrice.ToString();

        btn.onClick.AddListener(OnTurretIconSelected);
    }

    private void Update()
    {
        UnlockButtonIfTurretAffordable();
    }

    private void UnlockButtonIfTurretAffordable()
    {
        if (BankManager.instance != null)
        {
            if (BankManager.instance.GetCreditBalance() >= turretPrice)
            {
                SetButtonUnlocked();
            }
            else
            {
                SetButtonLocked();
            }
        }
    }

    private void OnTurretIconSelected()
    {
        if (buttonUnlocked)
        {
            if (UIManager.instance != null)
            {
                placeTurretButton.ToggleTurretButtonsVisibility();
                UIManager.instance.OnTurretIconSelected(turretToSpawn);
            }
        }
    }

    private void SetButtonLocked()
    {
        buttonUnlocked = false;
        padlock.SetActive(true);
    }

    private void SetButtonUnlocked()
    {
        buttonUnlocked = true;
        padlock.SetActive(false);
    }
}
