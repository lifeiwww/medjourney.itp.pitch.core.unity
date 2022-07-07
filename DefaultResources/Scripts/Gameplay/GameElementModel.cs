using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameElementType
{
    // Targets/Collectibles
    GAME_ELEMENT_WALL_TARGET,
    GAME_ELEMENT_FLOOR_TARGET,
    GAME_ELEMENT_FLOOR_TIMER_TARGET,
    GAME_ELEMENT_HOLD_TARGET,
    GAME_ELEMENT_PATH_COLLECTIBLE,

    // Zone Elements
    GAME_ELEMENT_DANGER_ZONE,
    GAME_ELEMENT_BONUS_ZONE

}

[Serializable]
public class GameElementModel
{
    public int ScoreValue { get; set; }
    public GameElementType ElementType { get; set; }
}


