using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event Action OnEncountered;
    
    private Vector2 _input;
    
    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");

            // removing diagonal movement
            if(_input.x != 0) _input.y = 0;

            if(_input != Vector2.zero)
            {
              StartCoroutine(character.Move(_input, CheckForEncounters));
            }
        }
        
        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    private void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;
        
        // Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);
        
        var collider = Physics2D.OverlapCircle(interactPos, .3f, GameLayers.Instance.InteractablesLayer);

        if (collider != null)
        {
            collider.GetComponent<IInteractable>()?.Interact(transform);
        }
    }
    
    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.Instance.GrassLayer) != null) 
        {
            if(UnityEngine.Random.Range(1, 101) <= 10)
            {
                character.Animator.IsMoving = false;
                OnEncountered();
            }
        
        }
    }
}
