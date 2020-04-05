using System.Collections;
using System.Collections.Generic;
using com.xenturio.entity;
using com.xenturio.enums;
using UnityEngine;

namespace com.xenturio.singleplayer
{
    public class PlayerController : MonoBehaviour
    {

        Player player;

        public Player Player { get => player; set => player = value; }

        private void Awake()
        {
            player = GetComponent<Player>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        public Player GetPlayer()
        {
            return player;
        }

        public void AddTerritory(Territory newTerritory)
        {
            player.AddTerritory(newTerritory);
        }

        public void LostTerritory(Territory lostTerritory)
        {
            player.LostTerritory(lostTerritory);
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

        public List<Territory> GetTerritoriesOwned()
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
