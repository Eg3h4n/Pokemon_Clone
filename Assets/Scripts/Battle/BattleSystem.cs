using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    PokemonSelection,
    BattleOver
}

public enum BattleAction
{
    Move,
    SwitchPokemon,
    UseItem,
    Run
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleDialogBox battleDialogBox;
    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image trainerImage;
    private int _currentAction;
    private int _currentMember;
    private int _currentMove;

    private bool _isTrainerBattle;
    private PlayerController _player;

    private PokemonParty _playerParty;
    private BattleState? _prevState;

    private BattleState _state;
    private TrainerController _trainer;
    private PokemonParty _trainerParty;
    private Pokemon _wildPokemon;

    public event Action<bool> OnBattleOver;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        _playerParty = playerParty;
        _wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        _playerParty = playerParty;
        _trainerParty = trainerParty;
        _isTrainerBattle = true;

        _player = _playerParty.GetComponent<PlayerController>();
        _trainer = _trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if (_isTrainerBattle) // Trainer battle
        {
            // Displaying trainer sprites and dialog
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite = _player.PlayerSprite;
            trainerImage.sprite = _trainer.TrainerSprite;

            yield return battleDialogBox.TypeDialog($"{_trainer.TrainerName} wants to battle!");

            // Trainer sends out first pokemon
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = _trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return battleDialogBox.TypeDialog($"{_trainer.TrainerName} chose {enemyPokemon.Base.Name}!");

            // Player sends out first pokemon
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = _playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return battleDialogBox.TypeDialog($"GO {playerPokemon.Base.Name}!");
            battleDialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }
        else // Wild Pokemon encounter
        {
            playerUnit.Setup(_playerParty.GetHealthyPokemon());
            enemyUnit.Setup(_wildPokemon);
            battleDialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

            yield return battleDialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");
        }

        partyScreen.Init();


        ActionSelection();
    }

    private void BattleOver(bool won)
    {
        _state = BattleState.BattleOver;
        _playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }

    private void ActionSelection()
    {
        _state = BattleState.ActionSelection;
        battleDialogBox.SetDialog("Choose an action");
        battleDialogBox.EnableActionSelector(true);
    }

    private void OpenPartyScreen()
    {
        _state = BattleState.PokemonSelection;
        partyScreen.SetPartyData(_playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    private void MoveSelection()
    {
        _state = BattleState.MoveSelection;
        battleDialogBox.EnableActionSelector(false);
        battleDialogBox.EnableDialogText(false);
        battleDialogBox.EnableMoveSelector(true);
    }

    private IEnumerator RunTurns(BattleAction playerAction)
    {
        _state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[_currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            var playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            var enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            // check who gets first move
            var playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;

            var firstUnit = playerGoesFirst ? playerUnit : enemyUnit;
            var secondUnit = playerGoesFirst ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;

            // First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (_state == BattleState.BattleOver) yield break;

            if (secondPokemon.HP > 0)
            {
                // Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (_state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = _playerParty.Pokemons[_currentMember];
                _state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }

            // Enemy Turn
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(enemyUnit);
            if (_state == BattleState.BattleOver) yield break;
        }

        if (_state != BattleState.BattleOver)
            ActionSelection();
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        var canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }

        yield return ShowStatusChanges(sourceUnit.Pokemon);

        move.PP--;

        yield return battleDialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            targetUnit.PlayHitAnimation();

            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon,
                    move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Pokemon.HP > 0)
                foreach (var item in move.Base.SecondaryEffects)
                {
                    var rnd = Random.Range(1, 101);
                    if (rnd <= item.Chance)
                        yield return RunMoveEffects(item, sourceUnit.Pokemon, targetUnit.Pokemon, item.Target);
                }

            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return battleDialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} fainted...");
                targetUnit.PlayFaintAnimation();

                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);
            }
        }
        else
        {
            yield return battleDialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed...");
        }
    }

    private IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {
        // Stat Boosting applied here
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.Boosts);
            else
                target.ApplyBoosts(effects.Boosts);
        }

        // Status Effects are applied here
        if (effects.Status != ConditionID.none) target.SetStatus(effects.Status);

        // Volatile Status Effects are applied here
        if (effects.VolatileStatus != ConditionID.none) target.SetVolatileStatus(effects.VolatileStatus);

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (_state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => _state == BattleState.RunningTurn);
        // status effects like burn and psn will hurt the pokemon after the 
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.UpdateHP();

        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return battleDialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} fainted...");
            sourceUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
        }
    }

    private bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        var accuracy = source.StatBoosts[Stat.Accurracy];
        var evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return Random.Range(1, 101) <= moveAccuracy;
    }

    private IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();

            yield return battleDialogBox.TypeDialog(message);
        }
    }

    private void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = _playerParty.GetHealthyPokemon();

            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
        {
            if (!_isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextPokemon = _trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                    StartCoroutine(SendNextTrainerPokemon(nextPokemon));
                else
                    BattleOver(true);
            }
        }
    }

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return battleDialogBox.TypeDialog("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return battleDialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return battleDialogBox.TypeDialog("It's not very effective...");
    }

    public void HandleUpdate()
    {
        if (_state == BattleState.ActionSelection)
            HandleActionSelection();
        else if (_state == BattleState.MoveSelection)
            HandleMoveSelection();
        else if (_state == BattleState.PokemonSelection) HandlePokemonSelection();
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            _currentAction++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            _currentAction--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            _currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) _currentAction -= 2;

        _currentAction = Mathf.Clamp(_currentAction, 0, 3);

        battleDialogBox.UpdateActionSelection(_currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (_currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (_currentAction == 1)
            {
                // Bag
            }
            else if (_currentAction == 2)
            {
                // Pokemon
                _prevState = _state;
                OpenPartyScreen();
            }
            else if (_currentAction == 3)
            {
                // Run
            }
        }
    }

    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++_currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --_currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            _currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) _currentMove -= 2;

        _currentMove = Mathf.Clamp(_currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);


        battleDialogBox.UpdateMoveSelection(_currentMove, playerUnit.Pokemon.Moves[_currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.Pokemon.Moves[_currentMove];
            if (move.PP == 0) return;

            battleDialogBox.EnableMoveSelector(false);
            battleDialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            battleDialogBox.EnableMoveSelector(false);
            battleDialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    private void HandlePokemonSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++_currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --_currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            _currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) _currentMember -= 2;

        _currentMember = Mathf.Clamp(_currentMember, 0, _playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(_currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = _playerParty.Pokemons[_currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText($"{selectedMember.Base.Name} is fainted, can't fight...");
                return;
            }

            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText($"{selectedMember.Base.Name} is already fighting...");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (_prevState == BattleState.ActionSelection)
            {
                _prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                _state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    private IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return battleDialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);

        battleDialogBox.SetMoveNames(newPokemon.Moves);

        yield return battleDialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        _state = BattleState.RunningTurn;
    }

    private IEnumerator SendNextTrainerPokemon(Pokemon nextPokemon)
    {
        _state = BattleState.Busy;

        enemyUnit.Setup(nextPokemon);
        yield return battleDialogBox.TypeDialog($"{_trainer.TrainerName} sends out {nextPokemon.Base.Name}!");

        _state = BattleState.RunningTurn;
    }
}