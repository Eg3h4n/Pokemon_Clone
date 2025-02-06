using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamationMark;
    [SerializeField] private GameObject fov;

    private Character _character;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(_character.Animator.DefaultDirection);
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
        StartCoroutine( DialogManager.Instance.ShowDialog(dialog, () =>
        {
            Debug.Log("Seni sectim pikachu");
        }));
    }

    public void SetFovRotation(FacingDirection facingDirection)
    {
        float angle = facingDirection switch
        {
            FacingDirection.Up => 90f,
            FacingDirection.Right => 90f,
            FacingDirection.Left => 270f,
            _ => 0f
        };
        
        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}
