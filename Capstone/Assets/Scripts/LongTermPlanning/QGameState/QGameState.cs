using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QGameState {
    // The thing that gets put onto the priority queue           

    public QGameState parent;
    public Work transitionWork; // transitionWork represents the job done to move from parent to currentGS
                                // NOTE: This will be EMPTY if parent is null (ie if this QE is the root qe)

    public BuildingGS gameState;
    public int costToGetHere;

    public QGameState(BuildingGS gs, QGameState parent, Work transitionWork, int costToGetHere)
    {
        this.gameState = gs;
        this.parent = parent;
        this.transitionWork = transitionWork;
        this.costToGetHere = costToGetHere;
    }
}
