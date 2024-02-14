using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMortalUnit
{
    void InitialiseStateMachine();
    MortalUnitStateMachine.MortalUnitState GetCurrentState();
    void DoDamage(int damagePoints);
    void TakeDamage(int damagePoints);
    void OnDeath();
}
