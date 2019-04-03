using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class ALongTermPlanner : MonoBehaviour {
    protected bool finished = false;
    protected Stack<Work> result;

    public abstract void plan(BuildingGS initialGS,  BuildingGS targetGS, Func<Stack<Work>, bool> callback);

    protected IEnumerator waitForFinish(Func<Stack<Work>, bool> callback) {
        // Check every 0.5 seconds if the path has been found + processed.
        // Once it's ready, return the result to the provided callback
        while (!finished) { yield return new WaitForSeconds(0.5f); }
        callback(result);
    }
    
    protected bool processResult(QGameState engineOutput) {
        // Process the output result from the LTPEngine
        if (engineOutput == null) {
            Debug.Log("No path found");
            this.result = new Stack<Work>();
        }

        Stack<Work> result = new Stack<Work>();
        result.Push(Work.Wait); // TODO: there's some bug that causes the Engine to end one step short :( 
        while (engineOutput != null) {
            // Note, the finalResult value should be the targetGS. IE: We're looping backwards from
            // finish to start. So last element into the list == the first unit of work that
            // should be done
            result.Push(engineOutput.transitionWork);
            engineOutput = engineOutput.parent;
        }

        this.result = result;
        this.finished = true;
        return true;
    }
}
