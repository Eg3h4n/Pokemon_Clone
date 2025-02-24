using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnHideDialog;
    
    public static DialogManager Instance {
        get; private set;
    }

    private void Awake()
    {
        Instance = this;
    }
    
    private Dialog _dialog;
    private int _currentLine = 0;
    private bool _isTyping;
    Action OnDialogEnd;
    
    public bool IsInDialog {get; private set;}

    public IEnumerator ShowDialog(Dialog dialog, Action onFinish = null)
    {
        yield return new WaitForEndOfFrame(); // we wait because the first interaction is already pressing Z key
        
        OnShowDialog?.Invoke();
        
        IsInDialog = true;
        _dialog = dialog;
        OnDialogEnd = onFinish;
        dialogBox.SetActive(true);
        
        yield return TypeDialog(dialog.Lines[0]);
    }

    public void HandleUpdate()
    {
        if (!Input.GetKeyDown(KeyCode.Z) || _isTyping) return;
        
        ++_currentLine;

        if (_currentLine < _dialog.Lines.Count)
        {
            StartCoroutine(TypeDialog(_dialog.Lines[_currentLine]));
        }
        else
        {
            _currentLine = 0;
            IsInDialog = false;
            dialogBox.SetActive(false);
            OnHideDialog?.Invoke();
            OnDialogEnd?.Invoke();
        }
    }
    
    public IEnumerator TypeDialog(string dialog)
    {
        _isTyping = true;
        dialogText.text = "";

        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        _isTyping = false;
    }
}
