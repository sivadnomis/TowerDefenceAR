using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceTurretButton : MonoBehaviour
{
    public GameObject[] turretButtons;

    private void OnEnable()
    {
        HideTurretButtons();
    }

    public void ToggleTurretButtonsVisibility()
    {
        if (GameManager.instance.GetTurretPlacementState() == TurretPlacementController.TurretPlacementState.Placing)
        {
            GameManager.instance.GetTurretPlacementController().SetTurretPlacementStateInactive();
            HideTurretButtons();
        }
        else
        {
            foreach (GameObject turretButton in turretButtons)
            {
                turretButton.SetActive(!turretButton.activeInHierarchy);
            }
        }
    }

    public void HideTurretButtons()
    {
        foreach (GameObject turretButton in turretButtons)
        {
            turretButton.SetActive(false);
        }
    }
}
