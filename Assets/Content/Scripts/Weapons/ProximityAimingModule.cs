using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityAimingModule : MonoBehaviour
{
    public string targetTag;
    public LayerMask layerMask;
    private GameObject[] targets;

    public bool FindClosestTarget(ref GameObject closestTarget, float range, bool requiresLineOfSight = false)
    {
        targets = GameObject.FindGameObjectsWithTag(targetTag);

        if (targets.Length > 0)
        {
            GameObject tempClosestTarget = null;
            float distanceToClosestTarget = Mathf.Infinity;

            foreach (GameObject target in targets)
            {
                if (requiresLineOfSight)
                {
                    if (!HasClearLineOfSight(target, range))
                    {
                        continue;
                    }
                }

                if (target.GetComponent<IMortalUnit>().GetCurrentState() != MortalUnitStateMachine.MortalUnitState.Die)
                {
                    float tempDistance = GetDistanceBetweenObjects(gameObject, target);
                    if (tempDistance < distanceToClosestTarget)
                    {
                        tempClosestTarget = target;
                        distanceToClosestTarget = tempDistance;
                    }
                }
            }
            closestTarget = tempClosestTarget;
            return true;
        }

        closestTarget = null;
        return false;
    }

    private bool HasClearLineOfSight(GameObject target, float range)
    {
        if (Physics.Linecast(transform.position, target.transform.position, out RaycastHit hit, layerMask))
        {
            //Debug.DrawLine(transform.position, hit.point, Color.blue, 2, false);
            if (hit.transform.tag == targetTag)
            {
                //Debug.DrawLine(transform.position, hit.point, Color.green, 2, false);
                return true;
            }
        }
        //else
        //{
        //    Debug.DrawLine(transform.position, target.transform.position, Color.red, 2, false);
        //}

        return false;
    }

    private float GetDistanceBetweenObjects(GameObject x, GameObject y)
    {
        return Vector3.Distance(x.transform.position, y.transform.position);
    }

    public LayerMask GetLayerMask()
    {
        return layerMask;
    }
}
