using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Territory : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
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

    private TerritoryTextName textName;

    private TextArmy textArmy;

    private void Awake()
    {
        if (string.IsNullOrEmpty(name))
        {
            name = this.gameObject.name;
        }
        gameManager = FindObjectOfType<GameManager>();
        GetComponentInChildren<Text>().text = name;
        armies = new List<Army>();
        textName = GetComponentInChildren<TerritoryTextName>();
        textArmy = GetComponentInChildren<TextArmy>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateArmyDisplay();        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private bool buttonClickSemaphore = false;
    private void OnMouseOver()
    {
        if (Input.GetMouseButton(1) && !buttonClickSemaphore)
        {
            RemoveArmy();
            buttonClickSemaphore = true;
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("Mouse click");
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

    public void RemoveArmy() {
        if (this.armies != null && this.armies.Count > 0) {
            this.armies.RemoveAt(0);
            UpdateArmyDisplay();
        }
    }

    /*
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Territory neighborTerritory = collider.GetComponent<Territory>();
        if (neighborTerritory != null && !neighborTerritories.Contains(neighborTerritory))
        {
            Debug.Log(name + " confina con " + neighborTerritory.name);
            neighborTerritories.Add(neighborTerritory);
        }
    }
    */


    public void SetOwner(Player owner) {
        this.owner = owner;
        textName.SetColorPlayerText();
        GetComponentInChildren<SpriteRenderer>().color = owner.GetPlayerColor();
    }

    private void HighlightTerritory() {
        if (IsSelectableTerritory())
        {
            gameManager.SetSelectedTerritory(this);
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
        textArmy.UpdateArmyNumber(armies.Count);
    }

    public Player GetPlayer() {
        return this.owner;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            AddArmy();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            RemoveArmy();
        }
            
    }
}
