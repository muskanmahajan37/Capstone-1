using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    Sprite buildingIcon();
}


public class SimpleBuilding : IBuilding {
    /**
     * A simple building that has 
     *  - only one output resource
     *  - no input resources
     *  - no population cap
     * 
     */

    private readonly HashSet<ResourceChange> buildCost;

    private readonly ResourceType outputResource;
    private int currentPop;
    private int productionPerPop;

    private Sprite icon;


    public SimpleBuilding(ResourceType outputResource, int prodPerPop, Sprite icon, HashSet<ResourceChange> costToBuild) {
        this.outputResource = outputResource;
        this.buildCost = costToBuild;
        this.productionPerPop = prodPerPop;

        this.icon = icon;
    }
    

    public IBuilding simpleClone() {
        // TODO: Consider, is having every clone point to the same buildCost object ok? 
        return new SimpleBuilding(this.outputResource, this.productionPerPop, this.icon, this.buildCost);
    }

    public IBuilding deepClone() {
        SimpleBuilding result = new SimpleBuilding(this.outputResource, this.productionPerPop, this.icon, this.buildCost);
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


    public Sprite buildingIcon() {
        return this.icon;
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

