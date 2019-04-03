using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QGameStateFactory {

    // This function represents an edge
    public static QGameState waitTransition(QGameState qe, int time) {
        BuildingGS endGS = waitGameState(qe.gameState, time);
        int timeCost = qe.costToGetHere + time;
        return new QGameState(endGS, qe, Work.Wait, timeCost);
    }

    public static BuildingGS waitGameState(BuildingGS gs, int time) {
        BuildingGS endGS = new BuildingGS(gs);
        endGS.timePasses(time);
        return endGS;
    }

    //////////////////////////////////////////////////


    public static QGameState buyWorker(QGameState qGS, BuildingType bt) {
        BuildingGS newGameState = waitGameState(qGS.gameState, 1);  // TODO: We currently think a worker takes 1 ticks to build and assign
        newGameState.buyAndAssignWorker(bt);
        int costToGetHere = qGS.costToGetHere + 1;   // TODO: Worker time currently = 1  :(
        return new QGameState(newGameState, qGS, WorkHelper.assignWorkerTo(bt), costToGetHere);
    }

    public static QGameState buyBuilding(QGameState qGS, IBuilding newBuilding) {
        BuildingGS newGameState = waitGameState(qGS.gameState, 10); // TODO: Currently we assume all buildings take 10 ticks to build
        newGameState.buyBuilding(newBuilding);
        int costToGetHere = qGS.costToGetHere + 10;    // TODO: Building time currently = 10 :(
        return new QGameState(newGameState, qGS, WorkHelper.buildBuilding(newBuilding), costToGetHere);
    }
}
