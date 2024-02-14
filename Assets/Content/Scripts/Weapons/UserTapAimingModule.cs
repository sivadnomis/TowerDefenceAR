using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTapAimingModule : MonoBehaviour
{
    public bool CheckForValidTargetPosition(float range, LayerMask layerMask, ref Vector3 targetPos)
    {
        Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(raycast, out raycastHit, range, layerMask))
        {
            targetPos = raycastHit.point;
            return true;
        }

        return false;
    }
}
