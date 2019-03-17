using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueueDemo;
using System;

public class LongTermPlanning {

    public static readonly int WAIT_TIME = 10;
    private static readonly int MAX_DEPTH = 5000000; // 5M searches


    public static int compareGameState(GameState currentGS, GameState targetGS) {
        // How many time cycles will it take if we do nothing? 
        // NOTE: It's expected that currentGS and targetGS have the types of resources
        //       (Or at the least, that targetGS is a subset of currentGS)

        int maxWaitTime = 0; 
        int totalRPTDelta = 0;

        foreach (var kvp in targetGS.resources) {
            Resource currentResource = currentGS.resources[kvp.Key];
            Resource targetResource = kvp.Value;

            // How long will it take to get to the target if we just wait? 
            int resourceDelta = Mathf.Max(0, targetResource.resourceCount - currentResource.resourceCount);
            int resourceWaitTime = resourceDelta / currentResource.resourcePerTick;

            // How far are we from the target resource income rate? 
            int RPTDelta = Mathf.Max(0, targetResource.resourcePerTick - currentResource.resourcePerTick);

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

    private HashSet<QGameState> getNeighbors(QGameState qEntry) {
        // For a given game state return all valid edges out of it

        HashSet<QGameState> result = new HashSet<QGameState>();
        result.Add(waitTransition(qEntry, WAIT_TIME));

        if (canBuildGoldMiner(qEntry))
            { result.Add(buildGoldMiner(qEntry)); }


        return result;
    }

    public Queue<Work> plan(GameState initialGS, GameState targetGS) {
        // TODO: This function is the exact same as the AStart.getPath() function

        QGameState currentQE = BuildPlan(initialGS, targetGS);

        foreach (var kvp in currentQE.gameState.resources) {
            Debug.Log("Final game state " + kvp.Value.name + ": " + kvp.Value.resourceCount);
            Debug.Log("Final game state " + kvp.Value.name + " per tick: " + kvp.Value.resourcePerTick);
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


    private QGameState BuildPlan(GameState initialGS, GameState targetGS) {

        PriorityQueue<int, QGameState> priorityQueue = new PriorityQueue<int, QGameState>();

        Dictionary<GameState, int> bestCostToGetHere = new Dictionary<GameState, int>();


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
                    //Debug.Log("Ignoring edge");
                    //Debug.Log("          gold resource count: " + neighbor.gameState.resources["gold"].resourceCount);
                    //Debug.Log("          gold worker count: " + neighbor.gameState.resources["gold"].workerCount);
                    //Debug.Log("          first check: " + bestCostToGetHere.ContainsKey(neighbor.gameState));
                    //Debug.Log("          second check: " + bestCostToGetHere[neighbor.gameState] + " vs " + neighbor.costToGetHere);
                    continue;
                }


                heuristic = compareGameState(neighbor.gameState, targetGS) + neighbor.costToGetHere;
                priorityQueue.Enqueue(heuristic, neighbor);

            }

        } // End while queue is NOT empty
        return null;

    }

    private class QGameState {
        // The thing that gets put onto the priority queue           

        public QGameState parent;
        public Work transitionWork; // transitionWork represents the job done to move from parent to currentGS
                                    // NOTE: This will be EMPTY if parent is null (ie if this QE is the root qe)

        public GameState gameState;
        public int costToGetHere;

        public QGameState(GameState gs, QGameState parent, Work transitionWork, int costToGetHere) {
            this.gameState = gs;
            this.parent = parent;
            this.transitionWork = transitionWork;
            this.costToGetHere = costToGetHere;
        }
    }


    ////////////////////////////////////////////////

    // This function represents an edge
    private static QGameState waitTransition(QGameState qe, int time) {
        GameState endGS = waitGameState(qe.gameState, time);
        int timeCost = qe.costToGetHere + time;
        return new QGameState(endGS, qe, Work.Wait, timeCost);
    }

    private static GameState waitGameState(GameState gs, int time) {
        GameState endGS = new GameState(gs);
        endGS.timePasses(time);
        return endGS;
    }

    //////////////////////////////////////////////////

    private static readonly int workerBuildTime = 1;
    private static readonly int workerCostGold = 100;
    private static bool canBuildGoldMiner(QGameState qe) {
        return qe.gameState.resources["gold"].resourceCount >= workerCostGold;
    }

    private static QGameState buildGoldMiner(QGameState qe) {
        GameState oldGameState = qe.gameState;
        GameState newGameState = waitGameState(oldGameState, workerBuildTime);

        // Subtract the cost of building a worker
        newGameState.resources["gold"].resourceCount -= workerCostGold;
        newGameState.resources["gold"].workerCount++;

        int timeCost = qe.costToGetHere + workerBuildTime;
        return new QGameState(newGameState, qe, Work.NewGoldMiner, timeCost);

    }
}




