using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.xenturio.multiplayer
{
    public class TextCurrentPlayerUI : MonoBehaviour
    {
        MultiplayerGameManager gameManager;
        // Start is called before the first frame update
        void Start()
        {
            gameManager = FindObjectOfType<MultiplayerGameManager>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateTextPlayer();
        }

        private void UpdateTextPlayer()
        {
            if (gameManager.GetCurrentPlayer())
            {
                GetComponent<Text>().text = gameManager.GetCurrentPlayer().PlayerName + " : " + gameManager.GetCurrentPlayer().GetArmiesPerTurn();
                GetComponent<Text>().color = gameManager.GetCurrentPlayer().GetPlayerColor();
            }
        }
    }
}