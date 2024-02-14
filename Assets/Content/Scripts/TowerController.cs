using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class TowerController : MonoBehaviour, IMortalUnit
{
    private MortalUnitStateMachine stateMachine;

    private Health health;
    private Vector3 startingPos;
    private AudioSource audioSource;
    private ObjectPooler objectPooler;

    [Header("Death")]
    public Transform deathSinkTargetPos;
    public float deathSinkSpeed;

    [Header("Death Explosion")]
    public string explosionPoolTag;
    public GameObject explosionParticleEffect;
    public AudioClip explosionSFX;

    private void Start()
    {
        InitialiseStateMachine();
        startingPos = transform.position;
        audioSource = GetComponent<AudioSource>();
        health = GetComponent<Health>();
        objectPooler = ObjectPooler.instance;
        GameManager.instance.gameRestartedEvent.AddListener(OnGameRestart);
    }

    private void Update()
    {
        if (stateMachine.currentState == MortalUnitStateMachine.MortalUnitState.Die)
        {
            transform.position = Vector3.MoveTowards(transform.position, deathSinkTargetPos.position, deathSinkSpeed * Time.deltaTime);
        }
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

    public void DoDamage(int damagePoints) { }

    public void TakeDamage(int damagePoints)
    {
        if (health.ModifyHPAndCheckForDeath(-damagePoints))
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        GameManager.instance.EndGame();
        stateMachine.ChangeState(MortalUnitStateMachine.MortalUnitState.Die);

        GameObject explosion = objectPooler.SpawnFromPool(explosionPoolTag, transform.position, transform.rotation);

        explosionParticleEffect.SetActive(true);
        audioSource.PlayOneShot(explosionSFX);
    }

    public void OnGameRestart()
    {
        audioSource.Stop();
        explosionParticleEffect.SetActive(false);

        stateMachine.ChangeState(MortalUnitStateMachine.MortalUnitState.Idle);
        health.ResetHealth();
        transform.position = startingPos;
    }
}
