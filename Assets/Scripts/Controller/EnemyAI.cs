using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.xenturio.basegame
{
    public class EnemyAI : MonoBehaviour
    {
        private GameManager gameManager;

        private PlayerController playerController;
        // Start is called before the first frame update
        void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            playerController = GetComponent<PlayerController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (gameManager.IsMyTurn(playerController)) {
                if (GameStatesController.IsReinforce()) {
                    for (var i = 0; i < playerController.GetArmiesPerTurn(); i++) {
                        var randomTerritory = playerController.GetTerritoriesOwned()[Random.Range(0, playerController.GetTerritoriesOwned().Count)];
                        randomTerritory.SetArmies(randomTerritory.GetArmies() + 1);
                        playerController.RemoveArmyPerTurn();
                    }                   
                    gameManager.HandleContinueButton();
                }
                if (GameStatesController.IsAttack())
                {
                    gameManager.HandleContinueButton();
                }
                if (GameStatesController.IsMove()) {
                    gameManager.HandleContinueButton();
                }                
            }
        }
    }
}
