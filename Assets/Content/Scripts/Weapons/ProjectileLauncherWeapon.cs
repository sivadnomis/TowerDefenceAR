using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncherWeapon : MonoBehaviour, IWeapon
{
    public string poolTag;
    public float range = 100f;
    public float cooldownPeriodInSeconds;
    public float arcHeight;
    public AudioClip fireSFX;

    private ObjectPooler objectPooler;
    private GameObject closestTarget;
    private ProximityAimingModule proximityAimingModule;
    private float gravity;
    private float timeOfLastFire;

    private void Start()
    {
        objectPooler = ObjectPooler.instance;
        proximityAimingModule = GetComponent<ProximityAimingModule>();
        timeOfLastFire = Time.time;
        gravity = GameManager.instance.GetGravity();
    }

    private void Update()
    {
        if (GameManager.instance.GetGameState() != GameManager.GameState.End)
        {
            if (!proximityAimingModule.FindClosestTarget(ref closestTarget, range, false))
            {
                //Debug.Log("No projectile target found");
            }

            if (closestTarget != null)
            {
                if (CooldownPeriodHasExpired())
                {
                    Vector3 targetPos = closestTarget.transform.position;
                    FireWeaponAt(targetPos);
                    timeOfLastFire = Time.time;
                }
            }
        }
    }

    private bool CooldownPeriodHasExpired()
    {
        if (timeOfLastFire + cooldownPeriodInSeconds <= Time.time)
        {
            return true;
        }

        return false;
    }

    public void FireWeaponAt(Vector3 target)
    {
        PlayFireSFX();

        GameObject projectile = objectPooler.SpawnFromPool(poolTag, transform.position, transform.rotation);
        if (projectile != null)
        {
            Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
            projectileRigidbody.useGravity = true;
            projectileRigidbody.velocity = CalculateLaunchVelocity(projectile.transform.position, target);
        }
        else
        {
            Debug.Log("Could not spawn " + poolTag + " from pool");
        }
    }

    private Vector3 CalculateLaunchVelocity(Vector3 projectilePos, Vector3 targetPos)
    {
        float displacementY = targetPos.y - projectilePos.y;
        Vector3 displacementXZ = new Vector3(targetPos.x - projectilePos.x, 0, targetPos.z - projectilePos.z);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * arcHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * arcHeight / gravity) + Mathf.Sqrt(2 * (displacementY - arcHeight) / gravity));

        return velocityXZ + velocityY;
    }

    public void PlayFireSFX()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(fireSFX);
        }
    }
}
