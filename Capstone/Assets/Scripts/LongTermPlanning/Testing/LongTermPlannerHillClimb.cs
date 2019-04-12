using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class LongTermPlannerHillClimb : MonoBehaviour {

    public ALongTermPlanner ltp;
    

    public void hillClimb() {

        BuildingGS initialGS = new BuildingGS();
        initialGS.setStockpile(ResourceType.Gold, 100);
        initialGS.addResourcePerTick(ResourceType.Gold, 0);
        initialGS.setStockpile(ResourceType.Stone, 100);
        initialGS.addResourcePerTick(ResourceType.Stone, 0);
        initialGS.setStockpile(ResourceType.Wood, 100);
        initialGS.addResourcePerTick(ResourceType.Wood, 0);


        BuildingGS targetGS = new BuildingGS();
        targetGS.setStockpile(ResourceType.Gold, 20000);
        targetGS.addResourcePerTick(ResourceType.Gold, 0);
        targetGS.setStockpile(ResourceType.Stone, 20000);
        targetGS.addResourcePerTick(ResourceType.Stone, 0);
        targetGS.setStockpile(ResourceType.Wood, 20000);
        targetGS.addResourcePerTick(ResourceType.Wood, 0);
        targetGS.setStockpile(ResourceType.Silver, 20000);
        targetGS.addResourcePerTick(ResourceType.Silver, 0);

        UnityEngine.Debug.Log(Stopwatch.Frequency);
        UnityEngine.Debug.Log("StartTime: " + Stopwatch.GetTimestamp());
        ltp.plan(initialGS, targetGS, callback);
    }

    private bool callback(Stack<Work> work) {
        UnityEngine.Debug.Log("Finished");
        return true;
    }


}
