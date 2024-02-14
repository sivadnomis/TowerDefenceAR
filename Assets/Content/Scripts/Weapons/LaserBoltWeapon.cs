using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaserBoltWeapon : MonoBehaviour, IWeapon
{
    private ObjectPooler objectPooler;
    public string poolTag;

    public LayerMask layerMask;
    public float range = 100f;
    public float projectileSpeed = 10f;
    public float fireRate;
    public int numShotsPerFire;
    public AudioClip fireSFX;

    private Vector3 targetPos;
    private UserTapAimingModule userTapAimingModule;

    private void Start()
    {
        objectPooler = ObjectPooler.instance;
        userTapAimingModule = GetComponent<UserTapAimingModule>();
    }

    private void Update()
    {
        if (GameManager.instance.GetGameState() == GameManager.GameState.Playing && GameManager.instance.GetTurretPlacementState() == TurretPlacementController.TurretPlacementState.Inactive)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!EventSystem.current.IsPointerOverGameObject(0))
                {
                    if (userTapAimingModule.CheckForValidTargetPosition(range, layerMask, ref targetPos))
                    {
                        StartCoroutine(FireMultishot(targetPos, numShotsPerFire));
                    }
                }
            }
        }
    }

    private IEnumerator FireMultishot(Vector3 target, int numShots)
    {
        while (numShots > 0)
        {
            FireWeaponAt(target);
            yield return new WaitForSeconds(fireRate);
            numShots--;
        }
    }

    public void FireWeaponAt(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, range, layerMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, 2, false);

            PlayFireSFX();

            GameObject projectile = objectPooler.SpawnFromPool(poolTag, transform.position, transform.rotation);
            if (projectile != null)
            {
                Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();

                if (projectileRigidbody != null)
                {
                    projectileRigidbody.velocity = (hit.point - transform.position).normalized * projectileSpeed;
                    projectileRigidbody.rotation = Quaternion.LookRotation(projectileRigidbody.velocity);
                }
            }
        }
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
