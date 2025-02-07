using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private Text dialogText;
    [SerializeField] private int lettersPerSecond;
    [SerializeField] private Color highlightedColor;

    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject moveSelector;
    [SerializeField] private GameObject moveDetails;

    [SerializeField] private List<Text> actionTextList;
    [SerializeField] private List<Text> moveTextList;
    [SerializeField] private GameObject choiceBox;

    [SerializeField] private Text ppText;
    [SerializeField] private Text moveTypeText;
    [SerializeField] private Text yesText;
    [SerializeField] private Text noText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";

        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (var i = 0; i < actionTextList.Count; i++)
            if (i == selectedAction)
                actionTextList[i].color = highlightedColor;
            else
                actionTextList[i].color = Color.black;
    }

    public void UpdateChoiceBox(bool yesSelected)
    {
        if (yesSelected)
        {
            yesText.color = highlightedColor;
            noText.color = Color.black;
        }
        else
        {
            yesText.color = Color.black;
            noText.color = highlightedColor;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (var i = 0; i < moveTextList.Count; i++)
            if (i == selectedMove)
                moveTextList[i].color = highlightedColor;
            else
                moveTextList[i].color = Color.black;

        ppText.text = $"PP {move.PP}/{move.Base.Pp}";
        moveTypeText.text = move.Base.Type.ToString();

        if (move.PP == 0)
            ppText.color = Color.red;
        else
            ppText.color = Color.black;
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (var i = 0; i < moveTextList.Count; i++)
            if (i < moves.Count)
                moveTextList[i].text = moves[i].Base.Name;
            else
                moveTextList[i].text = "-";
    }
}