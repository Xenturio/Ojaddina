using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.xenturio.singleplayer
{
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

        public void UpdateTextPlayer()
        {
            if (gameManager.CurrentPlayerController)
            {
                GetComponent<Text>().text = gameManager.CurrentPlayerController.Player.PlayerName + " : " + gameManager.CurrentPlayerController.GetArmiesPerTurn();
                GetComponent<Text>().color = gameManager.CurrentPlayerController.GetPlayerColor();
            }
        }
    }
}