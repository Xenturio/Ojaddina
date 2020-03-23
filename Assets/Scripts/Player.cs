using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private string id;

    private string color;

    private int soldiers;

    private TargetCard targetCard;

    private StateCard[] stateCards;



    public Player(string id, string color, int soldiers)
    {
        this.Id = id;
        this.Color = color;
        this.Soldiers = soldiers;
    }

    public string Id { get => id; set => id = value; }
    public string Color { get => color; set => color = value; }
    public int Soldiers { get => soldiers; set => soldiers = value; }
    public TargetCard TargetCard { get => targetCard; set => targetCard = value; }
    public StateCard[] StateCards { get => stateCards; set => stateCards = value; }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
