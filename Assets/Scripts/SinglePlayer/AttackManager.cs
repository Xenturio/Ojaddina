using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.xenturio.basegame
{
    public class AttackManager : MonoBehaviour
    {
        [SerializeField] Slider attackerSlider;
        [SerializeField] Slider attackerHealth;
        [SerializeField] Slider defenderHealth;
        [SerializeField] TankController[] attackerTanks;
        [SerializeField] TankController[] defenderTanks;
        [SerializeField] Text resultText;
        [SerializeField] Text[] attackDiceResults;
        [SerializeField] Text[] defendDiceResults;


        GameManager gameManager;
        multiplayer.MultiplayerGameManager multiplayerGameManager;
        PlayerController attacker;
        PlayerController defender;
        TerritoryController attackerTerritory;
        TerritoryController defenderTerritory;
        LevelLoader levelLoader;

        int maxAttackerTanks = 3;
        int maxDefenderTanks = 3;

        int currentAttackerTanks = 1;
        int currentDefenderTanks = 1;


        // Start is called before the first frame update
        void Start()
        {
            levelLoader = FindObjectOfType<LevelLoader>();
            multiplayerGameManager = FindObjectOfType<multiplayer.MultiplayerGameManager>();
            if (!multiplayerGameManager)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
            StartAttack();
            SetAttackersCount(false);
            SetDefenderCount();
            attackerSlider.gameObject.GetComponentInChildren<Text>().text = attacker.Player.GetPlayerName();
            attackerSlider.gameObject.GetComponentInChildren<Text>().color = attacker.Player.GetPlayerColor();
        }

        public void Attack()
        {
            if (currentAttackerTanks <= 0 || currentDefenderTanks <= 0)
            {
                return;
            }

            List<int> attackerPoint = new List<int>(currentAttackerTanks);
            List<int> defenderPoint = new List<int>(currentDefenderTanks);

            for (var i = 0; i < currentAttackerTanks; i++)
            {
                attackerPoint.Add(Random.Range(1, 7));
            }
            for (var i = 0; i < currentDefenderTanks; i++)
            {
                defenderPoint.Add(Random.Range(1, 7));
            }
            attackerPoint.Sort((a, b) => b.CompareTo(a));
            defenderPoint.Sort((a, b) => b.CompareTo(a));

            for (var i = 0; i < attackerPoint.Count; i++)
            {
                attackDiceResults[i].text = attackerPoint[i].ToString();
            }
            for (var i = 0; i < defenderPoint.Count; i++)
            {
                defendDiceResults[i].text = defenderPoint[i].ToString();
            }

            int length = attackerPoint.Count >= defenderPoint.Count ? defenderPoint.Count : attackerPoint.Count;

            var attackerArmiesLost = 0;
            var defenderArmiesLost = 0;
            for (var i = 0; i < length; i++)
            {
                if (attackerPoint[i] > defenderPoint[i])
                {
                    defenderTanks[i].Death();
                    currentDefenderTanks--;
                    defenderArmiesLost++;
                }
                else
                {
                    attackerTanks[i].Death();
                    currentAttackerTanks--;
                    attackerArmiesLost++;
                }
            }
            UpdateArmies(attackerArmiesLost, defenderArmiesLost);
            HandleBattleResult();
        }

        public void Retire()
        {
            resultText.color = Color.red;
            resultText.text = "Shame on you!";
            GetGameManager().SetDesinationTerritory(null);
            StartCoroutine(EndBattle());
        }

        private void HandleBattleResult()
        {
            if (currentAttackerTanks <= 0 && maxAttackerTanks <= 1)
            {
                resultText.color = defender != null ? defender.GetPlayerColor() : Color.blue;
                resultText.text = (defender != null ? defender.Player.GetPlayerName() : "Defender") + " WIN";
                GetGameManager().SetDesinationTerritory(null);
                StartCoroutine(EndBattle());
            }
            else if (currentDefenderTanks <= 0 && maxDefenderTanks <= 0)
            {
                resultText.color = attacker != null ? attacker.GetPlayerColor() : Color.red;
                resultText.text = (attacker != null ? attacker.Player.GetPlayerName() : "Attacker") + " WIN";
                GetGameManager().SetTerritoryOwner(defenderTerritory, attacker, true);
                defenderTerritory.AddArmies(currentAttackerTanks);
                GetGameManager().SetSelectedTerritory(defenderTerritory);
                GetGameManager().SetDesinationTerritory(null);
                StartCoroutine(EndBattle());
            }
            else
            {
                attackerSlider.value = currentAttackerTanks;
            }

        }

        public void StartAttack()
        {
            this.attacker = GetGameManager().CurrentPlayerController;
            this.defender = GetGameManager().DestinationTerritory.OwnerController;
            this.attackerTerritory = GetGameManager().SelectedTerritoy;
            this.defenderTerritory = GetGameManager().DestinationTerritory;

            maxAttackerTanks = attackerTerritory.GetTerritory().GetArmies() - 1;
            maxDefenderTanks = defenderTerritory.GetTerritory().GetArmies();

            attackerHealth.maxValue = maxAttackerTanks;
            defenderHealth.maxValue = maxDefenderTanks;
            attackerHealth.value = maxAttackerTanks;
            defenderHealth.value = maxDefenderTanks;
        }

        private void ClearBattlefield()
        {

            attacker = null;
            defender = null;
            attackerTerritory = null;
            defenderTerritory = null;
            maxAttackerTanks = 3;
            maxDefenderTanks = 3;
            currentAttackerTanks = 1;
            currentDefenderTanks = 1;
            resultText.text = "";
            foreach (Text text in attackDiceResults) {
                text.text = "";
            }
            foreach (Text text in defendDiceResults)
            {
                text.text = "";
            }
        }

        IEnumerator EndBattle()
        {           
            yield return new WaitForSeconds(3f);
            ClearBattlefield();
            this.gameObject.SetActive(false);
            //levelLoader.ReturnToMainGame();
        }

      

        public void SetAttackersCount(bool sliding)
        {
            if (attackerSlider.value > maxAttackerTanks)
            {
                attackerSlider.value = maxAttackerTanks;
            }
            if (sliding)
            {
                currentAttackerTanks = (int)attackerSlider.value;
                if (currentAttackerTanks == 1)
                {
                    if (attackerTanks[1] != null)
                    {
                        attackerTanks[1].gameObject.SetActive(false);
                    }
                    if (attackerTanks[2] != null)
                    {
                        attackerTanks[2].gameObject.SetActive(false);
                    }
                }
                else if (currentAttackerTanks == 2)
                {
                    if (attackerTanks[1] != null)
                    {
                        attackerTanks[1].gameObject.SetActive(true);
                    }
                    if (attackerTanks[2] != null)
                    {
                        attackerTanks[2].gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (attackerTanks[1] != null)
                    {
                        attackerTanks[1].gameObject.SetActive(true);
                    }
                    if (attackerTanks[2] != null)
                    {
                        attackerTanks[2].gameObject.SetActive(true);
                    }
                }
            }
        }

        public void SetDefenderCount()
        {
            currentDefenderTanks = maxDefenderTanks > 3 ? 3 : maxDefenderTanks;
          
                if (currentDefenderTanks == 1)
                {
                    defenderTanks[1].gameObject.SetActive(false);
                    defenderTanks[2].gameObject.SetActive(false);
                }
                else if (currentDefenderTanks == 2)
                {
                    defenderTanks[1].gameObject.SetActive(true);
                    defenderTanks[2].gameObject.SetActive(false);
                }
                else
                {
                    defenderTanks[1].gameObject.SetActive(true);
                    defenderTanks[2].gameObject.SetActive(true);
                }
            
        }

        private void UpdateArmies(int attackerArmiesLost, int defenderArmiesLost)
        {

            attackerTerritory.LoseArmies(attackerArmiesLost);
            defenderTerritory.LoseArmies(defenderArmiesLost);

            if (attackerTerritory.GetTerritory().GetArmies() <= 3)
            {
                maxAttackerTanks = attackerTerritory.GetTerritory().GetArmies() - 1;
            }
            if (defenderTerritory.GetTerritory().GetArmies() < 3)
            {
                maxDefenderTanks = defenderTerritory.GetTerritory().GetArmies();
            }
            attackerHealth.value = maxAttackerTanks;
            defenderHealth.value = maxDefenderTanks;
        }

        private GameManager GetGameManager()
        {
            if (multiplayerGameManager != null)
            {
                return multiplayerGameManager;
            }
            return gameManager;
        }
    }
}