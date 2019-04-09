using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueueDemo;

public class UnmodifiedLongTermPlanner : ALongTermPlanner {


    public override void plan(BuildingGS initialGS, BuildingGS targetGS, Func<Stack<Work>, bool> callback) {

        PriorityQueue<int, QGameState> queue = new PriorityQueue<int, QGameState>();
        StartCoroutine(LTPEngine.BuildPlan(initialGS, targetGS, queue, base.processResult));
        StartCoroutine(base.waitForFinish(callback));

    }
}
