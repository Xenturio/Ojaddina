using UnityEngine;
using UnityEditor;
using com.xenturio.entity;
using com.xenturio.enums;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace com.xenturio.basegame
{
    public class GameStatesController
    {
        private static string currentState = GameStatesEnum.NOTSTARTEDGAME;

        public static void StartGameState()
        {
            currentState = GameStatesEnum.SETUPGAME;
            Cmd_UpdateState();
        }

        public static void EndGame()
        {
            currentState = GameStatesEnum.ENDGAME;
            Cmd_UpdateState();
        }

        public static void SetMoving()
        {
            currentState = GameStatesEnum.ISMOVING;
            Cmd_UpdateState();
        }

        public static void NextState()
        {
            switch (currentState)
            {
                case GameStatesEnum.NOTSTARTEDGAME:
                    currentState = GameStatesEnum.SETUPGAME;
                    break;
                case GameStatesEnum.SETUPGAME:
                    currentState = GameStatesEnum.REINFORCE;
                    break;
                case GameStatesEnum.REINFORCE:
                    currentState = GameStatesEnum.ATTACK;
                    break;
                case GameStatesEnum.ATTACK:
                    currentState = GameStatesEnum.MOVE;
                    break;
                case GameStatesEnum.MOVE:
                case GameStatesEnum.ISMOVING:
                    currentState = GameStatesEnum.REINFORCE;
                    break;
            }
            Cmd_UpdateState();
        }

        public static void SetCurrentState(string state) {
            currentState = state;
        }

        public static string GetCurrentState()
        {
            return currentState;
        }

        public static bool IsNotStartedGame()
        {
            return currentState.Equals(GameStatesEnum.NOTSTARTEDGAME);
        }

        public static bool IsSetupGame()
        {
            return currentState.Equals(GameStatesEnum.SETUPGAME);
        }

        public static bool IsReinforce()
        {
            return currentState.Equals(GameStatesEnum.REINFORCE);
        }

        public static bool IsAttack()
        {
            return currentState.Equals(GameStatesEnum.ATTACK);
        }

        public static bool IsMove()
        {
            return currentState.Equals(GameStatesEnum.MOVE);
        }

        public static bool IsMoving()
        {
            return currentState.Equals(GameStatesEnum.ISMOVING);
        }

        public static void OnChangeStateEvent(Hashtable dataEvent) {
            currentState = dataEvent["STATE"] as string;
        }

        private static void Cmd_UpdateState() {
           ExitGames.Client.Photon.Hashtable evData = new ExitGames.Client.Photon.Hashtable();
            evData.Add("STATE", currentState);
            PhotonNetwork.RaiseEvent(EventNetwork.NEXT_STATE, evData, RaiseEventOptions.Default, SendOptions.SendReliable);
        }
      
    }
}