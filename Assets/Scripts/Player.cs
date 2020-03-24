using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private string name;

    private string color;

    private int soldiers;

    private TargetCard targetCard;

    private StateCard[] stateCards;

    private List<Territory> territoriesOwned;

    public void AddTerritory(Territory newTerritory) {
        territoriesOwned.Add(newTerritory);
    }

    public void LostTerritory(Territory lostTerritory) {
        territoriesOwned.Remove(lostTerritory);
    }

    public void SetTargetCard(TargetCard targetCard) {
        this.targetCard = targetCard;
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



    }
}
