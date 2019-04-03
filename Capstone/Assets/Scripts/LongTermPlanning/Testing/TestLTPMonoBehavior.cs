﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestLTPMonoBehavior : MonoBehaviour
{

    public MemoryBoundedLongTermPlanner ltp;

    public void testLTP() {

        BuildingGS initialGS = new BuildingGS();
        initialGS.setStockpile(ResourceType.Gold, 1000);
        initialGS.addResourcePerTick(ResourceType.Gold, 0);
        initialGS.setStockpile(ResourceType.Stone, 1000);
        initialGS.addResourcePerTick(ResourceType.Stone, 0);
        initialGS.setStockpile(ResourceType.Wood, 1000);
        initialGS.addResourcePerTick(ResourceType.Wood, 0);


        BuildingGS targetGS = new BuildingGS();
        targetGS.setStockpile(ResourceType.Gold, 2000);
        targetGS.addResourcePerTick(ResourceType.Gold, 10);
        targetGS.setStockpile(ResourceType.Stone, 2000);
        targetGS.addResourcePerTick(ResourceType.Stone, 5);
        targetGS.setStockpile(ResourceType.Wood, 2000);
        targetGS.addResourcePerTick(ResourceType.Wood, 6);
        targetGS.setStockpile(ResourceType.Silver, 200);
        targetGS.addResourcePerTick(ResourceType.Silver, 9);

        int buildBank = 0;
        int waitCount = 0;
        int stoneMiner = 0;
        int woodsman = 0;

        //ltpID.iterativeDepth(initialGS, targetGS, true);

        ltp.plan(initialGS, targetGS, callback);
        
    }

    public bool callback(Stack<Work> result) {
        Debug.Log("Path found");
        foreach(Work w in result) {
            Debug.Log(w);
        }
        return true;
    }
}