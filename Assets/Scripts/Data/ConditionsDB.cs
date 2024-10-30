using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Condtions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionID, Condition> Condtions { get; set; } = new Dictionary<ConditionID, Condition>
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHP / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to poison");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned",
                OnAfterTurn = (Pokemon pokemon) =>
                {
                    pokemon.UpdateHP(pokemon.MaxHP / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to burn");
                }
            }
        },
        {
           ConditionID.par,
           new Condition()
           {
               Name = "Paralyzed",
               StartMessage = "has been paralyzed",
               OnBeforeMove = (Pokemon pokemon) =>
               {
                   if (Random.Range(0, 5) == 1)
                   {
                       pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s paralyzed and can't move...");
                       return false;
                   }

                   return true;
               }
           }
        },
        {
           ConditionID.frz,
           new Condition()
           {
               Name = "Freeze",
               StartMessage = "has been frozen",
               OnBeforeMove = (Pokemon pokemon) =>
               {
                   if (Random.Range(0, 5) == 1)
                   {
                       pokemon.CureStatus();
                       pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s not frozen anymore!");
                       return true;
                   }

                   return false;
               }
           }
        },
        {
           ConditionID.slp,
           new Condition()
           {
               Name = "Sleep",
               StartMessage = "has fallen asleep",
               OnStart = (Pokemon pokemon) =>
               {
                   // Sleep for 1-3 turns
                   pokemon.StatusTime = Random.Range(1, 4);
                   Debug.Log($"Will be asleep for {pokemon.StatusTime} turns");
               },
               OnBeforeMove = (Pokemon pokemon) =>
               {
                   if(pokemon.StatusTime <= 0)
                   {
                       pokemon.CureStatus();
                       pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                       return true;

                   }
                   pokemon.StatusTime--;
                   pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping...");
                   return false;
               }
           }
        },
        {
           ConditionID.cnf,
           new Condition()
           {
               Name = "Confusion",
               StartMessage = "has been confused",
               OnStart = (Pokemon pokemon) =>
               {
                   // Confused for 1-3 turns
                   pokemon.VolatileStatusTime = Random.Range(1, 5);
                   Debug.Log($"Will be confused for {pokemon.VolatileStatusTime} turns");
               },
               OnBeforeMove = (Pokemon pokemon) =>
               {
                   if(pokemon.VolatileStatusTime <= 0)
                   {
                       pokemon.CureVolatileStatus();
                       pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} kicked out of confusion!");
                       return true;

                   }
                   pokemon.VolatileStatusTime--;

                   // 50% chance to do a move
                   if(Random.Range(1, 3) == 1)
                       return true;

                   // Hurt by confusion
                   pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused");
                   pokemon.UpdateHP(pokemon.MaxHP / 8);
                   pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to confusion");
                   return false;
               }
           }
        }
    };
}

public enum ConditionID
{
    none,
    psn,
    brn,
    slp,
    par,
    frz,
    cnf
}