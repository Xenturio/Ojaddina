using com.xenturio.enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.xenturio.entity
{
    public class TerritoryCard : MonoBehaviour
    {
        private TerritoryData territory;

        private BonusCardTypeEnum bonusType;

        public TerritoryCard(TerritoryData territory)
        {
            this.territory = territory;
        }

    }
}
