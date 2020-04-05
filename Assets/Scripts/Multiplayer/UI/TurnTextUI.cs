using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.xenturio.multiplayer
{
    public class TurnTextUI : MonoBehaviour
    {
        Text text;

        void Start()
        {
            text = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            text.text = GameStatesController.GetCurrentState().ToString();
        }
    }
}
