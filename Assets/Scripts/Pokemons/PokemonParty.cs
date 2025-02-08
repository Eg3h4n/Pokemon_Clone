using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;

    public List<Pokemon> Pokemons => pokemons;

    private void Start()
    {
        foreach (var pokemon in pokemons) pokemon.Init();
    }

    public Pokemon GetHealthyPokemon()
    {
        return pokemons.Where(p => p.HP > 0).FirstOrDefault();
    }

    public void AddPokemon(Pokemon newPokemon)
    {
        if (pokemons.Count < 6) pokemons.Add(newPokemon);
        // TODO: send to pc
    }
}