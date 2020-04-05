using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private int playerTTL = 30 * 1000; //30sec
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("WaitingRoom", new RoomOptions { MaxPlayers = 6, PlayerTtl = playerTTL }, TypedLobby.Default);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
