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
        }

        public void LostArmies(int armies)
        {
            player.LostArmies(armies);
        }

        public int GetStartArmiesCount()
        {
            return player.GetStartArmiesCount();
        }

        public void SetStartArmiesCount(int armiesStart)
        {
            player.SetStartArmiesCount(armiesStart);
            if (multiGameManager && multiGameManager.CurrentPlayerController.Equals(this)) {
                multiGameManager.RaiseEvent(armiesStart, EventNetwork.PLAYER_START_ARMIES_COUNT, Player.GetPlayerName(), null, false);
            }
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
    }
}
