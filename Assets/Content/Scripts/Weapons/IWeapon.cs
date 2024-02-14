using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void FireWeaponAt(Vector3 target);
    void PlayFireSFX();
}
