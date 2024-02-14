using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour, IPooledObject
{
    public string poolTag;
    public int damage;
    public float explosionRadius;
    public float explosionForce;
    public float upwardExplosionForce;
    public GameObject particleEffect;
    public AudioClip impactSFX;
    public float impactSFXPitch;

    public void OnSpawn()
    {
        PlaySFX();
        EnableParticleEffect();
        DamageEnemiesInRadius();
        AddExplosionForceToRigidbodiesInRadius();
        StartCoroutine(StartDeathTimer());
    }

    private void DamageEnemiesInRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in hitColliders)
        {
            EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    private void AddExplosionForceToRigidbodiesInRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in hitColliders)
        {
            Rigidbody rigidbody = collider.gameObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardExplosionForce);
            }
        }
    }

    private IEnumerator StartDeathTimer()
    {
        yield return new WaitForSeconds(2f);
        OnDeath();
    }

    private void OnDeath()
    {
        ObjectPooler.instance.ReAddObjectToPool(poolTag, gameObject);
    }

    private void EnableParticleEffect()
    {
        if (particleEffect != null)
        {
            particleEffect.SetActive(true);
        }
    }

    private void PlaySFX()
    {
        if (impactSFX != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();

            if (audioSource != null)
            {
                float pitchModulationAmount = Random.Range(-0.05f, 0.05f);
                audioSource.pitch = impactSFXPitch + pitchModulationAmount;

                GetComponent<AudioSource>().PlayOneShot(impactSFX);
            }
        }
    }

    public void Reset() 
    {
        if (particleEffect != null)
        {
            particleEffect.SetActive(false);
        }
    }

    public void OnDespawn() { }
}
