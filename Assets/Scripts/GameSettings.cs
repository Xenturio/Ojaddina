using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings 
{
    public const int MAX_NUM_PLAYER = 6;

    public const int MIN_DICE_TO_ATTACK = 2;

    public const int DEFAULT_TURN_TIMER_MINUTES = 5;

    public static bool DOMINATION_MODE = false;

    public static bool RANDOMIZE_COLOR_PLAYER = true;

    public static bool AUTO_START_POSITIONING_ARMY = true;

    public static Color[] playerColors = { Color.black, Color.blue, Color.green, Color.yellow, Color.red, Color.cyan };

    public const int ASIA_ARMY = 7;

    public const int AFRIA_ARMY = 3;

    public const int AMERICA_ARMY = 5;

    public const int EUROPE_ARMY = 5;

    public const int SUDAMERICA_ARMY = 2;

    public const int OCEANIA_ARMY = 2;

}
