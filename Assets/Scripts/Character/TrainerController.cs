using System.Collections;
using UnityEngine;

public class TrainerController : MonoBehaviour, IInteractable
{
    [SerializeField] private string trainerName;
    [SerializeField] private Sprite trainerSprite;
    [SerializeField] private Dialog dialog;
    [SerializeField] private Dialog dialogAfterBattle;
    [SerializeField] private GameObject exclamationMark;
    [SerializeField] private GameObject fov;

    private Character _character;
    private bool _isDefeated;

    public string TrainerName => trainerName;
    public Sprite TrainerSprite => trainerSprite;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(_character.Animator.DefaultDirection);
    }

    private void Update()
    {
        _character.HandleUpdate();
    }

    public void Interact(Transform initiator)
    {
        _character.LookTowards(initiator.position);

        if (!_isDefeated)
            // Starts dialog
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog,
                () => { GameController.Instance.StartTrainerBattle(this); }));
        else
            StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterBattle));
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        exclamationMark.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamationMark.SetActive(false);

        var diff = player.transform.position - transform.position;
        var moveVector = diff - diff.normalized;
        moveVector = new Vector2(Mathf.Round(moveVector.x), Mathf.Round(moveVector.y));

        yield return _character.Move(moveVector);

        // Starts dialog
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog,
            () => { GameController.Instance.StartTrainerBattle(this); }));
    }

    public void GotDefeated()
    {
        _isDefeated = true;
        fov.gameObject.SetActive(false);
    }

    public void SetFovRotation(FacingDirection facingDirection)
    {
        var angle = facingDirection switch
        {
            FacingDirection.Up => 90f,
            FacingDirection.Right => 90f,
            FacingDirection.Left => 270f,
            _ => 0f
        };

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}