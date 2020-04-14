using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using com.xenturio.entity;
using com.xenturio.enums;

namespace com.xenturio.basegame
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] protected Text currentPlayerText;
        [SerializeField] protected Text selectedTerritoryText;
        [SerializeField] protected Text targetTerritoryText;
        [SerializeField] protected Image arrowFromTo;
        [SerializeField] protected Button attackMoveBtn;
        [SerializeField] protected AudioSource endTurnSound;
        [SerializeField] protected Slider moveSlider;
        [Tooltip("The prefab to use for representing the player")]
        [SerializeField] protected GameObject playerPrefab;
        [SerializeField] protected GameObject loaderCanvas;
        [SerializeField] protected Text localPlayerText;
        [SerializeField] protected Text localPlayerArmiesCount;
        [SerializeField] protected Text localPlayerTerritoriesCountText;
        [SerializeField] protected GameObject territoriesContainer;

        protected List<PlayerController> players = new List<PlayerController>();

        protected PlayerController localPlayerController;

        protected PlayerController currentPlayerController;

        protected PlayerController selectedDefenderController;

        protected TerritoryController selectedTerritoy;

        //Territorio selezionato da attaccare o come destinazione di un movimento
        protected TerritoryController destinationTerritory;

        protected TerritoryController[] territories;

        protected int allStartArmies = 0;

        protected int armiesToMove = 0;

        protected LevelLoader levelLoader;

        public PlayerController CurrentPlayerController { get => currentPlayerController; set => currentPlayerController = value; }
        public PlayerController SelectedDefenderController { get => selectedDefenderController; set => selectedDefenderController = value; }
        public TerritoryController SelectedTerritoy { get => selectedTerritoy; set => selectedTerritoy = value; }
        public TerritoryController DestinationTerritory { get => destinationTerritory; set => destinationTerritory = value; }

        void Awake()
        {
            DontDestroyOnLoad(this);
            levelLoader = FindObjectOfType<LevelLoader>();
            selectedTerritoryText.gameObject.SetActive(false);
            targetTerritoryText.gameObject.SetActive(false);
            arrowFromTo.gameObject.SetActive(false);
            attackMoveBtn.gameObject.SetActive(false);
            moveSlider.gameObject.SetActive(false);
            territories = territoriesContainer.GetComponentsInChildren<TerritoryController>();
        }
        // Start is called before the first frame update
        void Start()
        {           
            StartGame();
            localPlayerText.text = PlayerPrefsController.GetPlayerNickname();
        }

        public void StartGame()
        {
            CreatePlayers();
            loaderCanvas.gameObject.SetActive(false);
            if (GameStatesController.IsNotStartedGame())
            {
                SetupMatch();
            }
            if (GameStatesController.IsSetupGame())
            {
                GameStatesController.NextState();
                currentPlayerController.CalcReinforcmentArmies();
                UpdateTerritoriesInfo();
            }
        }


        #region SETUP GAME

        protected void CreatePlayers()
        {
            GameObject localPl = Instantiate(playerPrefab, transform, true);
            localPl.gameObject.GetComponent<PlayerController>().Player.SetPlayerName(PlayerPrefsController.GetPlayerNickname());
            localPl.gameObject.GetComponent<PlayerController>().PickUpColor(PlayerPrefsController.GetPlayerColor());
            players.Add(localPl.gameObject.GetComponent<PlayerController>());
            localPlayerController = localPl.gameObject.GetComponent<PlayerController>();

            var numberPlayers = PlayerPrefsController.GetNumberPlayers();
            if (numberPlayers == 0) {
                numberPlayers = 6;
            }
            for (var i = 0; i < numberPlayers - 1; i++)
            {
                GameObject newPlayer = Instantiate(playerPrefab, transform, true);
                newPlayer.gameObject.GetComponent<PlayerController>().Player.SetPlayerName("Giocatore " + (i + 1));
                players.Add(newPlayer.gameObject.GetComponent<PlayerController>());
            }
        }


        protected void SetupMatch()
        {
            //Scelta dell'ordine dei giocatori random
            PickUpPlayersOrder();
            //Scelta random dei colori solo se configurato
            RandomizeColorPlayer();
            //Seleziono il primo giocatore
            SetNextPlayer();
            //Mischio i territori
            ShuffleTerritories();
            //Distribuisco le carte territorio
            DistributeTerritories();
            //Distribuisco le carte obbiettivo
            DistributeTargetCard();
            //Distribuisco le armate
            DistributeArmies();
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

        private void ShuffleTerritories()
        {
            territories.Shuffle();
        }

        protected void DistributeTerritories()
        {
            if (players != null && players.Count > 0)
            {
                var playerIndex = 0;
                foreach (TerritoryController territory in territories)
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
        }

        protected void DistributeTargetCard()
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

        protected void DistributeArmies()
        {
            int startArmies = 20 + ((6 - players.Count) * 5);
            allStartArmies = startArmies * players.Count;
            foreach (PlayerController player in players)
            {
                player.AddArmies(startArmies);
                player.SetStartArmiesCount(startArmies - player.GetTerritoriesOwned().Count);
            }
        }
      
        protected void RandomizeColorPlayer()
        {
            if (GameSettings.RANDOMIZE_COLOR_PLAYER)
            {
                List<Color> colors = new List<Color>(GameSettings.playerColors);
                colors.Shuffle();
                var i = 0;
                foreach (PlayerController player in players)
                {
                    if (player.GetPlayerColor() == null || player.GetPlayerColor().Equals(Color.clear))
                    {
                        player.PickUpColor(colors[i]);
                    }
                    i++;
                }
            }
        }

        protected void PickUpPlayersOrder()
        {
            players.Shuffle();
        }

        protected void AutoStartPositioning()
        {
            if (GameSettings.AUTO_START_POSITIONING_ARMY)
            {
                int hasArmies = players.Count;
                while (hasArmies > 0)
                {
                    if (currentPlayerController.GetArmiesPerTurn() <= 0)
                    {
                        hasArmies--;
                    }
                    else
                    {
                        while (currentPlayerController.GetArmiesPerTurn() > 0)
                        {
                            currentPlayerController.GetTerritoriesOwned()[UnityEngine.Random.Range(0, currentPlayerController.GetTerritoriesOwned().Count)].gameObject.GetComponent<TerritoryController>().AddArmy();
                        }
                    }
                    EndPlayerTurn();
                }

            }
        }
        #endregion

        public void SetNextPlayer()
        {
            if (this.currentPlayerController == null && players.Count > 0)
            {
                this.currentPlayerController = players[0];
            }
            else if (this.currentPlayerController != null)
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
        }

        public void SetDefender(PlayerController defender)
        {
            this.selectedDefenderController = defender;
        }
        
        protected void SetMoving()
        {
            GameStatesController.SetMoving();
            UpdateAttackMoveInfo();
        }

        public void EndPlayerTurn()
        {
            if (currentPlayerController && currentPlayerController.GetArmiesPerTurn() <= 0)
            {
                if (GameStatesController.IsMove())
                {
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
        }



        protected void UpdateTerritoriesInfo()
        {
            foreach (TerritoryController territoryController in territories)
            {
                territoryController.UpdateTerritoryAtEndTurn();
            }
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
            this.selectedDefenderController = territory != null ? territory.GetOwnerController() : null;
            UpdateAttackMoveInfo();
        }

        public TerritoryController GetDestinationTerritory()
        {
            return this.destinationTerritory;
        }

        protected void StartAttack()
        {
            levelLoader.StartBattleField();
        }

        protected void CompleteMove() {
            this.SelectedTerritoy.RemoveArmies((int)moveSlider.value);
            this.DestinationTerritory.AddArmies((int)moveSlider.value);
            GameStatesController.NextState();
            moveSlider.value = 0;
            moveSlider.gameObject.SetActive(false);
            UpdateAttackMoveInfo();
            EndPlayerTurn();
        }

        protected bool CheckToStartGame()
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


        #region Handle UI
        public void HandleContinueButton()
        {
            if (currentPlayerController.GetArmiesPerTurn() <= 0)
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

        public void HandleAttackMoveBtn()
        {
            if (GameStatesController.IsAttack())
            {
                StartAttack();
            }
            else if (GameStatesController.IsMove())
            {
                if (this.selectedTerritoy != null && this.destinationTerritory != null)
                {
                    SetMoving();
                }
            }
            else if (GameStatesController.IsMoving())
            {
                CompleteMove();
            }
        }

        public void HandleSliderMove()
        {
            if (selectedTerritoy.Territory.GetArmies() > 1)
            {
                armiesToMove = (int)moveSlider.value;
                moveSlider.GetComponentInChildren<Text>().text = moveSlider.value.ToString();
            }
        }

        public void HandleExitButton() {
            levelLoader.LoadMainMenu();
        }

        protected void UpdateAttackMoveInfo()
        {

            if (GameStatesController.IsMoving())
            {
                moveSlider.gameObject.SetActive(true);
                attackMoveBtn.gameObject.SetActive(true);
                moveSlider.maxValue = selectedTerritoy.Territory.GetArmies() - 1;
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
                else
                {
                    targetTerritoryText.gameObject.SetActive(false);
                    arrowFromTo.gameObject.SetActive(false);
                    attackMoveBtn.gameObject.SetActive(false);
                }
            }
            else
            {
                selectedTerritoryText.gameObject.SetActive(false);
                targetTerritoryText.gameObject.SetActive(false);
                arrowFromTo.gameObject.SetActive(false);
                attackMoveBtn.gameObject.SetActive(false);
            }

        }
        #endregion
        public bool IsMyTurn()
        {
            return currentPlayerController != null && currentPlayerController.GetPlayer() != null && localPlayerText.text.Equals(currentPlayerController.GetPlayer().GetPlayerName());
        }

        public void RaiseEvent(object data, byte code, object subject, ExitGames.Client.Photon.Hashtable evData, bool master) { }

    }
}
