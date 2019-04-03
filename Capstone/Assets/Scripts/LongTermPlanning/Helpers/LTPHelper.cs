using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LTPHelper {

    public static readonly int WAIT_TIME = 10;
    public static readonly int MAX_DEPTH = 500000; // 500K searches


    public static int compareGameState(BuildingGS currentGS, BuildingGS targetGS) {
        // How many time cycles will it take if we do nothing? 
        // NOTE: It's expected that currentGS and targetGS have the types of resources
        //       (Or at the least, that targetGS is a subset of currentGS)

        int maxWaitTime = 0;
        //int totalWaitTime = 0;
        int totalRPTDelta = 0;

        foreach (ResourceType rt in targetGS.getAllResourceTypes())  {
            int currentStockpile = currentGS.getStockpile(rt);
            int targetStockpile = targetGS.getStockpile(rt);

            int currentResourcePerTick = currentGS.getPerTickChange(rt);
            int targetResourcePerTick = targetGS.getPerTickChange(rt);

            // How long will it take to get to the target if we just wait? 
            int resourceDelta = Mathf.Max(0, targetStockpile - currentStockpile);

            int resourceWaitTime = (currentResourcePerTick == 0) ?
                                       resourceDelta : // If we make this value int.maxValue then we have overflow problems
                                       resourceDelta / currentResourcePerTick;

            // How far are we from the target resource income rate? 
            int RPTDelta = Mathf.Max(0, targetResourcePerTick - currentResourcePerTick);
            
            maxWaitTime = Mathf.Max(maxWaitTime, resourceWaitTime);
            //totalWaitTime += resourceWaitTime;
            totalRPTDelta += RPTDelta;
        }
        
        //return totalRPTDelta + totalWaitTime;
        return maxWaitTime + totalRPTDelta;
    }

    public static HashSet<QGameState> getNeighbors(QGameState qEntry) {
        // For a given game state return all valid edges out of it

        HashSet<QGameState> result = new HashSet<QGameState>();
        result.Add(QGameStateFactory.waitTransition(qEntry, WAIT_TIME));

        BuildingGS gs = qEntry.gameState;
        
        // Branches related to workers
        if (gs.canBuyWorker()) {
            // If we have the resources to build a new worker
            foreach (BuildingType bt in gs.getOpenSlots()) {
                // One branch for every possible type of worker slot we can fill
                QGameState neighbor = QGameStateFactory.buyWorker(qEntry, bt);
                result.Add(neighbor);
            }
        }
        
        // Branches related to Buildings
        // TODO: Why build a building if we can't populate it with a worker? 
        foreach (BuildingType bt in BuildingFactory.allBuildings) {
            // One branch for every new possible building
            IBuilding possibleBuilding = BuildingFactory.buildNew(bt, -1, -1); // TODO: do we care about pos when doing A*? 
            if (qEntry.gameState.canBuyBuilding(possibleBuilding)) {
                // If we can build this building, then add a branch
                QGameState neighbor = QGameStateFactory.buyBuilding(qEntry, possibleBuilding);
                result.Add(neighbor);
            }
        }

        return result;
    }
}
