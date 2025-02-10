using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private Text messageText;

    private PartyMemberUI[] memberSlots;
    private List<Pokemon> pokemonList;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        pokemonList = pokemons;

        for (var i = 0; i < memberSlots.Length; i++)
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemons[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }

        messageText.text = "Choose a pokemon";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (var i = 0; i < pokemonList.Count; i++)
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}