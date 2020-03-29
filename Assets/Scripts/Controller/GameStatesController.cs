﻿using UnityEngine;
using UnityEditor;

public class GameStatesController 
{
    private static string currentState = GameStatesEnum.NOTSTARTEDGAME;

    public static void StartGameState() {
        currentState = GameStatesEnum.SETUPGAME;
    }

    public static void EndGame() {
        currentState = GameStatesEnum.ENDGAME;
    }

    public static void NextState() {
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
                currentState = GameStatesEnum.REINFORCE;
                break;

        }
    }

    public static string GetCurrentState() {
        return currentState;
    }

    public static bool IsNotStartedGame()
    {
        return currentState.Equals(GameStatesEnum.NOTSTARTEDGAME);
    }

    public static bool IsSetupGame() {
        return currentState.Equals(GameStatesEnum.SETUPGAME);
    }

    public static bool IsReinforce() {
        return currentState.Equals(GameStatesEnum.REINFORCE);
    }

    public static bool IsAttack() {
        return currentState.Equals(GameStatesEnum.ATTACK);
    }

    public static bool IsMove()
    {
        return currentState.Equals(GameStatesEnum.MOVE);
    }
}