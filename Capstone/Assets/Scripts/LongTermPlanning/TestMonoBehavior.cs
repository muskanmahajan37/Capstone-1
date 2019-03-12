using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestMonoBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LongTermPlanning ltp = new LongTermPlanning();


        GameState initialGS = new GameState();
        Resource gold = new RoundedResource("gold", 5, 100, 2, 2);
        initialGS.addResource(gold);


        GameState targetGS = new GameState();
        Resource targetGold = new RoundedResource("gold", 5, 5000000, 10, 2);
        targetGS.addResource(targetGold);

        int buildBank = 0;
        int waitCount = 0;
        foreach(Work w in ltp.plan(initialGS, targetGS))
        {
            if (w == Work.EMPTY) {
            } else if (w == Work.BuildBank)
            {
                buildBank++;
            }
            else if (w == Work.Wait)
            {
                waitCount++;
            }
        }
        Debug.Log("Wait count:  " + waitCount + "  bank build:  " + buildBank);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
