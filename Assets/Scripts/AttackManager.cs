using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackManager : MonoBehaviour
{
    [SerializeField] Slider attackerSlider;
    [SerializeField] Slider defenderSlider;
    [SerializeField] TankController[] attackerTanks;
    [SerializeField] TankController[] defenderTanks;
    [SerializeField] Text resultText;
    [SerializeField] Text[] attackDiceResults;
    [SerializeField] Text[] defendDiceResults;

   
    GameManager gameManager;
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
        gameManager = FindObjectOfType<GameManager>();        
    }

    public void Attack() {
        if (currentAttackerTanks <= 0 || currentDefenderTanks <= 0) {
            return;
        }

        List<int> attackerPoint = new List<int>(currentAttackerTanks);
        List<int> defenderPoint = new List<int>(currentDefenderTanks);

        for (var i = 0; i < currentAttackerTanks; i++)
        {
            var dice = Random.Range(1, 7);
            attackerPoint.Add(Random.Range(1, 7));
        }
        for (var i = 0; i < currentDefenderTanks; i++)
        {
            var dice = Random.Range(1, 7);
            defenderPoint.Add(dice);
        }
        attackerPoint.Sort((a, b) => b.CompareTo(a));
        defenderPoint.Sort((a, b) => b.CompareTo(a));

        for (var i = 0; i < attackerPoint.Count; i++) {
            attackDiceResults[i].text = attackerPoint[i].ToString();
        }
        for (var i = 0; i < defenderPoint.Count; i++)
        {
            defendDiceResults[i].text = defenderPoint[i].ToString();
        }

        int length = attackerPoint.Count >= defenderPoint.Count ? defenderPoint.Count : attackerPoint.Count;

        for (var i = 0; i < length; i++) {
            if (attackerPoint[i] > defenderPoint[i])
            {
                defenderTanks[i].Death();
                currentDefenderTanks--;               
            }
            else {
                attackerTanks[i].Death();
                currentAttackerTanks--;                
            }
        }        
        HandleBattleResult();
    }

    private void HandleBattleResult() {
        Debug.Log("Attackers :" + currentAttackerTanks);
        Debug.Log("Defenders :" + currentDefenderTanks);
        if (currentAttackerTanks <= 0)
        {
            resultText.color = defender != null ? defender.GetPlayerColor() : Color.blue;
            resultText.text = (defender != null ? defender.name : "Defender") + " WIN";
            StartCoroutine(EndBattle());
        }
        else if (currentDefenderTanks <= 0)
        {
            resultText.color = attacker != null ? attacker.GetPlayerColor() : Color.red;
            resultText.text = (attacker != null ? attacker.name : "Attacker") + " WIN";
            StartCoroutine(EndBattle());
        }
        else {
            defenderSlider.value = currentDefenderTanks;
            attackerSlider.value = currentAttackerTanks;
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAttack(PlayerController attacker, PlayerController defender, TerritoryController attackerTerritory, TerritoryController defenderTerritory) {
        this.attacker = attacker;
        this.defender = defender;
        this.attackerTerritory = attackerTerritory;
        this.defenderTerritory = defenderTerritory;
        this.gameObject.SetActive(true);

        if (attackerTerritory.GetTerritory().GetArmies() <= 3) {
            maxAttackerTanks = attackerTerritory.GetTerritory().GetArmies() - 1;
        }
        if (defenderTerritory.GetTerritory().GetArmies() < 3)
        {
            maxDefenderTanks = defenderTerritory.GetTerritory().GetArmies();
        }

    }

    IEnumerator EndBattle() {
       
        yield return new WaitForSeconds(2f);
        levelLoader.ReturnToMainGame();
    }

    public void SetAttackersCount(bool sliding) {
        if (attackerSlider.value > maxAttackerTanks) {
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

    public void SetDefenderCount(bool sliding) {
        if (defenderSlider.value > maxDefenderTanks)
        {
            defenderSlider.value = maxDefenderTanks;
        }       
        if (sliding)
        {
            currentDefenderTanks = (int)defenderSlider.value;
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
    }



}
