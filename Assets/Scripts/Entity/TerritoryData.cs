using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.xenturio.enums;
using UnityEngine.Networking;

namespace com.xenturio.entity
{
    [System.Serializable]
    public class TerritoryData : MonoBehaviour
    {
        [SerializeField] string territoryName;

        [SerializeField] ContinentEnum continent;

        [SerializeField] List<TerritoryData> neighborTerritories = new List<TerritoryData>();
      
        private PlayerData owner;

        private int armies = 1;
        //Armate all'inizio del turno di rinforzo
        private int startReinforceArmies = 0;

        void Awake()
        {
            if (string.IsNullOrEmpty(territoryName))
            {
                territoryName = this.gameObject.name;
            }
        }

        public PlayerData GetOwner()
        {
            return owner;
        }

        
        public void SetOwner(PlayerData value)
        {
            owner = value;
        }

        public int GetArmies()
        {
            return armies;
        }

        public void SetArmies(int value)
        {
            armies = value;
        }

        public int GetStartReinforceArmies()
        {
            return startReinforceArmies;
        }

        public void SetStartReinforceArmies(int value)
        {
            startReinforceArmies = value;
        }

        public string GetTerritoryName()
        {
            return territoryName;
        }

        public void SetTerritoryName(string value)
        {
            territoryName = value;
        }

        public ContinentEnum GetContinent()
        {
            return continent;
        }

        public void SetContinent(ContinentEnum value)
        {
            continent = value;
        }

        public List<TerritoryData> GetNeighborTerritories()
        {
            return neighborTerritories;
        }

        
        public void SetNeighborTerritories(List<TerritoryData> value)
        {
            neighborTerritories = value;
        }        
    }
}