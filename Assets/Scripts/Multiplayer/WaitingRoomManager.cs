
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
        private List<Dropdown.OptionData> optionDatasAvailable = new List<Dropdown.OptionData>();
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
                optionDatasAvailable.Insert(ic, new Dropdown.OptionData(GameUtils.GetColorName(GameSettings.playerColors[ic])));
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
                Debug.Log(playersTable[i].GetComponentInChildren<Text>().text);
                if (string.Equals(playersTable[i].GetComponentInChildren<Text>().text, NO_PLAYER, System.StringComparison.OrdinalIgnoreCase))
                {
                    playersTable[i].GetComponentInChildren<Text>().text = player.Value.NickName;
                }
                if (player.Value.CustomProperties.ContainsKey(NetworkCustomProperties.PLAYER_COLOR))
                {
                    playersTable[i].GetComponentInChildren<Dropdown>().value = GetValueByColor(GameSettings.playerColors[(int)NetworkCustomProperties.GetPlayerProperty(player.Value, NetworkCustomProperties.PLAYER_COLOR)]);
                }
                if (player.Value.CustomProperties.ContainsKey(NetworkCustomProperties.PLAYER_IS_READY)) {
                    playersTable[i].GetComponentInChildren<Toggle>().isOn = (bool) player.Value.CustomProperties[NetworkCustomProperties.PLAYER_IS_READY];
                }
                playersTable[i].GetComponentInChildren<Dropdown>().interactable = PhotonNetwork.IsMasterClient || PhotonNetwork.NickName.Equals(playersTable[i].GetComponentInChildren<Text>().text);                
                i++;
            }
            int playersReady = (int) NetworkCustomProperties.GetRoomProperty(NetworkCustomProperties.ROOM_PLAYERS_READY);
            if (playersReady == PhotonNetwork.CurrentRoom.Players.Count)
            {
                startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            }
            else
            {
                startButton.gameObject.SetActive(false);
            }
        }

        private void UpdateDropdownValuesAvailable()
        {
            foreach (GameObject table in playersTable)
            {

            }
        }

        public void HandleReadyButton()
        {

            int playersReady = (int)NetworkCustomProperties.GetRoomProperty(NetworkCustomProperties.ROOM_PLAYERS_READY);
            if (readyButton.isOn)
            {
                playersReady++;               
            }
            else if (playersReady > 0)
            {
                playersReady--;
            }
            NetworkCustomProperties.AddPlayerProperty(NetworkCustomProperties.PLAYER_IS_READY, readyButton.isOn);
            NetworkCustomProperties.AddRoomProperty(NetworkCustomProperties.ROOM_PLAYERS_READY, playersReady);
        }

        public void HandleColor(Dropdown change)
        {
            int i = 0;
            foreach (KeyValuePair<int, Photon.Realtime.Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                Color color = GetColorByValue(playersTable[i].GetComponentInChildren<Dropdown>().value);
                i++;
                int colorIndex = 0;
                for (colorIndex = 0; colorIndex < GameSettings.playerColors.Length; colorIndex++)
                {
                    if (GameSettings.playerColors[colorIndex].Equals(color))
                    {
                        break;
                    }
                }
                NetworkCustomProperties.AddPlayerProperty(player.Value, NetworkCustomProperties.PLAYER_COLOR, colorIndex);
            }
        }

        public void StartPlay()
        {
            PhotonNetwork.LogLevel = PunLogLevel.Full;
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

        private Color GetColorByValue(int value)
        {

            //{ Color.black, /*Blue*/new Color32(0, 114, 255, 255), Color.green, Color.yellow, Color.red, new Color(226, 0, 225, 225) }
            Color color;
            switch (value)
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
            return color;
        }

        private int GetValueByColor(Color color)
        {

            //{ Color.black, /*Blue*/new Color32(0, 114, 255, 255), Color.green, Color.yellow, Color.red, new Color(226, 0, 225, 225) }
            int  value = 0;
            if (color.Equals(Color.black)) {
                value = 0;
            }
            if (color.Equals(new Color32(0, 114, 255, 255)))
            {
                value = 1;
            }
            if (color.Equals(Color.green))
            {
                value = 2;
            }
            if (color.Equals(Color.yellow))
            {
                value = 3;
            }
            if (color.Equals(Color.red))
            {
                value = 4;
            }
            if (color.Equals(new Color(226, 0, 225, 225)))
            {
                value = 5;
            }

            return value;
        }

    }

   
}
