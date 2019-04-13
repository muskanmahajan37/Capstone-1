using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PriorityQueueDemo;

public static class LTPEngine {

    /*
     * How many cycles/ nodes should we check per in game frame? 
     * Increasing this number may cause substantial lag,
     * decreasing this number may take longer real world time to calculate a path
     */
    private static readonly int NODES_CHECKED_PER_YIELD = 1; // Check n nodes every frame


    public static IEnumerator BuildPlan(BuildingGS initialGS,
                                         BuildingGS targetGS,
                                         PriorityQueue<QPriority, QGameState> priorityQueue,
                                         Func<QGameState, bool> finishCallback) {
        int nodesChecked = 0;
        // Initialize data structures
        Dictionary<int, int> bestCostToGetHere = new Dictionary<int, int>();

        // NOTE: Don't add the initial state to the bestCostToGetHere, that will be done automatically
        Work initialWork = new Work(EWork.EMPTY, BuildingType.NONE, 0);
        QGameState firstGS = new QGameState(initialGS, null, initialWork, 0);
        QPriority firstPriority = new QPriority(firstGS, targetGS);
        priorityQueue.Enqueue(firstPriority, firstGS);


        int totalChecks = 0;
        while (priorityQueue.Count > 0) {
            nodesChecked++;
            if (nodesChecked >= NODES_CHECKED_PER_YIELD) {
                // TODO: There may be an off by 1 error, but not a major problem
                nodesChecked = 0;
                yield return null;
            }
            totalChecks++;
            KeyValuePair<QPriority, QGameState> kvp = priorityQueue.Dequeue();
            QGameState qe = kvp.Value;

            Debug.Log("===============================================================================================");
            Debug.Log("Cost to get here: " + qe.costToGetHere + " + " + kvp.Key.maxWaitTime);
            Debug.Log("incoming work: " + qe.transitionWork.workType + "  " + qe.transitionWork.buildingType);

            // Early exit conditions
            if (LTPHelper.estematedRemainingDistance(qe.gameState, targetGS).atTarget()) {
                // If we are 0 distance away from the target game state
                // IE: If we have found the target game state
                Debug.Log("Nocab flag 1");
                finishCallback(qe);
                yield break;
            } else if (totalChecks > LTPHelper.MAX_DEPTH) {
                Debug.Log("Nocab flag 2");
                finishCallback(null);
                yield break;
            }

            if (bestCostToGetHere.ContainsKey(qe.gameState.GetHashCode()) &&
                bestCostToGetHere[qe.gameState.GetHashCode()] <= qe.costToGetHere) {
                // If we've already explored this game state
                // AND if some other path is to this game state is cheeper
                continue;
            } else {
                // Else, this Queue Entry represents a cheeper path to get to this node
                bestCostToGetHere[qe.gameState.GetHashCode()] = qe.costToGetHere;
            }

            foreach (QGameState neighbor in LTPHelper.getNeighbors(qe)) {
                // The neighbor already has an updated gamestate, a job and an updated cost

                if (bestCostToGetHere.ContainsKey(neighbor.gameState.GetHashCode()) &&
                    bestCostToGetHere[neighbor.gameState.GetHashCode()] <= neighbor.costToGetHere) {
                    // If we already have a better way to get to the neighbor
                    continue;
                }

                QPriority heuristic = new QPriority(neighbor, targetGS);
                
                Debug.Log("********************");
                Debug.Log("**  Adding neighbor: ");
                Debug.Log("**  transition work: " + neighbor.transitionWork.workType + "  " + neighbor.transitionWork.buildingType);
                Debug.Log(" +  infinities     : " + heuristic.numberOfInfinities);
                Debug.Log(" +  unaquiriable   : " + heuristic.unaquirableResourceCount);
                Debug.Log(" +  totalCPTDelta  : " + heuristic.totalCPTDelta);
                Debug.Log(" +  best CPTDelta  : " + heuristic.bestCPTDelta);
                Debug.Log(" +  estTotalDist   : " + heuristic.estTotalDist);
                Debug.Log(" +  fudge factor   : " + heuristic.totalResourcesSpent);
                Debug.Log("iron  cpt: " + neighbor.gameState.getChangePerTick(ResourceType.Iron));
                Debug.Log("coal  cpt: " + neighbor.gameState.getChangePerTick(ResourceType.Coal));
                Debug.Log("Steel cpt: " + neighbor.gameState.getChangePerTick(ResourceType.Steel));
                Debug.Log("building count: " + neighbor.gameState.totalBuildingCount());
                Debug.Log("worker count: " + neighbor.gameState.totalWorkerCount());

                priorityQueue.Enqueue(heuristic, neighbor);
            } // End foreach neighbor
        } // End while queue is NOT empty

        Debug.Log("Nocab flag 3");
        finishCallback(null);
        yield break;
    }
}

