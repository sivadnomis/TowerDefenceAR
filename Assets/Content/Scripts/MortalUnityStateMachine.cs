using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortalUnitStateMachine : MonoBehaviour
{
    public enum MortalUnitState
    {
        Idle,
        Move,
        Attack,
        Die
    };

    public MortalUnitState currentState;

    private Animator animator;

    public void AssignAnimator(Animator _animator)
    {
        animator = _animator;
    }

    public void ChangeState(MortalUnitState newState)
    {
        currentState = newState;

        if (animator != null)
        {
            switch (newState)
            {
                case MortalUnitState.Idle:
                    animator.SetBool("isMoving", false);
                    animator.SetBool("isAttacking", false);
                    break;
                case MortalUnitState.Move:
                    animator.SetBool("isMoving", true);
                    break;
                case MortalUnitState.Attack:
                    animator.SetBool("isAttacking", true);
                    break;
                case MortalUnitState.Die:
                    animator.SetTrigger("Die");
                    break;
                default:
                    break;

            }
        }
    }
}
