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
using System;

namespace com.xenturio.multiplayer
{
    public class MultiplayerGameManager : GameManager
    {
        
        private bool gameStarted = false;

        #region mono methods
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            levelLoader = FindObjectOfType<LevelLoader>();
            battleFieldCanvas.gameObject.SetActive(false);
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
            if (GameStatesController.IsSetupGame() && loaderCanvas.gameObject.activeSelf)
            {
                loaderCanvas.gameObject.SetActive(false);                
            }
            localPlayerTerritoriesCountText.text = localPlayerTerritoriesCountText.text.Replace("$1", localPlayerController.GetTerritoriesOwned().Count.ToString());
        }

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
            PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
        }

        private void OnDisabled()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
        }
        #endregion

        public new void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (players.Count > 0 && !gameStarted && GameStatesController.IsNotStartedGame())
                {
                    gameStarted = true;
                    SetupMatch();
                }
            }
        }

        #region SETUP GAMES

        protected new void SetupMatch()
        {
            StartCoroutine(RunSetupMatch());
        }

        IEnumerator RunSetupMatch()
        {
            yield return new WaitForSeconds(1);
            //Scelta dell'ordine dei giocatori random
            Cmd_ShuffleData();
            yield return new WaitForSeconds(1);
            //Seleziono il primo giocatore
            Cmd_SetNextPlayer();
            yield return new WaitForSeconds(1);
            //Distribuisco le carte territorio
            DistributeTerritories();
            yield return new WaitForSeconds(1);
            //Distribuisco le carte obbiettivo
            DistributeTargetCard();
            yield return new WaitForSeconds(1);
            //Inizializza le posizioni 
            StartPositioning();
        }

        new void CreatePlayers()
        {

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
                if (players.Count == PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    SetReady();
                }
            }

        }

        private void SetReady()
        {

            int playersReady = (int)PhotonNetwork.CurrentRoom.CustomProperties[NetworkCustomProperties.GAME_PLAYERS_LOADED];
            playersReady++;
            PhotonNetwork.CurrentRoom.CustomProperties[NetworkCustomProperties.GAME_PLAYERS_LOADED] = playersReady;
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
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
                ExitGames.Client.Photon.Hashtable territoriesOwners = new ExitGames.Client.Photon.Hashtable();
                territories.Shuffle();
                foreach (TerritoryController territory in territories)
                {
                    if (playerIndex == players.Count)
                    {
                        playerIndex = 0;
                    }
                    SetTerritoryOwner(territory, players[playerIndex], false);
                    territoriesOwners.Add(territory.Territory.GetTerritoryName(), players[playerIndex].Player.GetPlayerName());
                    playerIndex++;
                }
                Cmd_DistributeTerritories(territoriesOwners);
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
            if (players == null || players.Count == 0) { return; }
            int startArmies = 20 + ((6 - players.Count) * 5);
            foreach (PlayerController player in players)
            {
                player.SetStartArmiesCount(startArmies - player.GetTerritoriesOwned().Count);
            }
        }

        public new void StartPositioning()
        {
            DistributeArmies();
            if (PhotonNetwork.IsMasterClient)
            {
                GameStatesController.StartGameState();
            }
            if (currentPlayerController)
            {
                CalcReinforcmentArmies();
                UpdateTerritoriesInfo();
            }
        }

        #endregion

        public void GetNextPlayer() {

            string nextPlayer = NetworkCustomProperties.GetRoomProperty(NetworkCustomProperties.NEXT_PLAYER) as string;
            this.currentPlayerController = getPlayerByName(nextPlayer);

        }

        public new void SetNextPlayer()
        {
            string[] playersOrder = NetworkCustomProperties.GetRoomProperty(NetworkCustomProperties.PLAYERS_ORDER) as string[];
            string nextPlayer = null;
            if (this.currentPlayerController == null && players.Count > 0)
            {
                this.currentPlayerController = getPlayerByName(playersOrder[0]);
                nextPlayer = playersOrder[0];
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
                    nextPlayer = playersOrder[0];
                }
                else
                {
                    this.currentPlayerController = getPlayerByName(playersOrder[currentIndex + 1]);
                    nextPlayer = playersOrder[currentIndex + 1];
                }
               
            }
            NetworkCustomProperties.AddRoomProperty(NetworkCustomProperties.NEXT_PLAYER, nextPlayer);
        }

        public new void EndPlayerTurn()
        {
            if (currentPlayerController && currentPlayerController.GetArmiesPerTurn() <= 0)
            {
                if (GameStatesController.IsMove())
                {
                    GameStatesController.NextState();
                }
                SetNextPlayer();
                selectedTerritoy = null;
                DestinationTerritory = null;
                SelectedDefenderController = null;
                if (!GameStatesController.IsNotStartedGame() && endTurnSound)
                {
                    endTurnSound.Play();
                }
                if (CheckToStartGame()) {
                    GameStatesController.NextState();
                }
            }
        }

        private void StartTurn()
        {
            GetNextPlayer();
            if (this.currentPlayerController)
            {
                CalcReinforcmentArmies();
                selectedTerritoy = null;
                DestinationTerritory = null;
                SelectedDefenderController = null;
                if (!GameStatesController.IsNotStartedGame() && endTurnSound)
                {
                    endTurnSound.Play();
                }
            }
        }

        public new void SetTerritoryOwner(TerritoryController tc, PlayerController pc, bool sendEvent)
        {
            tc.SetOwner(pc);
            pc.AddTerritory(tc.Territory);
            if (sendEvent)
            {
                RaiseEvent(pc.GetPlayer().GetPlayerName(), EventNetwork.TERRITORY_SET_OWNER, tc.Territory.GetTerritoryName(), null, false);
            }
        }

        public new void SetSelectedTerritory(TerritoryController territory)
        {
            this.selectedTerritoy = territory;
            UpdateAttackMoveInfo();
        }

        public new void SetDesinationTerritory(TerritoryController territory)
        {
            this.destinationTerritory = territory;
            this.selectedDefenderController = territory != null ? territory.GetOwnerController() : null;
            UpdateAttackMoveInfo();
        }

        protected new void StartAttack()
        {
            battleFieldCanvas.gameObject.SetActive(true);
            //PhotonNetwork.LoadLevel(SceneEnum.BATTLEFIELD);
        }

        #region HandleUI
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
                StartAttack();
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

        protected new void UpdateAttackMoveInfo()
        {

            if (GameStatesController.IsMoving())
            {
                if (!moveSlider || !attackMoveBtn) { return; }
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
                if (!selectedTerritoryText || !targetTerritoryText || !arrowFromTo || !attackMoveBtn) { return; }
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
        #region CMD EVENT

        public new void RaiseEvent(object data, byte code, object subject, ExitGames.Client.Photon.Hashtable evData, bool master)
        {
            if (!PhotonNetwork.IsConnected) { return; }
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

        private void Cmd_DistributeTerritories(ExitGames.Client.Photon.Hashtable territoriesOwners)
        {
            RaiseEvent(null, EventNetwork.DISTRIBUTE_TERRITORIES, null, territoriesOwners, true);
        }

        private void Cmd_SetDestination(PlayerController playerController)
        {
            Debug.Log("Cmd_SetDestination");
            this.selectedDefenderController = playerController;
            RaiseEvent(playerController.GetPlayer().GetPlayerName(), EventNetwork.SET_DESTINATION, this.currentPlayerController.Player.GetPlayerName(), null, false);
        }

        private void Cmd_PlayersCreated()
        {
            Debug.Log("Cmd_PlayersCreated");
            RaiseEvent(null, EventNetwork.PLAYERS_CREATED, null, null, true);
        }

        #endregion


        #region EVENT_RECEIVED
        private void NetworkingClient_EventReceived(EventData obj)
        {
            ExitGames.Client.Photon.Hashtable evData = obj.CustomData != null ? obj.CustomData as ExitGames.Client.Photon.Hashtable : null;
            if (obj.Code < 200)
            {
                Debug.Log("Event received with code " + obj.Code + " From : " + obj.Sender);
            }
            switch (obj.Code)
            {
                case EventNetwork.NEXT_STATE:
                    GameStatesController.OnChangeStateEvent(evData);
                    if ((evData["STATE"] as string).Equals(GameStatesEnum.SETUPGAME)) {
                        StartPositioning();
                    }
                    break;
                case EventNetwork.NEXT_PLAYER:
                    SetNextPlayer();
                    break;
                case EventNetwork.END_PLAYER_TURN:
                    StartTurn();
                    break;
                case EventNetwork.SET_MOVING_STATE:
                    SetMoving();
                    break;
                case EventNetwork.SET_DESTINATION:
                    this.selectedDefenderController = players.Find(el => el.Player.GetPlayerName().Equals(evData["DATA"] as string));
                    break;
                case EventNetwork.SET_SELECTED_TERRITORY:
                    TerritoryController selectedTerr = getTerritoryByName(evData["DATA"] as string);
                    SetSelectedTerritory(selectedTerr);
                    break;
                case EventNetwork.SET_DESTINATION_TERRITORY:
                    TerritoryController destinTerr = getTerritoryByName(evData["DATA"] as string);
                    SetDesinationTerritory(destinTerr);
                    break;
                case EventNetwork.DISTRIBUTE_TERRITORIES:
                    foreach (DictionaryEntry values in evData)
                    {
                        PlayerController owner = players.Find(el => el.Player.GetPlayerName().Equals(values.Value as string));
                        TerritoryController territoryOwned = getTerritoryByName(values.Key as string);
                        if (owner && territoryOwned)
                        {
                            SetTerritoryOwner(territoryOwned, owner, false);
                        }
                    }
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
                case EventNetwork.PLAYER_ADD_ARMY:
                    PlayerController pcont = players.Find(el => el.Player.GetPlayerName().Equals(evData["SUBJECT"] as string));
                    if (pcont)
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
                case EventNetwork.PLAYERS_CREATED:
                    CreatePlayers();
                    break;
                case EventNetwork.TERRITORY_SET_OWNER:
                    TerritoryController subject = getTerritoryByName(evData["SUBJECT"] as string);
                    PlayerController pcontt = players.Find(el => el.Player.GetPlayerName().Equals(evData["DATA"] as string));
                    if (pcontt)
                        subject.SetOwner(pcontt);
                    break;
                case EventNetwork.TERRITORY_ADD_ARMY:
                    TerritoryController subject1 = getTerritoryByName(evData["SUBJECT"] as string);
                    if (subject1)
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
            return currentPlayerController != null && currentPlayerController.GetPlayer() != null && localPlayerText.text.Equals(currentPlayerController.GetPlayer().GetPlayerName());
        }

        private TerritoryController getTerritoryByName(string name)
        {
            if (territoriesContainer == null) { return null; }
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

