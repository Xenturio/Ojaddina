using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.xenturio.entity;
using com.xenturio.enums;

public class PlayerPrefsController : MonoBehaviour
{
    const string MASTER_VOLUME_KEY = "master.volume";
    const string MASTER_DIFFICULTY_KEY = "difficulty";
    const string SAVED_GAME_LEVEL = "saved.game.level";
    const string PLAYER_COLOR = "player.color";
    const string PLAYER_NICKNAME = "player.nickname";
    const string NUMBER_PLAYERS = "game.number.player";

    const float MAX_VOLUME = 10;
    const float MIN_VOLUME = 0;
    const int MAX_DIFFICULTY = 5;
    const int MIN_DIFFICULTY = 1;

    public static void SetMasterVolume(float volume)
    {
        if (volume >= MIN_VOLUME && volume <= MAX_VOLUME)
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, volume);
            MusicPlayer player = FindObjectOfType<MusicPlayer>();
            if (player)
            {
                player.SetVolume(volume);
            }
        }
    }

    public static float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1);
    }

    public static void SetDifficulty(int level)
    {
        if (level >= MIN_DIFFICULTY && level <= MAX_DIFFICULTY)
        {
            PlayerPrefs.SetInt(MASTER_DIFFICULTY_KEY, level);
        }
    }

    public static int GetDifficulty()
    {
        return PlayerPrefs.GetInt(MASTER_DIFFICULTY_KEY);
    }

    public static string GetSavedGameLevel()
    {
        return PlayerPrefs.GetString(SAVED_GAME_LEVEL);
    }

    public static void SetSavedGameLevel(string level)
    {
        PlayerPrefs.SetString(SAVED_GAME_LEVEL, level);
    }

    public static string GetPlayerNickname() {
        return PlayerPrefs.GetString(PLAYER_NICKNAME);
    }

    public static void SetPlayerNickname(string nickName)
    {
        PlayerPrefs.SetString(PLAYER_NICKNAME, nickName);
    }

    public static Color GetPlayerColor() {
        return GameUtils.GetColorByName( PlayerPrefs.GetString(PLAYER_COLOR));
    }

    public static void SavePlayerData(string nickname, Color color) {
        PlayerPrefs.SetString(PLAYER_NICKNAME,nickname);
        PlayerPrefs.SetString(PLAYER_COLOR, GameUtils.GetColorName(color));
    }

    public static int GetNumberPlayers() {
        return PlayerPrefs.GetInt(NUMBER_PLAYERS);
    }

    public static void SaveNumberPlayers(int nPlayers) {
        PlayerPrefs.SetInt(NUMBER_PLAYERS, nPlayers);
    }
}
