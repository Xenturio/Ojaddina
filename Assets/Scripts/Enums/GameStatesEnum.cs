using UnityEngine;
using UnityEditor;

namespace com.xenturio.enums
{
    public class GameStatesEnum
    {
        public const string NOTSTARTEDGAME = "Preparing Game";
        public const string SETUPGAME = "Setup game";
        public const string REINFORCE = "Reinforce";
        public const string ATTACK = "Attack";
        public const string MOVE = "Move";
        public const string ENDGAME = "End Game";
        public const string ISMOVING = "Moving";
    }
}