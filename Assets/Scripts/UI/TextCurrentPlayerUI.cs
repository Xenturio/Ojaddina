using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCurrentPlayerUI : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTextPlayer();
    }

    private void UpdateTextPlayer()
    {
        GetComponent<Text>().text = gameManager.GetCurrentPlayer().name + " : " + gameManager.GetCurrentPlayer().GetArmiesPerTurn();
        GetComponent<Text>().color = gameManager.GetCurrentPlayer().GetPlayerColor();
    }
}
