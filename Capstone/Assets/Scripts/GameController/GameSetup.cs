﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSetup {
    public static readonly int BOARD_WIDTH  = 25;
    public static readonly int BOARD_HEIGHT = 25;


    public static readonly int BUILDING_LAYER = 15;
    public static readonly int CHARACTER_LAYER = 10;

    public static readonly float TICK_LENGHT_SEC = 2.0f; // 2 secconds = one in game tick

    public static readonly int WORKER_GOLD_COST = 30;


    public static readonly string GAME_CONTROLLER_TAG = "GameController";
    public static readonly string CONSTRUCTION_CONTROLLER_TAG = "ConstructionController";


}
