using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MultiResourceBuilding : IBuilding {
    /**
     * To represent a building that takes in or ouputs multiple types of resources
     */
    
    private readonly BuildingType bt;    
    private Vector2Int pos;
    private int currentWorkerCount;

    private BuildingBlueprint myBlueprint { get { return BuildingFactory.allBluePrints[this.bt]; } }
    private int maxWorkerCount { get { return myBlueprint.maxPop; } }
    private List<IResourceProducer> outputProduction { get { return myBlueprint.outputResourceProduction; } }
    private List<IResourceProducer> inputCosts { get { return myBlueprint.inputResourceCosts; } }

    #region Constructors

    public MultiResourceBuilding( BuildingType bt, Vector2Int position ) {
        this.bt = bt;
        this.pos = position;
    }
    
    private MultiResourceBuilding clone() {
        return new MultiResourceBuilding( this.bt, new Vector2Int(pos.x, pos.y) );
    }

    public IBuilding simpleClone() {
        return this.clone();
    }

    public IBuilding deepClone() {
        MultiResourceBuilding result = this.clone();
        result.currentWorkerCount = this.currentWorkerCount;
        return result;
    }

    #endregion


    public List<ResourceChange> costToBuild() {
        return BuildingFactory.allBluePrints[this.bt].buildCost;
    }

    public List<ResourceType> outputResources() {
        List<ResourceType> result = new List<ResourceType>();
        foreach (IResourceProducer producer in outputProduction) {
            result.Add(producer.targetResource());
        }
        return result;
    }
    public List<ResourceChange> outputResourceProduction() {
        // What exactly does this specific building produce in a single tick? 
        List<ResourceChange> result = new List<ResourceChange>();
        foreach(IResourceProducer producer in outputProduction) {
            result.Add(producer.simulate(this.currentWorkerCount));
        }
        return result;
    }

    public List<ResourceType> inputResources() {
        List<ResourceType> result = new List<ResourceType>();
        foreach (IResourceProducer costs in inputCosts) {
            result.Add(costs.targetResource());
        }
        return result;
    }
    public List<ResourceChange> inputResourceCost() {
        // What exactly does this specific building eat up in a single tick? 
        // NOTE: The result value should alwasy be positive despite intuition saying otherwise
        List<ResourceChange> result = new List<ResourceChange>();
        foreach (IResourceProducer producer in inputCosts) {
            result.Add(producer.simulate(this.currentWorkerCount));
        }
        return result;
    }

    public List<ResourceChange> simulate(int time) {
        // NOTE: normally input resources are represented as a positive value, but here they are returned negative

        List<ResourceChange> result = this.changePerTick();
        foreach (ResourceChange rc in result) {
            // NOTE: This implimentation of IBuilding class does not have "building momentum"
            // IE: just because a building has been "alive" for longer does NOT mean it's efficency increases.
            // This allows us to simply multiply the time by the producer.simulate(...) func 
            rc.change *= time;
        }
        return result;
    }


    public List<ResourceChange> changePerTick() {
        // NOTE: normally input resources are represented as a positive value, but here they are returned negative

        List<ResourceChange> result = new List<ResourceChange>();
        // Calculate outputs
        foreach (IResourceProducer producer in outputProduction) {
            ResourceChange prodSimulate = producer.simulate(this.currentWorkerCount);
            result.Add(prodSimulate);
        }

        // Calculate inputs
        foreach (IResourceProducer costs in inputCosts) {
            // Remember, Blueprint.inputResourceCosts should always be positive despite intuition
            ResourceChange costSimulate = costs.simulate(this.currentWorkerCount);
            costSimulate.change = -1 * costSimulate.change;
            result.Add(costSimulate);
        }
        return result;
    }


    public TileBase buildingIcon() { return BuildingFactory.getIcon(this.bt); }
    public Vector2Int position() { return this.pos; }
    public BuildingType getBuildingType() { return this.bt; }
    public int currentWorkers() { return this.currentWorkerCount; }
    public int openWorkerSlots() { return this.maxWorkerCount - this.currentWorkerCount; }


    public bool addWorker() {
        if (this.maxWorkerCount <= this.currentWorkerCount) { return false; }
        this.currentWorkerCount++;
        return true;
    }
    
    public bool removeWorker() {
        if (this.currentWorkerCount <= 0) { return false; }
        this.currentWorkerCount--;
        return true;
    }
}
