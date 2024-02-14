using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSlotController : MonoBehaviour
{
    public enum TurretSlotState
    {
        Empty,
        InUse
    };

    private TurretSlotState turretSlotState;
    private GameObject occupyingTurret;

    public GameObject slotMarker;

    [HideInInspector]
    public TurretPlacementController turretPlacementController;

    private RaycastHit hit;
    private Ray ray;

    private void Start()
    {
        turretSlotState = TurretSlotState.Empty;
        slotMarker.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Collider markerCollider = slotMarker.GetComponent<Collider>();
                if (hit.collider == markerCollider)
                {
                    turretPlacementController.RegisterSlotSelection(this);
                }
            }
        }
    }

    public void SpawnTurret(GameObject turretToPlacePrefab)
    {
        GameObject placedTurret = Instantiate(turretToPlacePrefab, transform.position, Quaternion.identity);
        placedTurret.transform.parent = transform;
        placedTurret.transform.localScale = new Vector3(1, 1, 1);

        placedTurret.GetComponent<TurretController>().SetTurretSlot(this);
        RegisterNewTurretSpawned(placedTurret);
    }

    public void RegisterTurretDeath()
    {
        turretSlotState = TurretSlotState.Empty;
        if (turretPlacementController.GetTurretPlacementState() == TurretPlacementController.TurretPlacementState.Placing)
        {
            slotMarker.SetActive(true);
        }
        occupyingTurret = null;
    }

    public void ToggleSlotMarkerVisibility(bool newState)
    {
        if (turretSlotState == TurretSlotState.Empty)
        {
            slotMarker.SetActive(newState);
        }
    }

    public void RegisterNewTurretSpawned(GameObject turret)
    {
        turretSlotState = TurretSlotState.InUse;
        slotMarker.SetActive(false);
        occupyingTurret = turret;
    }
}
