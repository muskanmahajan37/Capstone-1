using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MidLevelManager : MonoBehaviour {
    
    public GameController gc = GameController.singleton;
    public ALongTermPlanner ltp;

    public void test() {

        BuildingGS initialGS = new BuildingGS();
        initialGS.setStockpile(ResourceType.Gold, 1000);
        initialGS.addResourcePerTick(ResourceType.Gold, 0);
        initialGS.setStockpile(ResourceType.Stone, 1000);
        initialGS.addResourcePerTick(ResourceType.Stone, 0);
        initialGS.setStockpile(ResourceType.Wood, 1000);
        initialGS.addResourcePerTick(ResourceType.Wood, 0);


        BuildingGS targetGS = new BuildingGS();
        targetGS.setStockpile(ResourceType.Gold, 200);
        targetGS.addResourcePerTick(ResourceType.Gold, 60);
        targetGS.setStockpile(ResourceType.Stone, 200);
        targetGS.addResourcePerTick(ResourceType.Stone, 30);
        targetGS.setStockpile(ResourceType.Wood, 200);
        targetGS.addResourcePerTick(ResourceType.Wood, 30);
        targetGS.setStockpile(ResourceType.Silver, 200);
        targetGS.addResourcePerTick(ResourceType.Silver, 30);

        ltp.plan(initialGS, targetGS, callback);
    }

    private bool callback(Stack<Work> work) {
        StartCoroutine(executeOrders(work));
        return true;
    }

    private IEnumerator executeOrders(Stack<Work> workOrder) {
        foreach (Work w in workOrder) {
            switch (w.workType) {
                case EWork.Wait:
                case EWork.EMPTY:
                    // A wait object is yield returned after the switch
                    break;

                case EWork.BuildBuilding:
                    Vector2Int nextPos = this.nextPos();
                    IBuilding b = BuildingFactory.buildNew(w.buildingType, nextPos.x, nextPos.y);
                    if (!gc.canBuildBuilding(w.buildingType, nextPos.x, nextPos.y))
                        { throw new System.Exception("Can't build the building for some reason"); }
                    gc.startBuildBuilding(b);
                    break;

                case EWork.BuyAndAssignWorker:
                    if (!gc.canBuyWorker())
                        { throw new System.Exception("Can't buy worker for some reason"); }
                    gc.forceBuyWorker();
                    IBuilding targetBuilding = gc.getAnyOpenBuilding(w.buildingType);
                    if (targetBuilding == null)
                        { throw new System.Exception("No open building to assign worker to"); }
                    gc.cleanAssignWorker(targetBuilding);
                    break;

                default:
                    throw new System.Exception("Unknown work type: " + Enum.GetName(typeof(EWork), w.workType));

            }
            yield return new WaitForSeconds(w.frameWait * GameSetup.TICK_LENGHT_SEC);
        }
        yield break;
    }


    private int currentX = 5;
    private int currentyY = 5;
    private Vector2Int nextPos() {
        Vector2Int result = new Vector2Int(currentX, currentyY);
        currentX++;
        if (currentX >= GameSetup.BOARD_WIDTH) {
            currentX = 5;
            currentyY++;
        }
        return result;
    }
}
