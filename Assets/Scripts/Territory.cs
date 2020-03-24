using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Territory : MonoBehaviour
{

    [SerializeField] string name;

    [SerializeField] Continent continent;

    [SerializeField] List<Territory> neighborTerritories;

    [SerializeField] Color highlightedColor = Color.white;

    [SerializeField] Color baseColor;

    private Player owner;

    private List<Army> armies;

    private Territory attackingTerritory;

    private GameManager gameManager;

    private void Awake()
    {
        if (string.IsNullOrEmpty(name))
        {
            name = "Territorio" + this.gameObject.name;
        }
        baseColor = GetComponent<SpriteRenderer>().color;
        gameManager = FindObjectOfType<GameManager>();
        GetComponentInChildren<Text>().text = name;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseDown()
    {
        HighlightTerritory();
        if (gameManager.GetCurrentTurnState().Equals(TurnStateEnum.REINFORCE))
        {
            AddArmy();
        }
        else if (gameManager.GetCurrentTurnState().Equals(TurnStateEnum.ATTACK) && IsAttackableTerritory()) {

        }
        
    }

    public void AddArmy() {
        this.armies.Add(new Army());
        UpdateArmyDisplay();
    }

    public void removeArmy() {
        if (this.armies != null && this.armies.Count > 0) {
            this.armies.RemoveAt(0);
            UpdateArmyDisplay();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Territory neighborTerritory = collider.GetComponent<Territory>();
        if (neighborTerritory != null && !neighborTerritories.Contains(neighborTerritory))
        {
            Debug.Log(name + " confina con " + neighborTerritory.name);
            neighborTerritories.Add(neighborTerritory);
        }
    }


    public void SetOwner(Player owner) {
        this.owner = owner;
    }

    private void HighlightTerritory() {
        if (IsSelectableTerritory())
        {
            Color color = GetComponent<SpriteRenderer>().color;
            if (color.Equals(highlightedColor))
            {
                GetComponent<SpriteRenderer>().color = baseColor;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = highlightedColor;
            }
            ResetHighlightTerritory();
            gameManager.SetSelectedTerritory(this);
        }
    }

    private void ResetHighlightTerritory() {
        foreach (Territory territory in FindObjectsOfType<Territory>()) {
            if (attackingTerritory.name != territory.name)
            {
                territory.GetComponent<SpriteRenderer>().color = territory.baseColor;
            }
        }
    }

    private bool IsSelectableTerritory() {

        //Un territorio è selezionabile quando è un mio territorio
        if (this.owner == null || gameManager.GetCurrentPlayer() == null) {
            return false;
        }
        if (this.owner.name == gameManager.GetCurrentPlayer().name)
        {
            return true;
        }
        return false;
    }

    private bool IsAttackableTerritory() {

        if (this.owner.name == gameManager.GetCurrentPlayer().name){
            return false;
        }
        if (this.armies == null || this.armies.Count < GameSettings.MIN_DICE_TO_ATTACK) {
            return false;
        }

        if (gameManager.GetSelectedTerritory() != null) {
            foreach (Territory neighbour in gameManager.GetSelectedTerritory().neighborTerritories)
            {
                if (neighbour.name.Equals(this.name))
                {
                    attackingTerritory = neighbour;
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateArmyDisplay() {
        Slider armyDisplay = GetComponentInChildren<Slider>();
        if (armyDisplay != null) {
            armyDisplay.value = armies.Count;
        }
    }
}
