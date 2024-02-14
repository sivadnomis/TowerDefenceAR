using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour, IPooledObject, IMortalUnit
{
    private MortalUnitStateMachine stateMachine;
    private Health health;

    public string poolTag = "Enemy";
    public float movementSpeed;
    public int attackPoints;
    public int creditRewardOnDeath;

    private GameObject closestTarget;
    private List<GameObject> currentCollisions;
    private Animator animator;
    private ProximityAimingModule proximityAimingModule;
    private RagdollManager ragdollManager;

    private AudioSource audioSource;
    public float sfxPitch;
    public AudioClip deathSFX;
    public AudioClip takeDamageSFX;
    public AudioClip doDamageSFX;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        health = GetComponent<Health>();
        proximityAimingModule = GetComponent<ProximityAimingModule>();
        ragdollManager = GetComponent<RagdollManager>();
    }

    public void OnSpawn()
    {
        animator = GetComponent<Animator>();
        InitialiseStateMachine();

        GameManager.instance.gameEndedEvent.AddListener(OnGameEnd);
        GameManager.instance.gameRestartedEvent.AddListener(OnGameRestart);

        if (ragdollManager != null)
        {
            ragdollManager.OnSpawn();
        }

        currentCollisions = new List<GameObject>();

        StartCoroutine(UpdateClosestTargets());

        if (System.Math.Abs(health.GetMaxHP() - health.GetCurrentHealth()) > Mathf.Epsilon)
        {
            Debug.LogWarning("Enemy spawned on lower than max health: " + health.GetCurrentHealth());
        }
    }

    void Update()
    {
        if (closestTarget != null)
        {
            if (stateMachine.currentState == MortalUnitStateMachine.MortalUnitState.Move)
            {
                float step = movementSpeed * Time.deltaTime;

                transform.LookAt(closestTarget.transform.position, Vector3.up);
                transform.position = Vector3.MoveTowards(transform.position, closestTarget.transform.position, step);
            }
        }
    }

    public void InitialiseStateMachine()
    {
        stateMachine = gameObject.AddComponent<MortalUnitStateMachine>();
        stateMachine.ChangeState(MortalUnitStateMachine.MortalUnitState.Idle);
        stateMachine.AssignAnimator(animator);
    }

    public MortalUnitStateMachine.MortalUnitState GetCurrentState()
    {
        return stateMachine.currentState;
    }

    private IEnumerator UpdateClosestTargets()
    {
        while (stateMachine.currentState != MortalUnitStateMachine.MortalUnitState.Die && GameManager.instance.GetGameState() != GameManager.GameState.End)
        {
            if (proximityAimingModule.FindClosestTarget(ref closestTarget, 100f)) //TODO remove magic number
            {
                stateMachine.ChangeState(MortalUnitStateMachine.MortalUnitState.Move);

            }
            else
            {
                stateMachine.ChangeState(MortalUnitStateMachine.MortalUnitState.Idle);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentCollisions.Add(collision.gameObject);

        if (collision.gameObject.tag == "PlayerUnit")
        {
            stateMachine.ChangeState(MortalUnitStateMachine.MortalUnitState.Attack);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        currentCollisions.Remove(collision.gameObject);

        if (collision.gameObject.tag == "PlayerUnit")
        {
            stateMachine.ChangeState(MortalUnitStateMachine.MortalUnitState.Idle);
        }
    }

    public void DoDamage(int damagePoints)
    {
        foreach (GameObject collision in currentCollisions)
        {
            if (collision != null)
            {
                if (collision.tag == "PlayerUnit")
                {
                    PlaySFX(doDamageSFX);

                    IMortalUnit mortalUnit = collision.GetComponentInParent<IMortalUnit>();
                    if (mortalUnit != null)
                    {
                        mortalUnit.TakeDamage(attackPoints);
                    }
                }
            }
        }
    }

    public void TakeDamage(int damagePoints)
    {
        if (stateMachine.currentState != MortalUnitStateMachine.MortalUnitState.Die)
        {
            if (health.ModifyHPAndCheckForDeath(-damagePoints))
            {
                OnDeath();
            }
            else
            {
                PlaySFX(takeDamageSFX);
            }
        }
    }

    private void OnGameEnd()
    {
        stateMachine.ChangeState(MortalUnitStateMachine.MortalUnitState.Idle);
    }

    private void OnGameRestart()
    {
        OnDespawn();
    }

    public void OnDeath()
    {
        if (stateMachine.currentState != MortalUnitStateMachine.MortalUnitState.Die)
        {
            StopCoroutine(UpdateClosestTargets());

            stateMachine.ChangeState(MortalUnitStateMachine.MortalUnitState.Die);
            if (BankManager.instance != null)
            {
                BankManager.instance.ModifyCreditBalance(creditRewardOnDeath);
            }

            if (ragdollManager != null)
            {
                ragdollManager.OnDeath();
            }

            GameManager.instance.RegisterEnemyDeath();

            StartCoroutine(RemoveFromBattlefield());
        }
    }

    public void OnDespawn()
    {
        ObjectPooler.instance.ReAddObjectToPool(poolTag, gameObject);
        GameManager.instance.gameEndedEvent.RemoveListener(OnGameEnd);
        GameManager.instance.gameRestartedEvent.RemoveListener(OnGameRestart);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            float pitchModulationAmount = Random.Range(-0.05f, 0.05f);
            audioSource.pitch = sfxPitch + pitchModulationAmount;

            audioSource.PlayOneShot(clip);
        }
    }

    public IEnumerator RemoveFromBattlefield()
    {
        yield return new WaitForSeconds(3f);

        OnDespawn();
    }

    public void Reset()
    {
        health.ResetHealth();
    }

    private void OnDestroy()
    {
        GameManager.instance.gameEndedEvent.RemoveListener(OnGameEnd);
        GameManager.instance.gameRestartedEvent.RemoveListener(OnGameRestart);
    }
}
