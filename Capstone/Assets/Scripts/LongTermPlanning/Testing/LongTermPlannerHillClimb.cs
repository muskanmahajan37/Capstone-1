using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LongTermPlannerHillClimb : MonoBehaviour {

    public void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            Debug.Log("Starting");
            StartCoroutine(hillClimb());
            //StartCoroutine(waiting());
            Debug.Log("nocab flag 1");
        }
    }


    public IEnumerator waiting() {
        Debug.Log("Waiting");
        yield return new WaitForSeconds(4);
    }


    public IEnumerator hillClimb() {

        BuildingGS initialGS = new RoundableBuildingGameState();
        initialGS.setStockpile(ResourceType.Gold, 1000);
        initialGS.addResourcePerTick(ResourceType.Gold, 0);
        initialGS.setStockpile(ResourceType.Stone, 1000);
        initialGS.addResourcePerTick(ResourceType.Stone, 0);
        initialGS.setStockpile(ResourceType.Wood, 1000);
        initialGS.addResourcePerTick(ResourceType.Wood, 0);

        
        BuildingGS targetGS = new RoundableBuildingGameState();
        targetGS.setStockpile(ResourceType.Gold, 20000);
        targetGS.addResourcePerTick(ResourceType.Gold, 900);
        targetGS.setStockpile(ResourceType.Stone, 20000);
        targetGS.addResourcePerTick(ResourceType.Stone, 900);
        targetGS.setStockpile(ResourceType.Wood, 20000);
        targetGS.addResourcePerTick(ResourceType.Wood, 900);

        float prevHash = float.MaxValue;
        int direction = 1;

        for (int i = 0; i < 30; i++) {
            float startTime = Time.realtimeSinceStartup;
            int inGameTime = LongTermPlanningBuildings.planTotalIGTime(initialGS, targetGS);
            float endTime = Time.realtimeSinceStartup;


            if (inGameTime >= 0) {
                float realWorldTime = (endTime - startTime);
                float hash = (realWorldTime * 100 ) + inGameTime;
                Debug.Log("Version: " + i + "   IGTime: " + inGameTime + "   realTime: " + realWorldTime + "  hash: " + hash + "  wait_time: " + LongTermPlanning.public_wait_time);

                if (hash < prevHash) {
                    // Keep going in that direction
                    LongTermPlanning.public_wait_time = LongTermPlanning.public_wait_time + direction;
                } else {
                    // TODO: Strange things may happend with a negitive weight time :(
                    LongTermPlanning.public_wait_time = LongTermPlanning.public_wait_time - direction;
                }
                prevHash = hash;
            }
            
            yield return null;
        }

    }


}
