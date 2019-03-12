using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestMonoBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        GameState initialGS = new GameState();
        Resource gold = new RoundedResource(name: "gold", perWorkerTick: 5, resourceCount: 100, workerCount: 0, percision: 2);
        Resource stone = new RoundedResource("stone", 5, 100, 0, 2);
        Resource wood = new RoundedResource("wood", 5, 100, 0, 2);
        initialGS.addResource(gold);
        initialGS.addResource(stone);
        initialGS.addResource(wood);


        GameState targetGS = new GameState();
        Resource targetGold = new RoundedResource("gold", 5, 5000000, 5, 2);
        Resource targetStone = new RoundedResource("stone", 5, 5000000, 5, 2);
        Resource targetWood = new RoundedResource("wood", 5, 5000000, 5, 2);
        targetGS.addResource(targetGold);
        //targetGS.addResource(targetStone);
        //targetGS.addResource(targetWood);

        int buildBank = 0;
        int waitCount = 0;
        int stoneMiner = 0;
        int woodsman = 0;
        foreach(Work w in LongTermPlanning.plan(initialGS, targetGS))
        {
            if (w == Work.EMPTY) { }
            else if (w == Work.NewGoldMiner)  { buildBank++; }
            else if (w == Work.Wait)          { waitCount++; }
            else if (w == Work.NewStoneMiner) { stoneMiner++; }
            else if (w == Work.NewWoodsman)   { woodsman++; }
        }
        Debug.Log("Wait count:  " + waitCount + "  bank build:  " + buildBank + "  stoneMiner:  " + stoneMiner + "  woodsman:  " + woodsman);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
