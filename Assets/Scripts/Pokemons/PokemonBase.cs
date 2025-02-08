using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private string name;

    [TextArea] [SerializeField] private string description;

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;

    // Base Stats
    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;
    [SerializeField] private int catchRate;

    [SerializeField] private List<LearnableMove> learnableMoves;

    public string Name => name;
    public string Description => description;
    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;
    public PokemonType Type1 => type1;
    public PokemonType Type2 => type2;
    public int MaxHP => maxHP;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;
    public int Speed => speed;
    public int CatchRate => catchRate;

    public List<LearnableMove> LearnableMoves => learnableMoves;
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase moveBase;
    [SerializeField] private int level;

    public MoveBase Base => moveBase;

    public int Level => level;
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    //These 2 are not actual stats, they are used for move accuracy
    Accurracy,
    Evasion
}

public class TypeChart
{
    private static readonly float[][] chart =
    {
        //                Nor  Fir  Wat   Ele   Gra   Ice   Fig   Poi   Gro   Fly   Psy   Bug   Roc   Gho   Dra   Dar  Ste    Fai
        /*Normal*/ new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 0, 1f, 1f, 0.5f, 1f },
        /*Fire*/ new[] { 1f, 0.5f, 0.5f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 1f, 2f, 1f },
        /*Water*/ new[] { 1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f, 1f },
        /*Electric*/new[] { 1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 0f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f },
        /*Grass*/ new[] { 1f, 0.5f, 2f, 1f, 0.5f, 1f, 1f, 0.5f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 0.5f, 1f, 0.5f, 1f },
        /*Ice*/ new[] { 1f, 0.5f, 0.5f, 1f, 2f, 0.5f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f },
        /*Fighting*/new[] { 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 0.5f, 0.5f, 2f, 0f, 1f, 2f, 2f, 0.5f },
        /*Poison*/ new[] { 1f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 0f, 2f },
        /*Ground*/ new[] { 1f, 2f, 1f, 2f, 0.5f, 1f, 1f, 2f, 1f, 0f, 1f, 0.5f, 2f, 1f, 1f, 1f, 2f, 1f },
        /*Flying*/ new[] { 1f, 1f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 0.5f, 1f },
        /*Psychic*/ new[] { 1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 0.5f, 1f, 1f, 1f, 1f, 0f, 0.5f, 1f },
        /*Bug*/ new[] { 1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f, 0.5f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 1f, 2f, 0.5f, 0.5f },
        /*Rock*/ new[] { 1f, 2f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f },
        /*Ghost*/ new[] { 0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f },
        /*Dragon*/ new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 0f },
        /*Dark*/ new[] { 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f },
        /*Steel*/ new[] { 1f, 0.5f, 0.5f, 0.5f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 0.5f, 2f },
        /*Fairy*/ new[] { 1f, 0.5f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 0.5f, 1f }
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1f;

        var row = (int)attackType - 1;
        var col = (int)defenseType - 1;

        return chart[row][col];
    }
}