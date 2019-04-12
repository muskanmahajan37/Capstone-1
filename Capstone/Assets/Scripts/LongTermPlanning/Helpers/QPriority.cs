using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QPriority : IComparable {

    private QGameState currentNode;
    public int costToGetHere { get { return this.currentNode.costToGetHere; } }

    private BuildingGS targetGS;
    public int unaquirableResourceCount { get { return LTPHelper.unaquirableResources(currentNode.gameState, targetGS); } }
    public int totalResourcesSpent { get { return currentNode.gameState.totalResourcesSpent; } }

    private RemainingDistance distanceRuler;
    public int numberOfInfinities { get { return distanceRuler.NumberOfInfinities; } }
    public int maxWaitTime        { get { return distanceRuler.MaxWaitTime; } }
    public int totalWaitTime      { get { return distanceRuler.TotalWaitTime; } }
    public int totalCPTDelta      { get { return distanceRuler.TotalWaitTime; } }

    public int estTotalDist        { get { return this.costToGetHere + this.maxWaitTime; } }



    public QPriority(QGameState node, BuildingGS targetGS) {
        this.currentNode = node;
        this.targetGS = targetGS;
        this.distanceRuler = LTPHelper.estematedRemainingDistance(node.gameState, targetGS);
    }

    public int CompareTo(object obj) {
        QPriority other = obj as QPriority;
        if (other == null) { return -1; } // this thing is smaller than a non QPriority obj

        /**
         * -- Most important reasons --
         * fewer infinities should win
         * access to more resources should win
         * shorter remaining distance should win
         * faster velocity should win
         * less resources spent should win
         * -- least important reasons --
         * 
         */

        if (this.numberOfInfinities != other.numberOfInfinities)
            { return this.numberOfInfinities - other.numberOfInfinities; }
        if (this.unaquirableResourceCount != other.unaquirableResourceCount)
            { return this.unaquirableResourceCount - other.unaquirableResourceCount; }
        if (this.estTotalDist != other.estTotalDist)
            { return this.estTotalDist - other.estTotalDist; }
        if (this.totalCPTDelta != other.totalCPTDelta)
            { return this.totalCPTDelta - other.totalCPTDelta; }

        // All things equal go for the cheaper option
        return this.totalResourcesSpent - other.totalResourcesSpent;

        /*
         * Examples:
         * 10 vs 7
         * 10 - 7  =  3  => 2nd entry goes first
         * 
         * 7  vs 10
         * 7  - 10 = -3  => 1st entry goes first
         */
    }
}
