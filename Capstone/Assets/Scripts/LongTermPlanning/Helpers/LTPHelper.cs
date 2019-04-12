using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class LTPHelper {
    
    public static readonly int MAX_DEPTH = 500000; // 500K searches


    public static int unaquirableResources(BuildingGS currentGS, BuildingGS targetGS) {
        // Are there any resources that are impossible to get at this moment (ignoring workers)
        HashSet<ResourceType> unaquirableResources = targetGS.getCPTResourceTypes();
        unaquirableResources.ExceptWith(currentGS.slotsForIncome());
        return unaquirableResources.Count;
    }

    public static RemainingDistance estematedRemainingDistance(BuildingGS currentGS, BuildingGS targetGS) {
        // How far away are we from the target? 
        RemainingDistance result = new RemainingDistance();

        // Compare numerical distance
        foreach (ResourceType rt in targetGS.getAllResourceTypes()) {
            int currentStockpile = currentGS.getStockpile(rt);
            int targetStockpile = targetGS.getStockpile(rt);
            int stockpileDelta = Mathf.Max(0, targetStockpile - currentStockpile);

            int currentResourcePerTick = currentGS.getChangePerTick(rt);
            int targetResourcePerTick = targetGS.getChangePerTick(rt);
            int rptDelta = Mathf.Max(0, targetResourcePerTick - currentResourcePerTick);

            if (currentResourcePerTick == 0) { 
                result.addInfinity();
            } else {
                float exactWaitTime = stockpileDelta / (float)currentResourcePerTick;
                int estWaitTime = (int)(exactWaitTime + 0.5f);
                result.updateWaitTime(estWaitTime);
            }

            result.updateCPTDelta(rptDelta);

        }
        return result;
    }

    public static HashSet<QGameState> getNeighbors(QGameState qEntry) {
        // For a given game state return all valid edges out of it

        HashSet<QGameState> result = new HashSet<QGameState>();
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

        // The length of all the no-op edges we want to consider
        HashSet<int> waitTimes = new HashSet<int>() { 10 };

        // Branches related to Buildings
        // TODO: Why build a building if we can't populate it with a worker? 
        foreach (BuildingType bt in BuildingFactory.allBuildings) {
            // One branch for every new possible building
            IBuilding possibleBuilding = BuildingFactory.buildNew(bt, -1, -1); // TODO: do we care about pos when doing A*? 
            if (gs.canBuyBuilding(possibleBuilding)) {
                // If we can build this building, then add a branch
                QGameState neighbor = QGameStateFactory.buyBuilding(qEntry, possibleBuilding);
                result.Add(neighbor);

                // For increased fidelity, each building time can also be a no-op
                //waitTimes.Add(possibleBuilding.timeToBuild());
            }
        }

        // Add in some no-op edges
        foreach (int waitTime in waitTimes) {
            result.Add(QGameStateFactory.waitTransition(qEntry, waitTime));
        }

        return result;
    }
}

public class RemainingDistance {

    private int numberOfInfinities = 0;
    public int NumberOfInfinities { get { return numberOfInfinities; } }

    private int maxWaitTime = 0;
    public int MaxWaitTime { get { return maxWaitTime; } }

    private int totalWaitTime = 0;
    public int TotalWaitTime { get { return totalWaitTime; } }

    private int totalChangePerTickDelta = 0;
    public int TotalChangePerTickDelta { get { return totalChangePerTickDelta; } }



    public void addInfinity() { numberOfInfinities++; }

    public void updateWaitTime(int estTime) {
        totalWaitTime += estTime;
        maxWaitTime = Mathf.Max(maxWaitTime, estTime);
    }

    public void updateCPTDelta(int cptDelta) {
        this.totalChangePerTickDelta += cptDelta;
    }

    public bool atTarget() {
        return numberOfInfinities == 0 && 
               totalWaitTime == 0 &&
               totalChangePerTickDelta == 0;
    }
}
