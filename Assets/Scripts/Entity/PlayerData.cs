using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.xenturio.entity
{
    [System.Serializable]
    public class PlayerData : MonoBehaviour
    {

        [SerializeField]
        private string playerName;

        [SerializeField]
        private Color color;

        [SerializeField]
        private int armies = 0;

        private TargetCard targetCard;

        private List<TerritoryCard> territoryCards = new List<TerritoryCard>();

        private List<TerritoryData> territoriesOwned = new List<TerritoryData>();

        [SerializeField]
        private int armiesPerTurn = 0;
        //Numero di armate iniziali
        [SerializeField]
        private int startArmiesCount = 20;
        
        private void Start()
        {
            if (string.IsNullOrEmpty(playerName))
            {
                playerName = name;
            }
        }

        public string GetPlayerName()
        {
            return playerName;
        }

        public void SetPlayerName(string value)
        {
            playerName = value;
        }

        public Color GetColor()
        {
            return color;
        }

        public void SetColor(Color value)
        {
            color = value;
        }

        public int GetArmies()
        {
            return armies;
        }

        public void SetArmies(int value)
        {
            armies = value;
        }

        public TargetCard GetTargetCard()
        {
            return targetCard;
        }

        
        public void SetTargetCard(TargetCard value)
        {
            targetCard = value;
        }

        public TerritoryCard[] GetStateCards()
        {
            return territoryCards.ToArray();
        }

        
        public void SetStateCards(TerritoryCard[] value)
        {
            territoryCards = new List<TerritoryCard>(value);
        }

        public List<TerritoryData> GetTerritoriesOwned()
        {
            return territoriesOwned;
        }

        
        public void SetTerritoriesOwned(List<TerritoryData> value)
        {
            territoriesOwned = value;
        }

        public int GetArmiesPerTurn()
        {
            return armiesPerTurn;
        }

        public void SetArmiesPerTurn(int value)
        {
            armiesPerTurn = value;
        }

        public int GetStartArmiesCount()
        {
            return startArmiesCount;
        }

        public void SetStartArmiesCount(int value)
        {
            startArmiesCount = value;
        }

        
        public void AddTerritory(TerritoryData newTerritory)
        {
            territoriesOwned.Add(newTerritory);
        }

        
        public void LostTerritory(TerritoryData lostTerritory)
        {
           territoriesOwned.Remove(lostTerritory);
        }
        
        public void RemoveArmyPerTurn()
        {
            this.armiesPerTurn--;
        }

        public void AddArmyPerTurn(int armies)
        {
            this.armiesPerTurn += armies;
        }

        public void AddArmies(int armies)
        {
            this.armies += armies;
        }

        public void LostArmies(int armies)
        {
            if (this.armies >= armies)
            {
                this.armies -= armies;
            }
            else
            {
                this.armies = 0;
            }

        }
        
        public void PickUpColor(Color color)
        {
            this.color = color;
        }

        public Color GetPlayerColor()
        {
            return this.color;
        }

    }
}
