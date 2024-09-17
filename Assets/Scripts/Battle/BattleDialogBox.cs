using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<Text> actionTextList;
    [SerializeField] List<Text> moveTextList;

    [SerializeField] Text ppText;
    [SerializeField] Text moveTypeText;

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

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTextList.Count; i++)
        {
            if (i == selectedAction)
                actionTextList[i].color = highlightedColor;
            else
                actionTextList[i].color = Color.black;

        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTextList.Count; i++)
        {
            if (i == selectedMove)
                moveTextList[i].color = highlightedColor;
            else
                moveTextList[i].color = Color.black;

        }

        ppText.text = $"PP {move.PP}/{move.Base.Pp}";
        moveTypeText.text = move.Base.Type.ToString();
    }

    public void SetMoveNames(List<Move> moves)
    {
        for(int i = 0;i < moveTextList.Count; i++)
        {
            if(i < moves.Count)
                moveTextList[i].text = moves[i].Base.Name;
            else
                moveTextList[i].text = "-";

        }
    }
}
