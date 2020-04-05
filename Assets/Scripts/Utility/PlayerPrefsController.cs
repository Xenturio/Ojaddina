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
        return PlayerPrefs.GetFloat(MASTER_VOLUME_KEY);
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

    public static void SavePlayer(Player player) {
        var playerJson = JsonUtility.ToJson(player);
        Debug.Log(playerJson);
        PlayerPrefs.SetString(player.name, playerJson);
    }

    public static Player LoadPlayer(Player player) {
        var playerJson = PlayerPrefs.GetString(player.name);
        Debug.Log(playerJson);
        return JsonUtility.FromJson<Player>(playerJson);
    }

    public static void SaveTerritory(Territory territory)
    {
        PlayerPrefs.SetString(territory.name, JsonUtility.ToJson(territory));
    }

    public static Territory LoadTerritory(Territory territory)
    {
        var territoryJson = PlayerPrefs.GetString(territory.name);
        return JsonUtility.FromJson<Territory>(territoryJson);
    }
}
