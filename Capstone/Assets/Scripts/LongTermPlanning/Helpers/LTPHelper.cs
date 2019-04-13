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
            result.updateCPTDelta(rptDelta);

            int currentBestResourcePerTick = currentGS.getBestPossibleChangePerTick(rt);
            int bestPossibleRPTDelta = Mathf.Max(0, targetResourcePerTick - currentBestResourcePerTick);
            result.updateBestPossibleCPTDelta(bestPossibleRPTDelta);

            if (currentResourcePerTick <= 0) { 
                result.addInfinity();
            } else {
                float exactWaitTime = stockpileDelta / (float)currentResourcePerTick;
                int estWaitTime = (int)(exactWaitTime + 0.5f);
                result.updateWaitTime(estWaitTime);
            }
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

    private int _numberOfInfinities = 0;
    public int NumberOfInfinities { get { return _numberOfInfinities; } }

    private int _maxWaitTime = 0;
    public int MaxWaitTime { get { return _maxWaitTime; } }

    private int _totalWaitTime = 0;
    public int TotalWaitTime { get { return _totalWaitTime; } }

    private int _totalChangePerTickDelta = 0;
    public int TotalChangePerTickDelta { get { return _totalChangePerTickDelta; } }

    private int _bestChangePerTickDelta = 0;
    public int BestChangePerTickDelta { get { return _bestChangePerTickDelta; } }

    public void addInfinity() { _numberOfInfinities++; }

    public void updateWaitTime(int estTime) {
        _totalWaitTime += estTime;
        _maxWaitTime = Mathf.Max(_maxWaitTime, estTime);
    }

    public void updateCPTDelta(int cptDelta) {
        this._totalChangePerTickDelta += cptDelta;
    }

    public void updateBestPossibleCPTDelta(int cptDelta) {
        this._bestChangePerTickDelta += cptDelta;
    }

    public bool atTarget() {
        return _numberOfInfinities <= 0 && 
               _totalWaitTime <= 0 &&
               _totalChangePerTickDelta <= 0;
    }
}
