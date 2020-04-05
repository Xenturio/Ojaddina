using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.xenturio.singleplayer
{
    public class TurnStateButton : MonoBehaviour
    {
        GameManager gameManger;

        public void EndTurn()
        {
            gameManger.EndPlayerTurn();
            NextTurnState();
        }

        public void NextTurnState()
        {
            GameStatesController.NextState();
        }
        // Start is called before the first frame update
        void Start()
        {
            gameManger = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
