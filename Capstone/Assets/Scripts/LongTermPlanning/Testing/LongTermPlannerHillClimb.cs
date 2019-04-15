using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class LongTermPlannerHillClimb : MonoBehaviour {

    public ALongTermPlanner ltp;
    

    public void hillClimb() {

        BuildingGS initialGS = new BuildingGS();
        initialGS.setStockpile(ResourceType.Gold, 1000);
        initialGS.addResourcePerTick(ResourceType.Gold, 0);
        initialGS.setStockpile(ResourceType.Stone, 1000);
        initialGS.addResourcePerTick(ResourceType.Stone, 0);
        initialGS.setStockpile(ResourceType.Wood, 1000);
        initialGS.addResourcePerTick(ResourceType.Wood, 0);


        BuildingGS targetGS = new BuildingGS();
        targetGS.setStockpile(ResourceType.Gold, 1000);
        targetGS.addResourcePerTick(ResourceType.Gold, 5);
        targetGS.setStockpile(ResourceType.Stone, 1000);
        targetGS.addResourcePerTick(ResourceType.Stone, 5);
        targetGS.setStockpile(ResourceType.Wood, 1000);
        targetGS.addResourcePerTick(ResourceType.Wood, 5);
        //targetGS.setStockpile(ResourceType.Silver, 500);
        targetGS.addResourcePerTick(ResourceType.Silver, 5);
        //targetGS.setStockpile(ResourceType.Coal, 100);
        targetGS.addResourcePerTick(ResourceType.Coal, 5);
        //targetGS.setStockpile(ResourceType.Iron, 100);
        targetGS.addResourcePerTick(ResourceType.Iron, 5);
        //targetGS.setStockpile(ResourceType.Steel, 10);
        targetGS.addResourcePerTick(ResourceType.Steel, 5);

        UnityEngine.Debug.Log(Stopwatch.Frequency);
        UnityEngine.Debug.Log("StartTime: " + Stopwatch.GetTimestamp());
        ltp.plan(initialGS, targetGS, callback);
    }

    private bool callback(Stack<Work> work) {
        UnityEngine.Debug.Log("Finished");
        return true;
    }


}
