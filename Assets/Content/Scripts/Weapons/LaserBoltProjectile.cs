using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBoltProjectile : MonoBehaviour, IProjectile, IPooledObject
{
    public string poolTag = "LaserBolt";
    public float maxLifespan = 5f;
    public int damage;
    public GameObject impactPrefab;
    public AudioClip impactSFX;

    public void OnSpawn() 
    {
        StartCoroutine(StartLifespanTimer());
    }

    public void OnCollisionEnter(Collision collision)
    {
        GameObject collisionGO = collision.gameObject;

        PlayImpactSFX();

        ContactPoint contactPoint = collision.contacts[0];
        GameObject impact = Instantiate(impactPrefab, contactPoint.point, Quaternion.LookRotation(-contactPoint.normal));

        Rigidbody[] rigidBodies = collisionGO.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigidbody in rigidBodies)
        {
            rigidbody.AddForce(contactPoint.normal * 30f);
        }

        if (collision.gameObject.tag == "Enemy")
        {
            EnemyController enemy = collisionGO.GetComponent<EnemyController>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        OnDeath();
    }

    public void OnDeath()
    {
        PlayImpactSFX();
        OnDespawn();
    }

    public void OnDespawn()
    {
        StopCoroutine(StartLifespanTimer());
        ObjectPooler.instance.ReAddObjectToPool(poolTag, gameObject);
    }

    public void Reset() { }

    public IEnumerator StartLifespanTimer()
    {
        yield return new WaitForSeconds(maxLifespan);
        OnDeath();
    }

    public void PlayImpactSFX()
    {
        GenericSoundSystem.instance.PlaySFXAtPosition(impactSFX, 0.1f, transform.position); //TODO remove magic number
    }
}
