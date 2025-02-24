using UnityEngine;

public enum GameState
{
    FreeRoam,
    Battle,
    Dialog,
    Cutscene
}

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private Camera worldCamera;
    private TrainerController _trainer;

    private GameState state;

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        ConditionsDB.Init();
    }

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        playerController.OnEnterTrainersFoV += StartTrainerEncounter;

        DialogManager.Instance.OnShowDialog += StartDialog;
        DialogManager.Instance.OnHideDialog += EndDialog;
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
            playerController.HandleUpdate();
        else if (state == GameState.Battle)
            battleSystem.HandleUpdate();
        else if (state == GameState.Dialog) DialogManager.Instance.HandleUpdate();
    }

    private void StartTrainerEncounter(Collider2D trainerCollider)
    {
        var trainer = trainerCollider.GetComponentInParent<TrainerController>();
        if (trainer != null)
        {
            state = GameState.Cutscene;
            StartCoroutine(trainer.TriggerTrainerBattle(playerController));
        }
    }

    private void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);

        battleSystem.StartBattle(playerParty, wildPokemonCopy);
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        _trainer = trainer;
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var trainerParty = _trainer.GetComponent<PokemonParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    private void EndBattle(bool won)
    {
        if (_trainer != null && won)
        {
            _trainer.GotDefeated();
            _trainer = null;
        }

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void StartDialog()
    {
        state = GameState.Dialog;
    }

    private void EndDialog()
    {
        if (state == GameState.Dialog)
            state = GameState.FreeRoam;
    }
}