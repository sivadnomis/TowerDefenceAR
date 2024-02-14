using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : MonoBehaviour, IProjectile, IPooledObject
{
    public string poolTag = "GreenLaserBolt";
    public float maxLifespan = 10f;
    public int damage;
    public string explosionPoolTag;
    public AudioClip impactSFX;

    private ObjectPooler objectPooler;

    private void Start()
    {
        objectPooler = ObjectPooler.instance;
    }

    public void OnSpawn()
    {
        StartCoroutine(StartLifespanTimer());
    }

    public void OnCollisionEnter(Collision collision)
    {
        PlayImpactSFX();
        GameObject explosion = objectPooler.SpawnFromPool(explosionPoolTag, transform.position, transform.rotation);
        OnDeath();
    }

    public void OnDeath()
    {
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
        if (impactSFX != null)
        {
            GenericSoundSystem.instance.PlaySFXAtPosition(impactSFX, 0.1f, transform.position); //TODO remove magic number
        }
    }
}