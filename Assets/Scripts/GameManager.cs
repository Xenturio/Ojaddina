using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text currentPlayerText;
    [SerializeField] Text selectedTerritoryText;
    [SerializeField] Text targetTerritoryText;
    [SerializeField] Image arrowFromTo;
    [SerializeField] Button attackMoveBtn;
    [SerializeField] AudioSource endTurnSound;
    [SerializeField] Slider moveSlider;

    private List<PlayerController> players;

    private Player currentPlayer;

    private PlayerController currentPlayerController;

    private PlayerController selectedDefenderController;

    private TerritoryController selectedTerritoy;

    //Territorio selezionato da attaccare o come destinazione di un movimento
    private TerritoryController destinationTerritory;

    private TerritoryController[] territories;

    private int allStartArmies = 0;

    private int armiesToMove = 0;

    private LevelLoader levelLoader;

    public PlayerController CurrentPlayerController { get => currentPlayerController; set => currentPlayerController = value; }
    public PlayerController SelectedDefenderController { get => selectedDefenderController; set => selectedDefenderController = value; }
    public TerritoryController SelectedTerritoy { get => selectedTerritoy; set => selectedTerritoy = value; }
    public TerritoryController DestinationTerritory { get => destinationTerritory; set => destinationTerritory = value; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        levelLoader = FindObjectOfType<LevelLoader>();
        selectedTerritoryText.gameObject.SetActive(false);
        targetTerritoryText.gameObject.SetActive(false);
        arrowFromTo.gameObject.SetActive(false);
        attackMoveBtn.gameObject.SetActive(false);
        moveSlider.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        players = new List<PlayerController>(FindObjectsOfType<PlayerController>());
        if (GameStatesController.IsNotStartedGame())
        {
            SetupMatch();
        }
        else {
            //LoadGameObjects();
        }
        StartGame();
    }

   

    // Update is called once per frame
    void Update()
    {

    }

    private void SetupMatch()
    {
        //Scelta dell'ordine dei giocatori random
        PickUpPlayersOrder();
        //Scelta random dei colori solo se configurato
        RandomizeColorPlayer();
        //Seleziono il primo giocatore
        SetNextPlayer();
        //Distribuisco le carte territorio
        DistributeTerritories();
        //Distribuisco le carte obbiettivo
        DistributeTargetCard();
        //Distribuisco le armate
        DistributeArmies();
        //Si aggiunge un armata per ogni territorio
        AddFirstArmyInTerritory();
        //Inizializza le posizioni 
        StartPositioning();
    }

    public void StartPositioning()
    {
        GameStatesController.StartGameState();
        currentPlayerController.CalcReinforcmentArmies();
        //Dispone automaticamente le armate
        AutoStartPositioning();
    }

    public void StartGame()
    {
        if (GameStatesController.IsSetupGame()){            
            GameStatesController.NextState();
            currentPlayerController.CalcReinforcmentArmies();
            UpdateTerritoriesInfo();
        }
    }

    private void DistributeTerritories()
    {

        territories = FindObjectsOfType<TerritoryController>();
        List<TerritoryController> randomTerritories = new List<TerritoryController>(territories);
        randomTerritories.Shuffle();
        var playerIndex = 0;
        foreach (TerritoryController territory in randomTerritories)
        {
            if (playerIndex == players.Count)
            {
                playerIndex = 0;
            }
            players[playerIndex].AddTerritory(territory.GetTerritory());
            territory.SetOwner(players[playerIndex]);
            playerIndex++;
        }
    }

    private void DistributeTargetCard()
    {
        if (!GameSettings.DOMINATION_MODE)
        {
            TargetCard[] allTargetsCard = FindObjectsOfType<TargetCard>();
            List<TargetCard> randomCards = new List<TargetCard>(allTargetsCard.Length);
            foreach (TargetCard card in allTargetsCard)
            {
                randomCards.Add(card);
            }
            randomCards.Shuffle();
            var cardIndex = 0;
            foreach (PlayerController player in players)
            {
                player.SetTargetCard(randomCards[cardIndex]);
                cardIndex++;
            }

        }
    }

    private void DistributeArmies()
    {
        int startArmies = 20 + ((6 - players.Count) * 5);
        allStartArmies = startArmies * players.Count;
        foreach (PlayerController player in players)
        {
            player.AddArmies(startArmies);
            player.SetStartArmiesCount(startArmies);
        }
    }

    private void AddFirstArmyInTerritory()
    {
        foreach (TerritoryController territory in territories)
        {
            territory.AddArmy();
            territory.GetTerritory().GetPlayer().SetStartArmiesCount(territory.GetTerritory().GetPlayer().GetStartArmiesCount() - 1);
        }
    }

    private void RandomizeColorPlayer()
    {
        if (GameSettings.RANDOMIZE_COLOR_PLAYER)
        {
            List<Color> colors = new List<Color>(GameSettings.playerColors);
            colors.Shuffle();
            var i = 0;
            foreach (PlayerController player in players)
            {
                player.PickUpColor(colors[i]);
                i++;
            }
        }
    }

    private void PickUpPlayersOrder()
    {
        players.Shuffle();
    }

    public void SetNextPlayer()
    {
        if (this.currentPlayerController == null)
        {
            this.currentPlayerController = players[0];
        }
        else
        {
            var currentIndex = 0;
            foreach (PlayerController playerC in players)
            {
                if (currentPlayerController.Equals(playerC))
                {
                    break;
                }
                currentIndex++;
            }
            if (currentIndex == players.Count - 1)
            {
                this.currentPlayerController = players[0];
            }
            else
            {
                this.currentPlayerController = players[currentIndex + 1];
            }
        }
        this.currentPlayer = this.currentPlayerController.GetPlayer();
    }

    public void AddPlayer(PlayerController player)
    {
        players.Add(player);
    }

    public void RemovePlayer(PlayerController player)
    {
        players.Remove(player);
    }

    private void SetMoving() {        
        GameStatesController.SetMoving();
        UpdateAttackMoveInfo();
    }

    public void EndPlayerTurn()
    {
        if (currentPlayer.GetArmiesPerTurn() <= 0)
        {
            if (GameStatesController.IsMove()) {
                GameStatesController.NextState();
            }
            SetNextPlayer();
            currentPlayerController.CalcReinforcmentArmies();
            selectedTerritoy = null;
            DestinationTerritory = null;
            SelectedDefenderController = null;
            if (!CheckToStartGame())
            {
                endTurnSound.Play();
            }
        }
        else
        {
            Debug.Log("Use all of your armies first");
        }
    }

    public void HandleContinueButton() {
        if (currentPlayer.GetArmiesPerTurn() <= 0)
        {
            UpdateAttackMoveInfo();
            UpdateTerritoriesInfo();
            if (GameStatesController.IsSetupGame() || GameStatesController.IsMove() || GameStatesController.IsNotStartedGame())
            {                
                EndPlayerTurn();
            }
            else
            {
               
                GameStatesController.NextState();
            }
        }

    }

    private void UpdateTerritoriesInfo()
    {
        foreach (TerritoryController territoryController in territories) {
            territoryController.UpdateTerritoryAtEndTurn();            
        }
    }

    public Player GetCurrentPlayer()
    {
        return this.currentPlayer;
    }

    public TerritoryController GetSelectedTerritory()
    {
        return this.selectedTerritoy;
    }

    public void SetSelectedTerritory(TerritoryController territory)
    {
        this.selectedTerritoy = territory;
        UpdateAttackMoveInfo();
    }

    public void SetDesinationTerritory(TerritoryController territory)
    {
        this.destinationTerritory = territory;
        UpdateAttackMoveInfo();
    }

    public TerritoryController GetDestinationTerritory()
    {
        return this.destinationTerritory;
    }

    private bool CheckToStartGame()
    {
        if (GameStatesController.IsSetupGame())
        {
            bool canStart = true;
            foreach (PlayerController player in players)
            {
                if (player.GetStartArmiesCount() > 0)
                {
                    canStart = false;
                    break;
                }
            }
            return canStart;
        }
        else
        {
            return false;
        }
    }

    private void AutoStartPositioning()
    {
        if (GameSettings.AUTO_START_POSITIONING_ARMY)
        {
            int hasArmies = players.Count;
            while(hasArmies > 0)
            {
                if (currentPlayerController.GetArmiesPerTurn() <= 0)
                {
                    hasArmies--;
                }
                else
                {
                    while (currentPlayerController.GetArmiesPerTurn() > 0)
                    {
                        currentPlayerController.GetTerritoriesOwned()[UnityEngine.Random.Range(0, currentPlayerController.GetTerritoriesOwned().Count)].GetTerritoryController().AddArmy();
                    }
                }
                EndPlayerTurn();
            }
            
        }
    }

    public void HandleAttackMoveBtn() {

        if (GameStatesController.IsAttack())
        {
            this.selectedDefenderController = destinationTerritory.GetOwnerController();
            levelLoader.StartBattleField();
        }
        else if (GameStatesController.IsMove())
        {
            if (this.selectedTerritoy != null && this.destinationTerritory != null)
            {
                SetMoving();
            }
        }
        else if (GameStatesController.IsMoving()) {
            this.SelectedTerritoy.RemoveArmies((int)moveSlider.value);
            this.DestinationTerritory.AddArmies((int)moveSlider.value);
            GameStatesController.NextState();
            moveSlider.value = 0;
            moveSlider.gameObject.SetActive(false);            
            UpdateAttackMoveInfo();           
            EndPlayerTurn();
        }
    }

    public void HandleSliderMove() {
        if (selectedTerritoy.Territory.Armies > 1)
        {
            armiesToMove = (int) moveSlider.value;
            moveSlider.GetComponentInChildren<Text>().text = moveSlider.value.ToString();
        }
    }

    private void UpdateAttackMoveInfo() {

        if (GameStatesController.IsMoving()) {
            moveSlider.gameObject.SetActive(true);
            attackMoveBtn.gameObject.SetActive(true);
            moveSlider.maxValue = selectedTerritoy.Territory.Armies - 1;
            if (attackMoveBtn.GetComponentInChildren<Text>())
            {
                attackMoveBtn.GetComponentInChildren<Text>().text = "CONFIRM";
            }
            return;
        }
        if (GameStatesController.IsAttack() || GameStatesController.IsMove())
        {
            if (selectedTerritoy != null)
            {
                selectedTerritoryText.gameObject.SetActive(true);
                selectedTerritoryText.text = selectedTerritoy.GetTerritory().GetTerritoryName();
            }
            else
            {
                selectedTerritoryText.gameObject.SetActive(false);
                targetTerritoryText.gameObject.SetActive(false);
                arrowFromTo.gameObject.SetActive(false);
                attackMoveBtn.gameObject.SetActive(false);
                return;
            }
            if (destinationTerritory != null)
            {
                targetTerritoryText.gameObject.SetActive(true);
                targetTerritoryText.text = destinationTerritory.GetTerritory().GetTerritoryName();
                arrowFromTo.gameObject.SetActive(true);
                attackMoveBtn.gameObject.SetActive(true);
                if (attackMoveBtn.GetComponentInChildren<Text>())
                {
                    attackMoveBtn.GetComponentInChildren<Text>().text = GameStatesController.IsAttack() ? "ATTACK" : "MOVE";
                }
            }
            else {
                targetTerritoryText.gameObject.SetActive(false);
                arrowFromTo.gameObject.SetActive(false);
                attackMoveBtn.gameObject.SetActive(false);
            }
        }
        else {
            selectedTerritoryText.gameObject.SetActive(false);
            targetTerritoryText.gameObject.SetActive(false);
            arrowFromTo.gameObject.SetActive(false);
            attackMoveBtn.gameObject.SetActive(false);
        }

    }

    public void SetDefender(PlayerController defender) {
        this.selectedDefenderController = defender;
    }

}

static class MyExtensions
{

    public static void Shuffle<T>(this IList<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (System.Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
