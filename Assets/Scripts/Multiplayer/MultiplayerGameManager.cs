using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.xenturio.entity;
using com.xenturio.enums;
using com.xenturio.basegame;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace com.xenturio.multiplayer
{
    public class MultiplayerGameManager : GameManager
    {
        private bool gameStarted = false;


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
            // in case we started this demo with the wrong scene being active, simply load the menu scene
            if (!PhotonNetwork.IsConnected)
            {
                levelLoader.ConnectNetworkScene();
                return;
            }
            CreatePlayers();
            localPlayerText.text = PlayerPrefsController.GetPlayerNickname();
            localPlayerTerritoriesCountText.text = localPlayerTerritoriesCountText.text.Replace("$1" , localPlayerController.GetTerritoriesOwned().Count.ToString());
        }
        
        // Update is called once per frame
        void Update()
        {
            if (!PhotonNetwork.IsConnected) { return; }
            int playersReady = (int)PhotonNetwork.CurrentRoom.CustomProperties[NetworkCustomProperties.GAME_PLAYERS_LOADED];
            if (playersReady >= PhotonNetwork.CurrentRoom.PlayerCount)
            {                
                StartGame();
            }
            if (GameStatesController.IsNotStartedGame())
            {
                loaderCanvas.gameObject.GetComponentInChildren<Text>().text = "Preparazione gioco...";
            }
            if (GameStatesController.IsSetupGame()) {
                loaderCanvas.gameObject.SetActive(false);
            }
            localPlayerTerritoriesCountText.text = localPlayerTerritoriesCountText.text.Replace("$1", localPlayerController.GetTerritoriesOwned().Count.ToString());
        }

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
        }

        private void OnDisabled()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
        }

        private void OnDestroy()
        {
            Debug.Log("Chi cazzo è stato?");
        }

        new void CreatePlayers() {

            if (players.Count < PhotonNetwork.CurrentRoom.PlayerCount)
            {
               
                foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
                {                    
                    GameObject playerPrefabs = Instantiate(playerPrefab, transform, true);
                    PlayerController newPlayer = playerPrefabs.gameObject.GetComponent<PlayerController>();
                    newPlayer.GetPlayer().SetPlayerName(player.Value.NickName);
                    if (player.Value.CustomProperties[NetworkCustomProperties.PLAYER_COLOR] != null)
                    {
                        newPlayer.PickUpColor(GameSettings.playerColors[(int)player.Value.CustomProperties[NetworkCustomProperties.PLAYER_COLOR]]);
                    }
                    if (PhotonNetwork.LocalPlayer.NickName.Equals(player.Value.NickName))
                    {
                        this.localPlayerController = newPlayer;
                    }
                    players.Add(newPlayer);
                }
                if (players.Count == PhotonNetwork.CurrentRoom.PlayerCount) {
                    SetReady();
                }
            }

        }

        private void SetReady() {

            int playersReady = (int)PhotonNetwork.CurrentRoom.CustomProperties[NetworkCustomProperties.GAME_PLAYERS_LOADED];
            playersReady++;
            PhotonNetwork.CurrentRoom.CustomProperties[NetworkCustomProperties.GAME_PLAYERS_LOADED] = playersReady;
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }

        public new void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {                               
                if (players.Count > 0 && !gameStarted && GameStatesController.IsNotStartedGame())
                {
                    gameStarted = true;
                    Debug.Log("Setup Match");
                    SetupMatch();
                }
            }
        }

        #region SETUPGAME
        protected new void SetupMatch()
        {

            StartCoroutine(RunSetupMatch());
        }

        IEnumerator RunSetupMatch() {
            //Scelta dell'ordine dei giocatori random
            Cmd_ShuffleData();           
            //Seleziono il primo giocatore
            Cmd_SetNextPlayer();
            yield return new WaitForSeconds(1);
            //Distribuisco le carte territorio
            DistributeTerritories();
            yield return new WaitForSeconds(1);
            //Distribuisco le carte obbiettivo
            DistributeTargetCard();
            yield return new WaitForSeconds(1);
            //Distribuisco le armate
            DistributeArmies();
            yield return new WaitForSeconds(1);
            //Inizializza le posizioni 
            StartPositioning();
        }

        protected new void PickUpPlayersOrder()
        {
            players.Shuffle();
        }

        private void ShuffleTerritories()
        {
            territories.Shuffle();
        }


        protected new void DistributeTerritories()
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

        protected new void DistributeTargetCard()
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

        protected new void DistributeArmies()
        {
            int startArmies = 20 + ((6 - players.Count) * 5);
            allStartArmies = startArmies * players.Count;
            foreach (PlayerController player in players)
            {
                player.AddArmies(startArmies);
                player.SetStartArmiesCount(startArmies - player.GetTerritoriesOwned().Count);
            }
        }
        
        public new void StartPositioning()
        {
            GameStatesController.StartGameState();
            currentPlayerController.CalcReinforcmentArmies();
            UpdateTerritoriesInfo();
        }

        #endregion

        public new void SetNextPlayer()
        {
            string[] playersOrder = NetworkCustomProperties.GetRoomProperty(NetworkCustomProperties.PLAYERS_ORDER) as string[];

            if (this.currentPlayerController == null && players.Count > 0)
            {
                Debug.Log("SetNextPlayer");
                this.currentPlayerController = getPlayerByName(playersOrder[0]);
            }
            else if (this.currentPlayerController != null)
            {
                var currentIndex = 0;
                foreach (string playerC in playersOrder)
                {
                    if (currentPlayerController.Player.GetPlayerName().Equals(playerC))
                    {
                        break;
                    }
                    currentIndex++;
                }
                if (currentIndex == playersOrder.Length - 1)
                {
                    this.currentPlayerController = getPlayerByName(playersOrder[0]);
                }
                else
                {
                    this.currentPlayerController = getPlayerByName(playersOrder[currentIndex + 1]);
                }
            }
        }

        protected new void StartAttack()
        {
            PhotonNetwork.LoadLevel(SceneEnum.BATTLEFIELD);
        }

        public new void HandleContinueButton()
        {
            if (!IsMyTurn()) { return; }
            if (currentPlayerController.GetArmiesPerTurn() <= 0)
            {
                UpdateAttackMoveInfo();
                UpdateTerritoriesInfo();
                if (GameStatesController.IsSetupGame() || GameStatesController.IsMove() || GameStatesController.IsNotStartedGame())
                {
                    Cmd_EndTurnPlayer();
                }
                else
                {
                    GameStatesController.NextState();
                }
            }

        }

        public new void HandleAttackMoveBtn()
        {
            if (!IsMyTurn()) { return; }
            if (GameStatesController.IsAttack())
            {
                Cmd_SetDestination(destinationTerritory.GetOwnerController());
                levelLoader.StartBattleField();
            }
            else if (GameStatesController.IsMove())
            {
                if (this.selectedTerritoy != null && this.destinationTerritory != null)
                {
                    Cmd_SetMoving();
                }
            }
            else if (GameStatesController.IsMoving())
            {
                this.SelectedTerritoy.RemoveArmies((int)moveSlider.value);
                this.DestinationTerritory.AddArmies((int)moveSlider.value);
                GameStatesController.NextState();
                moveSlider.value = 0;
                moveSlider.gameObject.SetActive(false);
                UpdateAttackMoveInfo();
                Cmd_EndTurnPlayer();
            }
        }

        public new void HandleSliderMove()
        {
            if (!IsMyTurn()) { return; }
            if (selectedTerritoy.Territory.GetArmies() > 1)
            {
                armiesToMove = (int)moveSlider.value;
                moveSlider.GetComponentInChildren<Text>().text = moveSlider.value.ToString();
            }
        }

        public new void HandleExitButton()
        {
            StartCoroutine(DoExitGame());
        }

        IEnumerator DoExitGame()
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected)
                yield return null;
            levelLoader.LoadMainMenu();
        }

        #region CMD EVENT

        public new void RaiseEvent(object data, byte code, object subject, ExitGames.Client.Photon.Hashtable evData, bool master)
        {
            Debug.Log("Raise Event " + code);
            if (master || IsMyTurn())
            {
                if (evData == null && data != null)
                {
                    evData = new ExitGames.Client.Photon.Hashtable();
                    evData.Add("DATA", data);
                    if (subject != null)
                    {
                        evData.Add("SUBJECT", subject);
                    }
                }
                PhotonNetwork.RaiseEvent(code, evData, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            }
        }

        private void Cmd_ShuffleData()
        {
            Debug.Log("Cmd_ShuffleData");
            PickUpPlayersOrder();
            string[] playersOrder = new string[players.Count];
            for (var i = 0; i < players.Count; i++)
            {
                PlayerController pc = players[i];
                playersOrder[i] = pc.Player.GetPlayerName();
            }
            ShuffleTerritories();
            NetworkCustomProperties.AddRoomProperty(NetworkCustomProperties.PLAYERS_ORDER, playersOrder);
        }

        private void Cmd_SetNextPlayer()
        {
            Debug.Log("Cmd_SetNextPlayer");
            SetNextPlayer();
            RaiseEvent(null, EventNetwork.NEXT_PLAYER, null, null, true);
        }

        private void Cmd_EndTurnPlayer()
        {
            Debug.Log("Cmd_EndPlayerPlayer");
            if (currentPlayerController && currentPlayerController.GetArmiesPerTurn() <= 0)
            {
                EndPlayerTurn();
                RaiseEvent(null, EventNetwork.END_PLAYER_TURN, null, null, true);
            }
        }

        private void Cmd_SetMoving()
        {
            Debug.Log("Cmd_SetMoving");
            SetMoving();
            RaiseEvent(null, EventNetwork.SET_MOVING_STATE, null, null, true);
        }

        private void Cmd_SetDestination(PlayerController playerController)
        {
            Debug.Log("Cmd_SetDestination");
            this.selectedDefenderController = playerController;
            ExitGames.Client.Photon.Hashtable evData = new ExitGames.Client.Photon.Hashtable();
            evData.Add("DESTINATION", playerController);
            RaiseEvent(null, EventNetwork.SET_DESTINATION, null, evData, true);
        }

        private void Cmd_PlayersCreated() {
            Debug.Log("Cmd_PlayersCreated");
            RaiseEvent(null, EventNetwork.PLAYERS_CREATED, null, null, true);
        }

        #endregion


        #region EVENT_RECEIVED
        private void NetworkingClient_EventReceived(EventData obj)
        {
            ExitGames.Client.Photon.Hashtable evData = obj.CustomData != null ? obj.CustomData as ExitGames.Client.Photon.Hashtable : null;
            if(obj.Code < 200)
            Debug.Log("Event received with code " + obj.Code+ " From : " + obj.Sender);
            switch (obj.Code)
            {
                case EventNetwork.PLAYERS_CREATED:
                    CreatePlayers();
                    break;
                case EventNetwork.NEXT_STATE:
                    GameStatesController.OnChangeStateEvent(evData);
                    break;
                case EventNetwork.NEXT_PLAYER:
                    SetNextPlayer();
                    break;
                case EventNetwork.SET_MOVING_STATE:
                    SetMoving();
                    break;
                case EventNetwork.SET_DESTINATION:
                    this.selectedDefenderController = evData[0] as PlayerController;
                    break;
                case EventNetwork.END_PLAYER_TURN:
                    EndPlayerTurn();
                    break;
                case EventNetwork.PLAYER_ADD_ARMY:
                    PlayerController pcont = players.Find(el => el.Player.GetPlayerName().Equals(evData["SUBJECT"] as string));
                    if(pcont)
                        pcont.AddArmies((int)evData["DATA"]);
                    break;
                case EventNetwork.PLAYER_LOST_ARMY:
                    PlayerController pcont1 = players.Find(el => el.Player.GetPlayerName().Equals(evData["SUBJECT"] as string));
                    if (pcont1)
                        pcont1.LostArmies((int)evData["DATA"]);
                    break;
                case EventNetwork.PLAYER_START_ARMIES_COUNT:
                    PlayerController pcont4 = players.Find(el => el.Player.GetPlayerName().Equals(evData["SUBJECT"] as string));
                    if (pcont4)
                        pcont4.SetStartArmiesCount((int)evData["DATA"]);
                    break;
                case EventNetwork.PLAYER_ADD_TERRITORY:
                    PlayerController pcont2 = players.Find(el => el.Player.GetPlayerName().Equals(evData["SUBJECT"] as string));
                    TerritoryController terr = getTerritoryByName(evData["DATA"] as string);
                    if (pcont2 && terr)
                        pcont2.AddTerritory(terr.Territory);
                    break;
                case EventNetwork.PLAYER_LOST_TERRITORY:
                    PlayerController pcont3 = players.Find(el => el.Player.GetPlayerName().Equals(evData["SUBJECT"] as string));
                    TerritoryController terr1 = getTerritoryByName(evData["DATA"] as string);
                    if (pcont3 && terr1)
                        pcont3.LostTerritory(terr1.Territory);
                    break;
                case EventNetwork.TERRITORY_SET_OWNER:
                    TerritoryController subject = getTerritoryByName(evData["SUBJECT"] as string);
                    PlayerController pcontt = players.Find(el => el.Player.GetPlayerName().Equals(evData["DATA"] as string));
                    if (pcontt)
                        subject.SetOwner(pcontt);
                    break;
                case EventNetwork.TERRITORY_ADD_ARMY:
                    TerritoryController subject1 = getTerritoryByName(evData["SUBJECT"] as string);
                    if(subject1)
                        subject1.AddArmies((int)evData["DATA"]);
                    break;
                case EventNetwork.TERRITORY_LOST_ARMY:
                    TerritoryController subject2 = getTerritoryByName(evData["SUBJECT"] as string);
                    if (subject2)
                        subject2.RemoveArmies((int)evData["DATA"]);
                    break;
            }
        }
        #endregion

        #region utility functions

        public new bool IsMyTurn()
        {
            Debug.Log("MyTurn ? " + PhotonNetwork.LocalPlayer.NickName + " / " + currentPlayerController.GetPlayer().GetPlayerName());
            return currentPlayerController != null && currentPlayerController.GetPlayer() != null && localPlayerText.text.Equals(currentPlayerController.GetPlayer().GetPlayerName());
        }

        private TerritoryController getTerritoryByName(string name)
        {
            if (territoriesContainer == null) {
                Debug.LogError("territoriesContainer è null...perchèèèèè cazzo -> " + name);
                return null;
            }
            Debug.Log("Non è null bene -> " + name);
            foreach (TerritoryController tc in territoriesContainer.GetComponentsInChildren<TerritoryController>())
            {
                if (tc.GetTerritory().GetTerritoryName().Equals(name))
                {
                    return tc;
                }
            }
            return null;
        }

        private PlayerController getPlayerByName(string name)
        {
            return players.Find(el => el.Player.GetPlayerName().Equals(name));
        }
        #endregion
       

    }

}

