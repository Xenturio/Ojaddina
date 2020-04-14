using com.xenturio.basegame;
using com.xenturio.multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.xenturio.UI
{
    public class TextCurrentPlayerUI : MonoBehaviour
    {
        GameManager gameManager;
        MultiplayerGameManager multiplayerGameManager;
        // Start is called before the first frame update
        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            multiplayerGameManager = FindObjectOfType<MultiplayerGameManager>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateTextPlayer();
        }

        public void UpdateTextPlayer()
        {
            if (multiplayerGameManager && multiplayerGameManager.CurrentPlayerController)
            {
                GetComponent<Text>().text = multiplayerGameManager.CurrentPlayerController.Player.GetPlayerName() + " : " + multiplayerGameManager.CurrentPlayerController.GetArmiesPerTurn();
                GetComponent<Text>().color = multiplayerGameManager.CurrentPlayerController.GetPlayerColor();
            }
            else if (gameManager && gameManager.CurrentPlayerController)
            {
                GetComponent<Text>().text = gameManager.CurrentPlayerController.Player.GetPlayerName() + " : " + gameManager.CurrentPlayerController.GetArmiesPerTurn();
                GetComponent<Text>().color = gameManager.CurrentPlayerController.GetPlayerColor();
            }
        }
    }
}