using System.Collections.Generic;
using com.xenturio.entity;
using com.xenturio.enums;
using com.xenturio.multiplayer;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace com.xenturio.basegame
{
    public class PlayerController : MonoBehaviour
    {

        protected PlayerData player;

        public PlayerData Player { get => player; set => player = value; }

        private GameManager gameManager;

        private MultiplayerGameManager multiGameManager;

        private void Awake()
        {
            player = GetComponent<PlayerData>();
            multiGameManager = FindObjectOfType<MultiplayerGameManager>();
            gameManager = FindObjectOfType<GameManager>();
        }

        public PlayerData GetPlayer()
        {
            return player;
        }

        public void AddTerritory(TerritoryData newTerritory)
        {
            player.AddTerritory(newTerritory);
            if(multiGameManager)
                multiGameManager.RaiseEvent(newTerritory.GetTerritoryName(), EventNetwork.PLAYER_ADD_TERRITORY, Player.GetPlayerName(), null, false);
        }

        public void LostTerritory(TerritoryData lostTerritory)
        {
            player.LostTerritory(lostTerritory);
            if (multiGameManager)
                multiGameManager.RaiseEvent(lostTerritory.GetTerritoryName(), EventNetwork.PLAYER_LOST_TERRITORY, Player.GetPlayerName(), null, false);
        }

        public void SetTargetCard(TargetCard targetCard)
        {
            player.SetTargetCard(targetCard);
        }

        public int GetArmiesPerTurn()
        {
            return player.GetArmiesPerTurn();
        }

        public void RemoveArmyPerTurn()
        {
            player.RemoveArmyPerTurn();
        }

        public void AddArmyPerTurn(int armies)
        {
            player.AddArmyPerTurn(armies);
        }

        public void AddArmies(int armies)
        {
            player.AddArmies(armies);
            if (multiGameManager)
                multiGameManager.RaiseEvent(armies, EventNetwork.PLAYER_ADD_ARMY, Player.GetPlayerName(), null, false);
        }

        public void LostArmies(int armies)
        {
            player.LostArmies(armies);
            if (multiGameManager)
                multiGameManager.RaiseEvent(armies, EventNetwork.PLAYER_LOST_ARMY, Player.GetPlayerName(), null, false);
        }

        public int GetStartArmiesCount()
        {
            return player.GetStartArmiesCount();
        }

        public void SetStartArmiesCount(int armiesStart)
        {
            player.SetStartArmiesCount(armiesStart);
            if (multiGameManager)
                multiGameManager.RaiseEvent(armiesStart, EventNetwork.PLAYER_START_ARMIES_COUNT, Player.GetPlayerName(), null, false);
        }

        public void PickUpColor(Color color)
        {
            if (color != null)
            {
                player.PickUpColor(color);
            }
        }

        public Color GetPlayerColor()
        {
            return player.GetPlayerColor();
        }

        public List<TerritoryData> GetTerritoriesOwned()
        {
            return player.GetTerritoriesOwned();
        }

        public void CalcReinforcmentArmies()
        {
            var armiesPerTurn = CalculateReinforcmentArmies();
            player.AddArmyPerTurn(armiesPerTurn);
            //Numero di stati diviso 3
            if (GameStatesController.IsSetupGame() && player.GetStartArmiesCount() > 0)
            {
                player.SetStartArmiesCount(player.GetStartArmiesCount() - 3);
            }
        }

        public int CalculateReinforcmentArmies()
        {
            //Numero di stati diviso 3
            if (GameStatesController.IsSetupGame() && player.GetStartArmiesCount() > 0)
            {
                var armiesToAdd = player.GetStartArmiesCount() > 3 ? 3 : player.GetStartArmiesCount();
                return armiesToAdd;
            }
            else if (GameStatesController.IsReinforce())
            {
                var armies = Mathf.FloorToInt(player.GetTerritoriesOwned().Count / 3);
                //Controllo se ho tutti i territori di un continente
                var hasAsia = player.GetTerritoriesOwned().FindAll(x => x.GetContinent() == ContinentEnum.ASIA).Count == GameSettings.ASIA_ARMY;
                var hasEurope = player.GetTerritoriesOwned().FindAll(x => x.GetContinent() == ContinentEnum.EUROPE).Count == GameSettings.EUROPE_ARMY;
                var hasAmerica = player.GetTerritoriesOwned().FindAll(x => x.GetContinent() == ContinentEnum.NORD_AMERICA).Count == GameSettings.AMERICA_ARMY;
                var hasSudAmerica = player.GetTerritoriesOwned().FindAll(x => x.GetContinent() == ContinentEnum.SUD_AMERICA).Count == GameSettings.SUDAMERICA_ARMY;
                var hasAfrica = player.GetTerritoriesOwned().FindAll(x => x.GetContinent() == ContinentEnum.AFRICA).Count == GameSettings.AFRIA_ARMY;
                var hasOceania = player.GetTerritoriesOwned().FindAll(x => x.GetContinent() == ContinentEnum.OCEANIA).Count == GameSettings.OCEANIA_ARMY;

                if (hasAfrica) { armies += GameSettings.AFRIA_ARMY; }
                if (hasAsia) { armies += GameSettings.ASIA_ARMY; }
                if (hasAmerica) { armies += GameSettings.AMERICA_ARMY; }
                if (hasSudAmerica) { armies += GameSettings.SUDAMERICA_ARMY; }
                if (hasOceania) { armies += GameSettings.OCEANIA_ARMY; }
                if (hasEurope) { armies += GameSettings.EUROPE_ARMY; }

                return armies;
            }
            return 0;
        }

    }
}
