using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Territory : MonoBehaviour
{

    [SerializeField] string territoryName;

    [SerializeField] ContinentEnum continent;

    [SerializeField] List<Territory> neighborTerritories = new List<Territory>();

    [SerializeField] Color highlightedColor = Color.white;

    [SerializeField] Color baseColor;

    private Player owner;

    private TerritoryController controller;

    private int armies = 0;

    //Armate all'inizio del turno di rinforzo
    private int startReinforceArmies = 0;

    public Player Owner { get => owner; set => owner = value; }
    public TerritoryController Controller { get => controller; set => controller = value; }
    public int Armies { get => armies; set => armies = value; }
    public int StartReinforceArmies { get => startReinforceArmies; set => startReinforceArmies = value; }

    private void Awake()
    {
        if (string.IsNullOrEmpty(territoryName))
        {
            territoryName = this.gameObject.name;
        }
        GetComponentInChildren<Text>().text = territoryName;
        controller = GetComponent<TerritoryController>();
        DontDestroyOnLoad(this);
    }
    
    public void AddArmy()
    {
        this.armies++;
    }

    public void RemoveArmy(int count)
    {
        if (this.armies > 0)
        {
            this.armies -= count;
        }
    }
    
    public void SetOwner(Player owner)
    {
        this.owner = owner;
    }

    public Player GetPlayer()
    {
        return this.owner;
    }

    public string GetTerritoryName() {
        return this.territoryName;
    }

    public List<Territory> GetNeighborTerritories() {
        return neighborTerritories;
    }

    public Color GetHighlightColor() {
        return highlightedColor;
    }

    public int GetArmies() {
        return armies;
    }

    public void SetArmies(int armiesCount) {
        this.armies = armiesCount;
    }

    public ContinentEnum GetContinent() {
        return this.continent;
    }

    public TerritoryController GetTerritoryController() {
        return controller;
    }
}
