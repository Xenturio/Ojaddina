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
    public const byte SET_DESTINATION_TERRITORY = 9;
    public const byte SHUFFLE_DATA = 10;
    public const byte DISTRIBUTE_TERRITORIES = 11;
    //Player 20-39
    public const byte PLAYER_ADD_TERRITORY = 20;
    public const byte PLAYER_LOST_TERRITORY = 21;
    public const byte PLAYER_ADD_ARMY = 22;
    public const byte PLAYER_LOST_ARMY = 23;
    public const byte PLAYER_START_ARMIES_COUNT = 24;
    public const byte PLAYERS_CREATED = 25;

    //Territory 40-59
    public const byte TERRITORY_ADD_ARMY = 40;
    public const byte TERRITORY_LOST_ARMY = 41;
    public const byte TERRITORY_SET_OWNER = 42;

}
