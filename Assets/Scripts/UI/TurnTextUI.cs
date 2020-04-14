using com.xenturio.basegame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.xenturio.UI
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
