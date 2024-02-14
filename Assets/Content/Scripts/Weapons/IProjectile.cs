using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    void OnCollisionEnter(Collision collision);
    void PlayImpactSFX();
    IEnumerator StartLifespanTimer();
}
