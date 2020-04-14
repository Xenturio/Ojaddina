
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
            if (!PhotonNetwork.IsConnected) {
                FindObjectOfType<LevelLoader>().LoadMainMenu();
            }
            List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
            for (var ic = 0; ic < GameSettings.playerColors.Length; ic++)
            {
                optionDatas.Insert(ic, new Dropdown.OptionData(GameUtils.GetColorName(GameSettings.playerColors[ic])));
            }
            startButton.gameObject.SetActive(false);
            int i = 0;
            foreach (GameObject table in playersTable)
            {
                Dropdown dropdown = table.GetComponentInChildren<Dropdown>();
                dropdown.name = "Dropdown-"+ i;
                dropdown.AddOptions(optionDatas);
                dropdown.interactable = PhotonNetwork.IsMasterClient;
                dropdown.value = i;
                dropdown.onValueChanged.AddListener(delegate
                {
                    HandleColor(null);
                });
                table.GetComponentInChildren<Text>().text = NO_PLAYER;
                i++;
            }
            HandleColor(null);
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
                if (player.Value.CustomProperties.ContainsKey(NetworkCustomProperties.PLAYER_COLOR))
                {
                    playersTable[i].GetComponentInChildren<Text>().color = GameSettings.playerColors[(int)player.Value.CustomProperties[NetworkCustomProperties.PLAYER_COLOR]];
                }
                playersTable[i].GetComponentInChildren<Dropdown>().name = player.Value.NickName;
                i++;
            }
            int playersReady = (int)PhotonNetwork.CurrentRoom.CustomProperties[NetworkCustomProperties.ROOM_PLAYERS_READY];
            if (playersReady == PhotonNetwork.CurrentRoom.Players.Count)
            {
                startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            }
            else
            {
                startButton.gameObject.SetActive(false);
            }
        }


        public void HandleReadyButton()
        {

            int playersReady = (int)PhotonNetwork.CurrentRoom.CustomProperties[NetworkCustomProperties.ROOM_PLAYERS_READY];
            if (readyButton.isOn)
            {
                playersReady++;
            }
            else if (playersReady > 0)
            {
                playersReady--;
            }
            PhotonNetwork.CurrentRoom.CustomProperties[NetworkCustomProperties.ROOM_PLAYERS_READY] = playersReady;
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }

        public void HandleColor(Dropdown change)
        {
            int i = 0;
            foreach (KeyValuePair<int, Photon.Realtime.Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                Color color = Color.black;
                //{ Color.black, /*Blue*/new Color32(0, 114, 255, 255), Color.green, Color.yellow, Color.red, new Color(226, 0, 225, 225) }
                switch (playersTable[i].GetComponentInChildren<Dropdown>().value)
                {
                    case 0:
                        color = Color.black;
                        break;
                    case 1:
                        color = new Color32(0, 114, 255, 255);
                        break;
                    case 2:
                        color = Color.green;
                        break;
                    case 3:
                        color = Color.yellow;
                        break;
                    case 4:
                        color = Color.red;
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
                if (player.Value.CustomProperties.ContainsKey(NetworkCustomProperties.PLAYER_COLOR))
                {
                    player.Value.CustomProperties[NetworkCustomProperties.PLAYER_COLOR] = colorIndex;
                }
                else
                {
                    player.Value.CustomProperties.Add(NetworkCustomProperties.PLAYER_COLOR, colorIndex);
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
            PhotonNetwork.Disconnect();
            FindObjectOfType<LevelLoader>().LoadMainMenu();
        }
    }
}
