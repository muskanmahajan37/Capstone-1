using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueueDemo;
using System;

public static class LongTermPlanningBuildings {

    private static readonly int WAIT_TIME = 10;
    private static readonly int MAX_DEPTH = 500000; // 500K searches


    public static int compareGameState(BuildingGS currentGS, BuildingGS targetGS) {
        // How many time cycles will it take if we do nothing? 
        // NOTE: It's expected that currentGS and targetGS have the types of resources
        //       (Or at the least, that targetGS is a subset of currentGS)

        int maxWaitTime = 0; 
        int totalRPTDelta = 0;

        foreach (ResourceType rt in targetGS.getResourceType()) {
            int currentStockpile = currentGS.getStockpile(rt);
            int targetStockpile = targetGS.getStockpile(rt);

            int currentResourcePerTick = currentGS.getPerTickChange(rt);
            int targetResourcePerTick = targetGS.getPerTickChange(rt);

            // How long will it take to get to the target if we just wait? 
            int resourceDelta = Mathf.Max(0, targetStockpile - currentStockpile);

            int resourceWaitTime = (currentResourcePerTick == 0) ?
                                       WAIT_TIME * 1000 : // If we make this value int.maxValue then we have overflow problems
                                       resourceDelta / currentResourcePerTick;

            // How far are we from the target resource income rate? 
            int RPTDelta = Mathf.Max(0, targetResourcePerTick - currentResourcePerTick);

            maxWaitTime = Mathf.Max(maxWaitTime, resourceWaitTime);
            totalRPTDelta += RPTDelta;
        }


        return maxWaitTime + totalRPTDelta;
    }

    // TODO: put this somewhere else
    public static uint sigfigify(int n, int sigFigs) { return sigfigify((uint)n, sigFigs); }
    public static uint sigfigify(uint n, int sigFigs) {
        // NOTE: When casting a negative int into a uint the far left "negative marker" will cout as a sig fig

        int sizeOfN = 0;
        uint nPrime = n;
        while (nPrime > 0) {
            nPrime /= 10;
            sizeOfN++;
        }

        if (sigFigs >= sizeOfN) { return n; }

        int digitsCut = sizeOfN - sigFigs;
        uint trimmedN = n / (uint)Mathf.Pow(10, digitsCut); // Cut off the right most values
        return trimmedN * (uint)Mathf.Pow(10, digitsCut); // Replace them all with 0s 
    }

    private static HashSet<QGameState> getNeighbors(QGameState qEntry) {
        // For a given game state return all valid edges out of it

        HashSet<QGameState> result = new HashSet<QGameState>();
        result.Add(waitTransition(qEntry, WAIT_TIME));

        BuildingGS gs = qEntry.gameState;


        if (gs.canBuyWorker()) {
            // If we have the resources to build a new worker
            foreach (BuildingType bt in gs.getOpenSlots()) {
                // One branch for every possible type of worker slot we can fill
                QGameState neighbor = buyWorker(qEntry, bt);
                result.Add(neighbor);
            }
        }
        
        // TODO: Possible building of buildings 
        //       NOTE: why build a building if we can't populate it with a worker imediataly? 

        return result;
    }

    public static Queue<Work> plan(BuildingGS initialGS, BuildingGS targetGS) {

        float startTime = Time.realtimeSinceStartup;
        QGameState currentQE = BuildPlan(initialGS, targetGS);

        float endTime = Time.realtimeSinceStartup;
        Debug.Log(startTime);
        Debug.Log(endTime);
        Debug.Log("delta Time: "); Debug.Log((endTime - startTime));

        if (currentQE == null)
        {
            Debug.Log("No path found");
            return new Queue<Work>();
        }

        foreach (ResourceType rt in currentQE.gameState.getResourceType()) {
            Debug.Log("Final game state " + Enum.GetName(rt.GetType(), rt) + ": " + currentQE.gameState.getStockpile(rt));
            Debug.Log("Final game state " + Enum.GetName(rt.GetType(), rt) + " per tick: " + currentQE.gameState.getPerTickChange(rt));
        }

        Debug.Log("Total Time: " + currentQE.costToGetHere);

        // TODO: using double ended queues may be more efficent
        List<Work> tempList = new List<Work>();

        while (currentQE != null) {
            tempList.Add(currentQE.transitionWork);
            currentQE = currentQE.parent;
        }

        tempList.Reverse();
        Queue<Work> result = new Queue<Work>(tempList);
        return result;

    }


    private static QGameState BuildPlan(BuildingGS initialGS, BuildingGS targetGS) {

        //AccordionPriorityQueue<int, QGameState> priorityQueue = new AccordionPriorityQueue<int, QGameState>(400);
        MemoryBoundedPriorityQueue<int, QGameState> priorityQueue = new MemoryBoundedPriorityQueue<int, QGameState>(200);
        //PriorityQueue<int, QGameState> priorityQueue = new PriorityQueue<int, QGameState>(200);

        Dictionary<BuildingGS, int> bestCostToGetHere = new Dictionary<BuildingGS, int>();


        QGameState startQE = new QGameState(initialGS, null, Work.EMPTY, 0);
        int heuristic = compareGameState(initialGS, targetGS);
        priorityQueue.Enqueue(heuristic, startQE);

        int totalChecks = 0;

        while (priorityQueue.Count > 0) {

            totalChecks++;

            QGameState qe = priorityQueue.DequeueValue();

            if (compareGameState(qe.gameState, targetGS) <= 0)
            {
                // If we are 0 distance away from the target game state
                // IE: If we have found the target game state
                return qe;
            }

            if (totalChecks > MAX_DEPTH)
            {
                Debug.Log("exit case max depth");
                return null;
            }

            if (bestCostToGetHere.ContainsKey(qe.gameState) &&
                bestCostToGetHere[qe.gameState] <= qe.costToGetHere) {
                // If we've already explored this game state
                // AND if some other path is to this game state is cheeper
                continue;
            } else {
                // Else, this Queue Entry represents a cheeper path to get to this node
                bestCostToGetHere[qe.gameState] = qe.costToGetHere;
            }


            foreach(QGameState neighbor in getNeighbors(qe)) {
                // The neighbor already has an updated gamestate, a job and an updated cost



                if (bestCostToGetHere.ContainsKey(neighbor.gameState) &&
                    bestCostToGetHere[neighbor.gameState] <= neighbor.costToGetHere) {
                    // If we already have a better way to get to the neighbor
                    continue;
                }

                heuristic = compareGameState(neighbor.gameState, targetGS) + neighbor.costToGetHere;
                priorityQueue.Enqueue(heuristic, neighbor);

            }

        } // End while queue is NOT empty
        Debug.Log("exit case no path");
        return null;

    }

    private class QGameState {
        // The thing that gets put onto the priority queue           

        public QGameState parent;
        public Work transitionWork; // transitionWork represents the job done to move from parent to currentGS
                                    // NOTE: This will be EMPTY if parent is null (ie if this QE is the root qe)

        public BuildingGS gameState;
        public int costToGetHere;

        public QGameState(BuildingGS gs, QGameState parent, Work transitionWork, int costToGetHere) {
            this.gameState = gs;
            this.parent = parent;
            this.transitionWork = transitionWork;
            this.costToGetHere = costToGetHere;
        }
    }


    ////////////////////////////////////////////////

    // This function represents an edge
    private static QGameState waitTransition(QGameState qe, int time) {
        BuildingGS endGS = waitGameState(qe.gameState, time);
        int timeCost = qe.costToGetHere + time;
        return new QGameState(endGS, qe, Work.Wait, timeCost);
    }

    private static BuildingGS waitGameState(BuildingGS gs, int time) {
        BuildingGS endGS = new BuildingGS(gs);
        endGS.timePasses(time);
        return endGS;
    }

    //////////////////////////////////////////////////
    

    private static QGameState buyWorker(QGameState qGS, BuildingType bt) {
        BuildingGS newGameState = waitGameState(qGS.gameState, 0);
        newGameState.buyAndAssignWorker(bt);

        return new QGameState(newGameState, qGS, WorkHelper.assignWorkerTo(bt), 0);
    }
    
}
