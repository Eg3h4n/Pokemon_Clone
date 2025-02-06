using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private Sprite playerSprite;

    private Character _character;

    private Vector2 _input;

    public string PlayerName => playerName;
    public Sprite PlayerSprite => playerSprite;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainersFoV;

    public void HandleUpdate()
    {
        if (!_character.IsMoving)
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");

            // removing diagonal movement
            if (_input.x != 0) _input.y = 0;

            if (_input != Vector2.zero) StartCoroutine(_character.Move(_input, OnMoveOver));
        }

        _character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z)) Interact();
    }

    private void Interact()
    {
        var facingDir = new Vector3(_character.Animator.MoveX, _character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        // Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, .3f, GameLayers.Instance.InteractablesLayer);

        if (collider != null) collider.GetComponent<IInteractable>()?.Interact(transform);
    }

    private void OnMoveOver()
    {
        CheckForEncounters();
        CheckIfInTrainerFoV();
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.Instance.GrassLayer) == null) return;
        if (Random.Range(1, 101) > 10) return;
        _character.Animator.IsMoving = false;
        OnEncountered();
    }

    private void CheckIfInTrainerFoV()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.Instance.FovLayer);
        if (collider == null) return;
        _character.Animator.IsMoving = false;
        OnEnterTrainersFoV?.Invoke(collider);
    }
}