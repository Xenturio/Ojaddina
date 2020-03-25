using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private string name;

    private Color color;

    private int armies = 0;

    private TargetCard targetCard;

    private StateCard[] stateCards = new StateCard[] { };

    private List<Territory> territoriesOwned = new List<Territory>();

    private int armiesPerTurn = 1;

    public void AddTerritory(Territory newTerritory) {
        territoriesOwned.Add(newTerritory);
    }

    public void LostTerritory(Territory lostTerritory) {
        territoriesOwned.Remove(lostTerritory);
    }

    public void SetTargetCard(TargetCard targetCard) {
        this.targetCard = targetCard;
    }

    public int GetArmiesPerTurn() {
        return this.armiesPerTurn;
    }

    public void AddArmies(int armies) {
        this.armies += armies;
    }

    public void PickUpColor(Color color) {
        this.color = color;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CalcReinforcmentArmies() {
        //Numero di stati diviso 3
        var armies = this.territoriesOwned.Count / 3;


    }

    public Color GetPlayerColor() {
        return this.color;
       }
}
