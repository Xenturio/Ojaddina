using com.xenturio.basegame;
using com.xenturio.entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.xenturio.UI
{
    public class TerritoryTextName : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Text>().text = GetComponentInParent<TerritoryController>().name;
        }

        public void SetColorPlayerText()
        {
            PlayerController player = GetComponentInParent<TerritoryController>().GetOwnerController();
            GetComponent<Text>().color = player.GetPlayerColor();
        }

    }
}
