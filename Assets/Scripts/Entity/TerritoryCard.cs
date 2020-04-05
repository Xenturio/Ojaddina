using com.xenturio.enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.xenturio.entity
{
    public class TerritoryCard : MonoBehaviour
    {
        private Territory territory;

        private BonusCardTypeEnum bonusType;

        public TerritoryCard(Territory territory)
        {
            this.territory = territory;
        }

    }
}
