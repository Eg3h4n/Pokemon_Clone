using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialog dialog;
    [SerializeField] private List<Vector2> movementPattern;
    [SerializeField] float timeBetweenMovements;

    private NPCState state;
    private float idleTimer;
    int currentMovementPatternIndex = 0;
    
    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact()
    {
        if(state == NPCState.Idle)
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }

    private void Update()
    {
        if(DialogManager.Instance.IsInDialog) return;
        
        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= timeBetweenMovements)
            {
                idleTimer = 0;
                if (movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;
        
        yield return character.Move(movementPattern[currentMovementPatternIndex]);
        currentMovementPatternIndex = (currentMovementPatternIndex + 1) % movementPattern.Count; // to make it into a loop when the index is higher than count
        
        state = NPCState.Idle;
    }
}

public enum NPCState
{
    Idle,
    Walking
}
