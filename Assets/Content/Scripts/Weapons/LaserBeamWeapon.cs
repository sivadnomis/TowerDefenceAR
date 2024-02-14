using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamWeapon : MonoBehaviour, IWeapon
{
    public LayerMask layerMask;
    public float range = 100f;
    public float impactForce = 0f;
    public int damagePerSecond = 5;
    public GameObject laserBeam;

    public AudioClip fireSFX;
    public GameObject impactEffectPrefab;
    private GameObject impactEffect;
    private GameObject currentTarget;

    private GameObject closestTarget;
    private Coroutine damageTarget;
    private AudioSource audioSource;
    private ProximityAimingModule proximityAimingModule;

    private void Start()
    {
        if (damagePerSecond < 5)
        {
            damagePerSecond = 5;
        }

        audioSource = GetComponent<AudioSource>();
        proximityAimingModule = GetComponent<ProximityAimingModule>();

        GameManager.instance.gameEndedEvent.AddListener(OnGameEnd);
    }

    private void Update()
    {
        if (GameManager.instance.GetGameState() != GameManager.GameState.End)
        {
            if (!proximityAimingModule.FindClosestTarget(ref closestTarget, range, true))
            {
                //Debug.Log("No beam target found");

                if (impactEffect != null)
                {
                    Destroy(impactEffect);
                }
                if (audioSource != null)
                {
                    audioSource.enabled = false;
                }
            }

            if (closestTarget != null)
            {
                FireWeaponAt(closestTarget.transform.position);
            }
            else
            {
                if (damageTarget != null)
                {
                    StopCoroutine(damageTarget);
                }

                laserBeam.gameObject.SetActive(false);
                StopAudio();
                if (impactEffect != null)
                {
                    Destroy(impactEffect);
                }
            }
        }
    }

    public void FireWeaponAt(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, range, proximityAimingModule.GetLayerMask())) //TODO check to see whether this should be this scripts own layermask (so aiming and firing can use different ones)
        {
            laserBeam.gameObject.SetActive(true);
            laserBeam.GetComponentInChildren<LineRenderer>().SetPosition(0, transform.position);
            laserBeam.GetComponentInChildren<LineRenderer>().SetPosition(1, hit.point);
            PlayFireSFX();

            IMortalUnit targetUnit = hit.transform.GetComponent<IMortalUnit>();
            if (targetUnit != null && targetUnit.GetCurrentState() != MortalUnitStateMachine.MortalUnitState.Die)
            {
                damageTarget = StartCoroutine(DamageTargetUnit(targetUnit));
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            if (closestTarget != currentTarget)
            {
                SpawnImpactEffectAt(hit);
                currentTarget = closestTarget;
            }
            else
            {
                if (impactEffect != null)
                {
                    impactEffect.gameObject.transform.position = hit.point;
                }
            }
        }
    }

    private void SpawnImpactEffectAt(RaycastHit hit)
    {
        Destroy(impactEffect);
        impactEffect = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        impactEffect.transform.parent = transform;
    }

    private IEnumerator DamageTargetUnit(IMortalUnit targetUnit)
    {
        yield return new WaitForSeconds(0.2f);

        targetUnit.TakeDamage(damagePerSecond / 5);
    }

    public void PlayFireSFX()
    {
        if (audioSource != null)
        {
            audioSource.enabled = true;
        }
    }

    private void StopAudio()
    {
        if (audioSource != null)
        {
            audioSource.enabled = false;
        }
    }

    private void OnGameEnd()
    {
        laserBeam.gameObject.SetActive(false);
        closestTarget = null;

        if (impactEffect != null)
        {
            Destroy(impactEffect);
        }

        if (audioSource != null)
        {
            audioSource.enabled = false;
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.gameEndedEvent.RemoveListener(OnGameEnd);
    }
}
