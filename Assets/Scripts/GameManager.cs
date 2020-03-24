﻿using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<Player> players;

    private Player currentPlayer;

    private Territory selectedTerritoy;

    private TurnStateEnum currentState;

    // Start is called before the first frame update
    void Start()
    {
        players = new List<Player>(FindObjectsOfType<Player>());
        DistributeTerritories();
        DistributeTargetCard();
        currentState = TurnStateEnum.REINFORCE;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DistributeTerritories() {

        Territory[] allTerritories = FindObjectsOfType<Territory>();
        List<Territory> randomTerritories = new List<Territory>(allTerritories.Length);
        foreach (Territory territory in allTerritories) {
            randomTerritories.Add(territory);
        }
        randomTerritories.Shuffle();
        var playerIndex = 0;
        foreach(Territory territory in randomTerritories)
        {
            if (playerIndex == players.Count) {
                playerIndex = 0;
            }
            players[playerIndex].AddTerritory(territory);
            playerIndex++;
        }
    }

    private void DistributeTargetCard() {
        if (!GameSettings.DOMINATION_MODE) {
            TargetCard[] allTargetsCard = FindObjectsOfType<TargetCard>();
            List<TargetCard> randomCards = new List<TargetCard>(allTargetsCard.Length);
            foreach (TargetCard card in allTargetsCard)
            {
                randomCards.Add(card);
            }
            randomCards.Shuffle();
            var cardIndex = 0;
            foreach (Player player in players)
                {
                    player.SetTargetCard(randomCards[cardIndex]);
                cardIndex++;
                }
            
        }
    }

    public void AddPlayer(Player player) {
        players.Add(player);
    }

    public void RemovePlayer(Player player) {
        players.Remove(player);
    }

    public void EndPlayerTurn() {
        var currentIndex = players.IndexOf(currentPlayer);
        if (currentIndex == players.Count - 1)
        {
            currentPlayer = players[0];
        }
        else {
            currentPlayer = players[currentIndex + 1];
        }
        selectedTerritoy = null;
    }

    public Player GetCurrentPlayer() {
        return this.currentPlayer;
    }

    public Territory GetSelectedTerritory() {
        return this.selectedTerritoy;
    }

    public void SetSelectedTerritory(Territory territory) {
        this.selectedTerritoy = territory;
    }

    public TurnStateEnum GetCurrentTurnState() {
        return currentState;
    }
   
}

static class MyExtensions {

    public static void Shuffle<T>(this IList<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (System.Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}