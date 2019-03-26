using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PositionlessSimpleBuilding : IBuilding
{
    /**
     * A simple building that has 
     *  - only one output resource
     *  - no input resources
     *  - no population cap
     * 
     */

    private readonly BuildingType bt;

    private readonly HashSet<ResourceChange> buildCost;

    private readonly ResourceType outputResource;
    private int currentPop;
    private int maxPop;
    private int productionPerPop;

    private TileBase icon;

    // TODO: Clean this up. Maybe have a building factory class? 
    public PositionlessSimpleBuilding(
        BuildingType bt,
        ResourceType outputResource, 
        int prodPerPop, 
        int maxPop,
        TileBase icon, 
        HashSet<ResourceChange> costToBuild)
    {
        this.bt = bt;
        this.outputResource = outputResource;
        this.buildCost = costToBuild;

        this.productionPerPop = prodPerPop;
        this.maxPop = maxPop;

        this.icon = icon;
    }

    private PositionlessSimpleBuilding clone() {
        // TODO: Consider, is having every clone point to the same buildCost object ok? 
        return new PositionlessSimpleBuilding(
            this.bt,
            this.outputResource,
            this.productionPerPop,
            this.maxPop,
            this.icon,
            this.buildCost);
    }

    public IBuilding simpleClone() {
        return this.clone();
    }

    public IBuilding deepClone() {
        PositionlessSimpleBuilding result = this.clone();
        result.currentPop = this.currentPop;
        return result;
    }


    public HashSet<ResourceChange> costToBuild() {
        return this.buildCost;
    }

    public HashSet<ResourceType> inputResources() {
        // SimpleBuildings have no input resources
        return new HashSet<ResourceType>();
    }

    public HashSet<ResourceType> outputResources() {
        return new HashSet<ResourceType>() { outputResource };
    }

    public HashSet<ResourceChange> simulate(int time) {
        // NOTE: SimpleBuildings only have an outputResource and no input resources
        int change = this.currentPop * time * productionPerPop;
        return new HashSet<ResourceChange>() { new ResourceChange(this.outputResource, change) };

    }

    public HashSet<ResourceChange> changePerTick() {
        int change = this.currentPop * productionPerPop;
        return new HashSet<ResourceChange>() { new ResourceChange(this.outputResource, change) };
    }

    #region Accessors
    public TileBase buildingIcon() { return this.icon; }
    public Vector2Int position() { throw new System.Exception("You're trying to get the position of a PositionlessSimpleBuilding"); }
    public BuildingType getBuildingType() { return this.bt; }
    public int currentWorkers() { return this.currentPop; }
    public int openWorkerSlots() { return this.maxPop - this.currentPop; }

    #endregion;

    public bool addWorker() {
        if (this.maxPop <= this.currentPop) { return false; }
        this.currentPop++;
        return true;
    }

    public bool removeWorker() {
        if (this.currentPop <= 0) { return false; }
        this.currentPop--;
        return true;
    }
}
