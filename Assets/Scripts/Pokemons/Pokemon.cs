using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Pokemon
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private int level;

    public Pokemon(PokemonBase pokemonBase, int level)
    {
        _base = pokemonBase;
        this.level = level;

        Init();
    }

    public PokemonBase Base => _base;

    public int Level => level;

    public int HP { get; set; }

    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public Condition VolatileStatus { get; private set; }
    public int StatusTime { get; set; }
    public int VolatileStatusTime { get; set; }
    public Queue<string> StatusChanges { get; private set; } = new();
    public bool HPChanged { get; set; }

    public int Attack => GetStat(Stat.Attack);

    public int Defense => GetStat(Stat.Defense);

    public int SpAttack => GetStat(Stat.SpAttack);

    public int SpDefense => GetStat(Stat.SpDefense);

    public int Speed => GetStat(Stat.Speed);

    public int MaxHP { get; private set; }

    public event Action OnStatusChanged;

    public void Init()
    {
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));

            if (Moves.Count >= 4)
                break;
        }

        CalculateStats();
        HP = MaxHP;

        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    private void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>
        {
            { Stat.Attack, Mathf.FloorToInt(Base.Attack * Level / 100f) + 5 },
            { Stat.Defense, Mathf.FloorToInt(Base.Defense * Level / 100f) + 5 },
            { Stat.SpAttack, Mathf.FloorToInt(Base.SpAttack * Level / 100f) + 5 },
            { Stat.SpDefense, Mathf.FloorToInt(Base.SpDefense * Level / 100f) + 5 },
            { Stat.Speed, Mathf.FloorToInt(Base.Speed * Level / 100f) + 5 }
        };

        MaxHP = Mathf.FloorToInt(Base.MaxHP * Level / 100f) + 10 + Level;
    }

    private void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.SpAttack, 0 },
            { Stat.SpDefense, 0 },
            { Stat.Speed, 0 },
            { Stat.Accurracy, 0 },
            { Stat.Evasion, 0 }
        };
    }

    private int GetStat(Stat stat)
    {
        var statVal = Stats[stat];

        // Stat boost magic
        var boost = StatBoosts[stat];
        var boostValues = new[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.Stat;
            var boost = statBoost.Boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        var critical = 1f;
        if (Random.value * 100f <= 6.25)
            critical = 2f;

        var type = TypeChart.GetEffectiveness(move.Base.Type, Base.Type1) *
                   TypeChart.GetEffectiveness(move.Base.Type, Base.Type2);

        var damageDetails = new DamageDetails
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = move.Base.Category == MoveCategory.Special ? attacker.SpAttack : attacker.Attack;
        float defense = move.Base.Category == MoveCategory.Special ? SpDefense : Defense;

        var modifiers = Random.Range(0.85f, 1f) * type * critical;
        var a = (2 * attacker.Level + 10) / 250f;
        var d = a * move.Base.Power * (attack / defense) + 2;
        var damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);

        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        HPChanged = true;
    }

    public void SetStatus(ConditionID conditionId)
    {
        if (Status != null) return;

        Status = ConditionsDB.Condtions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionId)
    {
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionsDB.Condtions[conditionId];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public Move GetRandomMove()
    {
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();

        var r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public bool OnBeforeMove()
    {
        var canPerformMove = true;

        if (Status?.OnBeforeMove != null)
            if (!Status.OnBeforeMove(this))
                canPerformMove = false;

        if (VolatileStatus?.OnBeforeMove != null)
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;

        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}