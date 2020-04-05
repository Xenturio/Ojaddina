using com.xenturio.entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.xenturio.singleplayer
{
    public class TerritoryTextName : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Text>().text = GetComponentInParent<Territory>().name;
        }

        public void SetColorPlayerText()
        {
            Player player = GetComponentInParent<Territory>().GetPlayer();
            GetComponent<Text>().color = player.GetPlayerColor();
        }

    }
}
