using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Pokemon _pokemon;
    Dictionary<ConditionID, Color> statusColors;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = $"Lvl {pokemon.Level}";
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHP);

        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor},
            {ConditionID.brn, brnColor},
            {ConditionID.slp, slpColor},
            {ConditionID.par, parColor},
            {ConditionID.frz, frzColor},
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
    }

    private void SetStatusText()
    {
        if(_pokemon.Status == null)
            statusText.text = string.Empty;
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
