using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public interface IBuilding  {
    /**
     * A building is an object that always produces some resource per tick
     *      (IE: The output resource will always be positive and greater than 0)
     *      
     * A building may have the folowing qualities
     *  - incoming resources to start production 
     *      (IE output resource = 1 steel, will require incoming resource = (1 iron + 2 coal) )
     *  - a worker cap
     *      IE, how many workers can work in this building
     *      No worker cap, or 0 >= cap implies infinite workers can work in this building
     */

    
    // The type and amount of resources it takes to build this building
    // NOTE: This should always be a positive value
    // IE ResourceChange.value > 0
    // if ResourceChange.value < 0 => you get the resource when the building is initially built
    HashSet<ResourceChange> costToBuild();

    HashSet<ResourceType> outputResources();

    HashSet<ResourceType> inputResources();


    // This will return ALL the resources that will be changed by this building
    // over the given time perioud
    // The outputResources and input resources will be accounted for in the result
    HashSet<ResourceChange> simulate(int time);

    // How much does this building change the resources per tick?
    // Think of the results of this function like asking for the buildings ResourcePerTick values for 
    //   all the related resources to this building.
    HashSet<ResourceChange> changePerTick();

    // Clones this building, but the new building has no population
    IBuilding simpleClone();

    // Clones this building, but the new building has the same population as this building
    IBuilding deepClone();



    // Accessors
    TileBase buildingIcon();
    Vector2Int position();
    BuildingType getBuildingType();
    
    int currentWorkers();
    int openWorkerSlots();

    // Modifiers
    bool addWorker();
    bool removeWorker();
    
    // TODO: Claimed spots vs actually filled spots
}


public class SimpleBuilding : IBuilding {
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
    private Vector2Int pos;

    #region Constructors

    // TODO: Clean this up. Maybe have a building factory class? 
    public SimpleBuilding(
        BuildingType bt,
        ResourceType outputResource, 
        int prodPerPop, 
        int maxPop,
        TileBase icon, 
        Vector2Int position,
        HashSet<ResourceChange> costToBuild)
    {
        this.bt = bt;
        this.outputResource = outputResource;
        this.buildCost = costToBuild;

        this.productionPerPop = prodPerPop;
        this.maxPop = maxPop;

        this.icon = icon;
        this.pos = position;
    }
    
    private SimpleBuilding clone() {        
        // TODO: Consider, is having every clone point to the same buildCost object ok? 
        return new SimpleBuilding(
            this.bt,
            this.outputResource,
            this.productionPerPop,
            this.maxPop,
            this.icon,
            new Vector2Int(pos.x, pos.y),
            this.buildCost);
    }
    
    public IBuilding simpleClone() {
        return this.clone();
    }
    
    public IBuilding deepClone() {
        SimpleBuilding result = this.clone();
        result.currentPop = this.currentPop;
        return result;
    }

    #endregion

    public HashSet<ResourceChange> costToBuild() {
        return this.buildCost;
    }

    public HashSet<ResourceType> inputResources() {
        // SimpleBuildings have no input resources
        return new HashSet<ResourceType>();
    }

    // TODO: Does this really need to be a clone? probably... 
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
    public Vector2Int position() { return this.pos; }
    public BuildingType getBuildingType() { return this.bt; }
    public int currentWorkers() { return this.currentPop; }
    public int openWorkerSlots() { return this.maxPop - this.currentPop; }

    #endregion

    public bool addWorker() {
        if (this.maxPop <= this.currentPop) { return false; }
        this.currentPop++;
        return true;
    }

    public bool removeWorker()
    {
        if (this.currentPop <= 0) { return false; }
        this.currentPop--;
        return true;
    }
}

// TODO: Put this somewhere else
public class ResourceChange {
    /**
     * A simple container that describes a target resource and how it should change (up or down)
     */

    public ResourceType resourceType;
    public int change;

    public ResourceChange(ResourceType rt, int change) {
        this.resourceType = rt;
        this.change = change;
    }
}

