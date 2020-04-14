using UnityEngine;

public class EventNetwork : MonoBehaviour
{

    public const byte UPDATE_TERRITORY = 1;
    public const byte UPDATE_PLAYER = 2;
    public const byte NEXT_STATE = 3;
    public const byte NEXT_PLAYER = 4;
    public const byte END_PLAYER_TURN = 5;
    public const byte SET_MOVING_STATE = 6;
    public const byte SET_DESTINATION = 7;
    public const byte SET_SELECTED_TERRITORY = 8;
    public const byte SHUFFLE_DATA = 9;
    //Player 10-19
    public const byte PLAYER_ADD_TERRITORY = 10;
    public const byte PLAYER_LOST_TERRITORY = 11;
    public const byte PLAYER_ADD_ARMY = 12;
    public const byte PLAYER_LOST_ARMY = 13;
    public const byte PLAYER_START_ARMIES_COUNT = 14;
    public const byte PLAYERS_CREATED = 15;

    //Territory 20-29
    public const byte TERRITORY_ADD_ARMY = 20;
    public const byte TERRITORY_LOST_ARMY = 21;
    public const byte TERRITORY_SET_OWNER = 22;

}
