using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour, IMortalUnit
{
    private MortalUnitStateMachine stateMachine;
    private Health health;
    private TurretSlotController turretSlot;

    public int price;
    public GameObject explosionParticleEffectPrefab;
    public AudioClip deathSFX;

    private void Awake()
    {
        InitialiseStateMachine();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        GameManager.instance.gameRestartedEvent.AddListener(KillSelf);
    }

    public void InitialiseStateMachine()
    {
        stateMachine = gameObject.AddComponent<MortalUnitStateMachine>();
        stateMachine.ChangeState(MortalUnitStateMachine.MortalUnitState.Idle);
    }

    public MortalUnitStateMachine.MortalUnitState GetCurrentState() 
    {
        return stateMachine.currentState;
    }

    public void SetTurretSlot(TurretSlotController _turretSlot)
    {
        turretSlot = _turretSlot;
    }

    public void TakeDamage(int damagePoints)
    {
        if (stateMachine.currentState != MortalUnitStateMachine.MortalUnitState.Die)
        {
            if (health.ModifyHPAndCheckForDeath(-damagePoints))
            {
                OnDeath();
            }
        }
    }

    public void OnDeath()
    {
        GameObject explosion = Instantiate(explosionParticleEffectPrefab, transform.position, Quaternion.identity);
        PlayDeathSFX();
        KillSelf();
    }

    private void KillSelf()
    {
        stateMachine.ChangeState(MortalUnitStateMachine.MortalUnitState.Die);
        turretSlot.RegisterTurretDeath();
        StartCoroutine(DestroySelf());
    }

    private IEnumerator DestroySelf() //TODO fix this hacky solution for OnColliderExit not called on enemies if destroying object immediately
    {
        gameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 5, transform.position.z); 
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void PlayDeathSFX()
    {
        GenericSoundSystem.instance.PlaySFXAtPosition(deathSFX, 0.2f, transform.position);
    }

    public int GetPrice()
    {
        return price;
    }

    public void DoDamage(int damagePoints) { }
}
