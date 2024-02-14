using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPlacementController : MonoBehaviour
{
    public enum TurretPlacementState
    {
        Inactive,
        Placing
    };

    public TurretPlacementState turretPlacementState;

    public GameObject turretSlotPrefab;
    public int numTurretSlotRings = 4;
    public int numTurretSlotsPerRing = 10;
    public float gridRadius = 0.3f;

    public List<GameObject> turretSlotsList;
    private GameObject turretToPlacePrefab;

    void Start()
    {
        turretPlacementState = TurretPlacementState.Inactive;
        CreateAndPositionTurretSlots();
        GameManager.instance.gameEndedEvent.AddListener(SetTurretPlacementStateInactive);
    }

    private void CreateAndPositionTurretSlots()
    {
        for (int turretSlotRing = 1; turretSlotRing < numTurretSlotRings; turretSlotRing++)
        {
            for (int turretSlotIndex = 0; turretSlotIndex < numTurretSlotsPerRing; turretSlotIndex++)
            {
                float angleAroundCentre = turretSlotIndex * Mathf.PI * 2 / numTurretSlotsPerRing;
                Vector3 ringPositionOffset = new Vector3(Mathf.Cos(angleAroundCentre), 0, Mathf.Sin(angleAroundCentre)) * (gridRadius * turretSlotRing);
                Vector3 desiredPos = new Vector3(transform.position.x + ringPositionOffset.x, transform.position.y + ringPositionOffset.y, transform.position.z + ringPositionOffset.z);

                GameObject turretSlot = Instantiate(turretSlotPrefab, desiredPos, Quaternion.identity);
                turretSlot.transform.parent = transform;
                turretSlot.transform.localScale = new Vector3(1, 1, 1);
                turretSlot.GetComponent<TurretSlotController>().turretPlacementController = this;

                turretSlotsList.Add(turretSlot);
            }
        }
    }

    public void RegisterSlotSelection(TurretSlotController turretSlot)
    {
        if (turretToPlacePrefab != null)
        {
            turretSlot.SpawnTurret(turretToPlacePrefab);

            int turretPrice = turretToPlacePrefab.GetComponent<TurretController>().GetPrice();
            BankManager.instance.ModifyCreditBalance(-turretPrice);

            SetTurretPlacementStateInactive();
        }
        else
        {
            Debug.LogWarning("No turret prefab has been selected");
        }
    }

    public void OnTurretIconSelected(GameObject turretPrefab)
    {
        turretToPlacePrefab = turretPrefab;
        ToggleTurretPlacementState();
    }

    private void ToggleTurretPlacementState()
    {
        switch(turretPlacementState)
        {
            case TurretPlacementState.Inactive:
                SetTurretPlacementStatePlacing();
                break;
            case TurretPlacementState.Placing:
                SetTurretPlacementStateInactive();
                break;
        }
    }

    public void SetTurretPlacementStatePlacing()
    {
        turretPlacementState = TurretPlacementState.Placing;
        ChangeTurretSlotMarkersVisibility(true);
    }

    public void SetTurretPlacementStateInactive()
    {
        turretPlacementState = TurretPlacementState.Inactive;
        ChangeTurretSlotMarkersVisibility(false);
    }

    public TurretPlacementState GetTurretPlacementState()
    {
        return turretPlacementState;
    }

    public void ChangeTurretSlotMarkersVisibility(bool newState)
    {
        foreach (GameObject turretSlot in turretSlotsList)
        {
            turretSlot.GetComponent<TurretSlotController>().ToggleSlotMarkerVisibility(newState);
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.gameEndedEvent.RemoveListener(SetTurretPlacementStateInactive);
    }
}
