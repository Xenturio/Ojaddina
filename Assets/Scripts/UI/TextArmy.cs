using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.xenturio.UI
{
    public class TextArmy : MonoBehaviour
    {
        public void UpdateArmyNumber(int count)
        {
            GetComponent<Text>().text = "" + count;
        }

    }
}
