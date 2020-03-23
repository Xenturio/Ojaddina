using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateCard  : Card
{
    private Territory state;

    private string bonus;

    public StateCard(string id, string description, Territory state, string bonus)
    {
        State = state;
        Bonus = bonus;
        Description = description;
        Id = id;
    }

    public Territory State { get => state; set => state = value; }
    public string Bonus { get => bonus; set => bonus = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
