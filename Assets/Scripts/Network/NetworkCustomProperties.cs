using UnityEngine;
using System.Collections;
using Photon.Pun;

public class NetworkCustomProperties 
{

    public const string ROOM_PLAYERS_READY = "PLAYERS_READY";
    public const string GAME_PLAYERS_LOADED = "GAME_PLAYERS_LOADED";
    public const string PLAYER_COLOR = "COLOR";
    public const string PLAYERS_ORDER = "PLAYERS_ORDER";

    public static void AddRoomProperty(string key, object prop) {
        PhotonNetwork.CurrentRoom.CustomProperties.Add(key, prop);
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
    }

    public static object GetRoomProperty(string key) {
        return PhotonNetwork.CurrentRoom.CustomProperties[key];
    }

    public static void AddPlayerProperty(string key, object prop)
    {
        PhotonNetwork.LocalPlayer.CustomProperties.Add(key, prop);
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
    }

    public static object GetPlayerProperty(string key)
    {
        return PhotonNetwork.LocalPlayer.CustomProperties[key];
    }
}
