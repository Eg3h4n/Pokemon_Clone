using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string name;

    [TextArea] [SerializeField] private string description;

    [SerializeField] private PokemonType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private bool alwaysHits;
    [SerializeField] private int pp;
    [SerializeField] private int priority;
    [SerializeField] private MoveCategory category;
    [SerializeField] private MoveEffects effects;
    [SerializeField] private List<SecondaryEffects> secondaryEffects;
    [SerializeField] private MoveTarget target;

    public string Name => name;

    public string Description => description;

    public PokemonType Type => type;

    public int Power => power;

    public int Accuracy => accuracy;

    public bool AlwaysHits => alwaysHits;

    public int Pp => pp;

    public int Priority => priority;

    public MoveCategory Category => category;

    public MoveEffects Effects => effects;

    public List<SecondaryEffects> SecondaryEffects => secondaryEffects;

    public MoveTarget Target => target;
}

[Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatBoost> boosts;
    [SerializeField] private ConditionID status;
    [SerializeField] private ConditionID volatileStatus;

    public List<StatBoost> Boosts => boosts;

    public ConditionID Status => status;

    public ConditionID VolatileStatus => volatileStatus;
}

[Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] private int chance;
    [SerializeField] private MoveTarget target;

    public int Chance => chance;

    public MoveTarget Target => target;
}

[Serializable]
public class StatBoost
{
    public Stat Stat;
    public int Boost;
}

public enum MoveCategory
{
    Physical,
    Special,
    Status
}

public enum MoveTarget
{
    Foe,
    Self
}