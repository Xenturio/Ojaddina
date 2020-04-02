using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private string playerName;

    private Color color;

    private int armies = 0;

    private TargetCard targetCard;

    private TerritoryCard[] territoryCards = new TerritoryCard[] { };

    private List<Territory> territoriesOwned = new List<Territory>();

    private int armiesPerTurn = 0;
    //Numero di armate iniziali
    private int startArmiesCount = 20;

    private PlayerController controller;

    public string PlayerName { get => playerName; set => playerName = value; }
    public Color Color { get => color; set => color = value; }
    public int Armies { get => armies; set => armies = value; }
    public TargetCard TargetCard { get => targetCard; set => targetCard = value; }
    public TerritoryCard[] StateCards { get => territoryCards; set => territoryCards = value; }
    public List<Territory> TerritoriesOwned { get => territoriesOwned; set => territoriesOwned = value; }
    public int ArmiesPerTurn { get => armiesPerTurn; set => armiesPerTurn = value; }
    public int StartArmiesCount { get => startArmiesCount; set => startArmiesCount = value; }
    public PlayerController Controller { get => controller; set => controller = value; }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(playerName)) {
            playerName = name;
        }
    }

    public void AddTerritory(Territory newTerritory) {
        territoriesOwned.Add(newTerritory);
    }

    public void LostTerritory(Territory lostTerritory) {
        territoriesOwned.Remove(lostTerritory);
    }

    public void SetTargetCard(TargetCard targetCard) {
        this.targetCard = targetCard;
    }

    public int GetArmies() {
        return this.armies;
    }

    public int GetArmiesPerTurn() {
        return this.armiesPerTurn;
    }

    public void RemoveArmyPerTurn() {
        this.armiesPerTurn--;
    }

    public void AddArmyPerTurn(int armies)
    {
        this.armiesPerTurn += armies;
    }

    public void AddArmies(int armies) {
        this.armies += armies;
    }

    public void LostArmies(int armies) {
        if (this.armies >= armies)
        {
            this.armies -= armies;
        }
        else {
            this.armies = 0;
        }

    }

    public int GetStartArmiesCount() {
        return this.startArmiesCount;
    }

    public void SetStartArmiesCount(int armiesStart) {
        this.startArmiesCount = armiesStart;
    }

    public void PickUpColor(Color color) {
        this.color = color;
    }

    public Color GetPlayerColor() {
        return this.color;
     }

    public List<Territory> GetTerritoriesOwned() {
        return territoriesOwned;
    }
}
