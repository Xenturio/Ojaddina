using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using com.xenturio.enums;

namespace com.xenturio.multiplayer
{
    public class WaitingRoomManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] GameObject[] playersTable;
        [SerializeField] Toggle readyButton;
        [SerializeField] Button startButton;

        private const string NO_PLAYER = "NO PLAYER";
        // Start is called before the first frame update
        void Start()
        {
            List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
            for (var ic = 0; ic < GameSettings.playerColors.Length; ic++)
            {
                optionDatas.Insert(ic, new Dropdown.OptionData(GameSettings.playerColorsName[ic].ToString()));
            }
            startButton.gameObject.SetActive(false);
            int i = 0;
            foreach (GameObject table in playersTable)
            {
                Dropdown dropdown = table.GetComponentInChildren<Dropdown>();
                dropdown.AddOptions(optionDatas);
                dropdown.interactable = PhotonNetwork.IsMasterClient;
                dropdown.value = i;
                dropdown.onValueChanged.AddListener(delegate
                {
                    HandleColor();
                });
                //dropdown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
                table.GetComponentInChildren<Text>().text = NO_PLAYER;
                i++;
            }
            HandleColor();
        }

        // Update is called once per frame
        void Update()
        {
            var i = 0;
            if (PhotonNetwork.CurrentRoom == null) { return; }
            foreach (KeyValuePair<int, Photon.Realtime.Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                if (playersTable[i].GetComponentInChildren<Text>().text.Equals(NO_PLAYER))
                {
                    playersTable[i].GetComponentInChildren<Text>().text = string.IsNullOrEmpty(player.Value.NickName) ? player.Value.UserId : player.Value.NickName;
                }
                if (player.Value.CustomProperties.ContainsKey("Color"))
                {
                    playersTable[i].GetComponentInChildren<Text>().color = GameSettings.playerColors[(int)player.Value.CustomProperties["Color"]];
                }
                i++;
            }
        }


        public void HandleReadyButton()
        {

            int playersReady = (int)PhotonNetwork.CurrentRoom.CustomProperties["PlayersReady"];
            if (readyButton.isOn)
            {
                playersReady++;
            }
            else if (playersReady > 0)
            {
                playersReady--;
            }
            PhotonNetwork.CurrentRoom.CustomProperties["PlayersReady"] = playersReady;
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
            if (playersReady == PhotonNetwork.CurrentRoom.Players.Count)
            {
                startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            }
            else
            {
                startButton.gameObject.SetActive(false);
            }
        }

        public void HandleColor()
        {
            int i = 0;
            foreach (KeyValuePair<int, Photon.Realtime.Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                Color color = Color.black;
                switch (playersTable[i].GetComponentInChildren<Dropdown>().value)
                {
                    case 0:
                        color = Color.black;
                        break;
                    case 1:
                        color = Color.yellow;
                        break;
                    case 2:
                        color = Color.red;
                        break;
                    case 3:
                        color = Color.green;
                        break;
                    case 4:
                        color = Color.blue;
                        break;
                    case 5:
                        color = new Color(226, 0, 225, 225);
                        break;
                    default:
                        color = Color.black;
                        break;

                }
                i++;
                int colorIndex = 0;
                for (colorIndex = 0; colorIndex < GameSettings.playerColors.Length; colorIndex++)
                {
                    if (GameSettings.playerColors[colorIndex].Equals(color))
                    {
                        break;
                    }
                }
                if (player.Value.CustomProperties.ContainsKey("Color"))
                {
                    player.Value.CustomProperties["Color"] = colorIndex;
                }
                else
                {
                    player.Value.CustomProperties.Add("Color", colorIndex);
                }
                player.Value.SetCustomProperties(player.Value.CustomProperties);
            }
        }

        public void StartPlay()
        {
            PhotonNetwork.LoadLevel(SceneEnum.MULTIPLAYER_MAIN_GAME);
        }

        /// <summary>
        /// Called after disconnecting from the Photon server.
        /// </summary>
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");


        }

        public void ExitPlayer()
        {

            var i = 0;
            foreach (KeyValuePair<int, Photon.Realtime.Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                if (playersTable[i].GetComponentInChildren<Text>().text.Equals(PhotonNetwork.LocalPlayer.NickName))
                {
                    playersTable[i].GetComponentInChildren<Text>().text = NO_PLAYER;
                }
                i++;
            }
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            FindObjectOfType<LevelLoader>().LoadMainMenu();
        }
    }
}