using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text statusText;
    [SerializeField] private HPBar hpBar;

    [SerializeField] private Color psnColor;
    [SerializeField] private Color brnColor;
    [SerializeField] private Color slpColor;
    [SerializeField] private Color parColor;
    [SerializeField] private Color frzColor;

    private Pokemon _pokemon;
    private Dictionary<ConditionID, Color> statusColors;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = $"Lvl {pokemon.Level}";
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHP);

        statusColors = new Dictionary<ConditionID, Color>
        {
            { ConditionID.psn, psnColor },
            { ConditionID.brn, brnColor },
            { ConditionID.slp, slpColor },
            { ConditionID.par, parColor },
            { ConditionID.frz, frzColor }
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
    }

    private void SetStatusText()
    {
        if (_pokemon.Status == null)
        {
            statusText.text = string.Empty;
        }
        else
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }

    public IEnumerator UpdateHP()
    {
        if (_pokemon.HPChanged)
        {
            yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHP);
            _pokemon.HPChanged = false;
        }
    }
}