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
        initialGS.gold = 100;
        initialGS.goldPerTick = 1;


        GameState targetGS = new GameState();
        targetGS.gold = 500000;
        targetGS.goldPerTick = 100;
        int buildBank = 0;
        int waitCount = 0;
        foreach(Work w in ltp.plan(initialGS, targetGS))
        {
            if (w == Work.EMPTY) {
                Debug.Log("EMPTY");
            } else if (w == Work.BuildBank)
            {
                buildBank++;
                //Debug.Log("BUILD BANK !!!!!!!!!!!!!!!!!!!!!");
            }
            else if (w == Work.Wait)
            {
                waitCount++;
                //Debug.Log("wait");
            } else
            {
                Debug.Log(Enum.GetName(w.GetType(), w));
            }
        }
        Debug.Log("Wait count:  " + waitCount + "  bank build:  " + buildBank);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
