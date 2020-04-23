using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class NetworkCustomProperties 
{

    public const string ROOM_PLAYERS_READY = "ROOM_PLAYERS_READY";
    public const string GAME_PLAYERS_LOADED = "GAME_PLAYERS_LOADED";
    public const string NEXT_PLAYER = "NEXT_PLAYER";
    public const string PLAYER_COLOR = "PLAYER_COLOR";
    public const string PLAYERS_ORDER = "PLAYERS_ORDER";
    public const string PLAYER_IS_READY = "PLAYER_IS_READY";

    public static void AddRoomProperty(object key, object prop) {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key))
        {
            PhotonNetwork.CurrentRoom.CustomProperties[key] = prop;
        }
        else
        {
            PhotonNetwork.CurrentRoom.CustomProperties.Add(key, prop);
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
    }

    public static object GetRoomProperty(object key) {
        return PhotonNetwork.CurrentRoom.CustomProperties[key];
    }

    public static void AddPlayerProperty(object key, object prop)
    {
        AddPlayerProperty(PhotonNetwork.LocalPlayer, key, prop);
    }

    public static object GetPlayerProperty(object key)
    {
        return GetPlayerProperty(PhotonNetwork.LocalPlayer, key);
    }

    public static void AddPlayerProperty(Player player, object key, object prop)
    {
        if (player.CustomProperties.ContainsKey(key))
        {
            player.CustomProperties[key] = prop;
        }
        else
        {
            player.CustomProperties.Add(key, prop);
        }
        player.SetCustomProperties(player.CustomProperties);
    }

    public static object GetPlayerProperty(Player player, object key)
    {
        return player.CustomProperties[key];
    }
}
