using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float MoveSpeed;
    private CharacterAnimator animator;

    public CharacterAnimator Animator
    {
        get { return animator; }
    }

    public bool IsMoving { get; private set; }

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator Move(Vector2 moveVector, Action onMoveFinished=null)
    {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1.0f, 1.0f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1.0f, 1.0f);

        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;
        
        if(!IsWalkable(targetPos))
            yield break;
        
        IsMoving = true;

        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, MoveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        IsMoving = false;

        onMoveFinished?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }
    
    private bool IsWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.Instance.SolidObjectsLayer | GameLayers.Instance.InteractablesLayer) == null;
    }
}
