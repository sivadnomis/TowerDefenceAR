using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    private Animator animator;
    private Rigidbody[] rigidBodies;
    private Collider[] colliders;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void OnSpawn()
    {
        animator.enabled = true;
        SetRigidBodyState(true);
        SetColliderState(false);
    }

    public void OnDeath()
    {
        animator.enabled = false;
        SetRigidBodyState(false);
        SetColliderState(true);
    }

    private void SetRigidBodyState(bool newState)
    {
        foreach (Rigidbody rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = newState;
        }

        GetComponent<Rigidbody>().isKinematic = !newState;
    }

    private void SetColliderState(bool newState)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = newState;
        }

        GetComponent<Collider>().enabled = !newState;
    }
}
