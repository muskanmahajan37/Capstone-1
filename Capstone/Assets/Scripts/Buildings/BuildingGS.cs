using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// TODO: Impliment a GameState object pooler
public class BuildingGS : MonoBehaviour
{
    // Keeping track of what type of buildings exist is important
    // for when we want to redistribute workers and- or destroy buildings
    protected HashSet<IBuilding> buildings;  // All the buildings this game state has
    protected Dictionary<ResourceType, int> resourceStockpile;

    protected Dictionary<ResourceType, int> resourceChangePerTick;



    public BuildingGS() {
        this.buildings = new HashSet<IBuilding>();
        this.resourceStockpile = new Dictionary<ResourceType, int>();
        this.resourceChangePerTick = new Dictionary<ResourceType, int>();
    }

    // Copy constructor
    public BuildingGS(BuildingGS other) {
        // NOTE: This will deepClone all the buildings in the provided other (IE: The populations will be coppied)

        this.buildings = new HashSet<IBuilding>();
        this.resourceStockpile = new Dictionary<ResourceType, int>();

        foreach(IBuilding otherBuild in other.buildings) {
            IBuilding buildingClone = otherBuild.deepClone();
            this.buildings.Add(buildingClone);
        }

        // TODO: make this work for cloning this.resourceChangePerTick;
        foreach(KeyValuePair<ResourceType, int> kvp in other.resourceStockpile) {
            this.resourceStockpile.Add(kvp.Key, kvp.Value);
        }
    }

    #region accessors

    public int getStockpile(ResourceType rt) {
        if (this.resourceStockpile.ContainsKey(rt)) { return this.resourceStockpile[rt]; }
        return 0;
    }
    
    public int getPerTickChange(ResourceType rt) {
        if (this.resourceChangePerTick.ContainsKey(rt)) { return this.resourceChangePerTick[rt]; }
        return 0;
    }

    public IEnumerable<ResourceType> getResourceType() {
        return this.resourceStockpile.Keys;
    }

    #endregion

    public void timePasses(int time) {
        // Updates this game states resource stockpiles based on the buildings present
        // NOTE: this uses the resourceChangePerTick dictionary, which is updated at building addition time
        //       so this is not a direct update based on the buildings, but indirectly based on the buildings
        //       This is done in the name of speed. A bug may mean that the buildings are not in line with
        //       this.resourceChangePerTick field
        
        foreach(KeyValuePair<ResourceType, int> kvp in this.resourceChangePerTick) {
            this.resourceStockpile[kvp.Key] += kvp.Value * time;
        }
    }

    public void forceAddBuilding(IBuilding newBuilding) {
        // Add the provided building to the collection
        // AND updates this game state's resourceChangePerTick accordingly

        this.buildings.Add(newBuilding);
        
        foreach (ResourceChange rc in newBuilding.changePerTick()) {
            this.resourceChangePerTick[rc.resourceType] += rc.change;
        }
    }

    public void buyBuilding(IBuilding newBuilding) {
        // Note this function does no validation checking.
        // IE: going negative is ok, so a check must be made before calling this function

        // First, change the stockpiles accordingly by subtracting the cost of the building
        foreach (ResourceChange rc in newBuilding.costToBuild()) {
            // NOTE: the cost to build change value is always positive
            this.resourceStockpile[rc.resourceType] -= rc.change;
        }

        // Then add the building and it's resource per tick count to this game state
        this.forceAddBuilding(newBuilding);

    }


    public override int GetHashCode() {
        // Two game States are equal if they have:
        // - the same stockpiles
        // - the same income per tick

        // TODO: Consider number of buildings an important field
        // TODO: Consider adding "free-land remaining" to this hash
        int hash = 17;
        foreach (KeyValuePair<ResourceType, int> kvp in this.resourceStockpile) {
            int resourceChangePerTick = this.resourceChangePerTick[kvp.Key];
            hash = (hash * 23) + Convert.ToInt32(kvp.Key);
            hash = (hash * 23) + kvp.Value;
            hash = (hash * 23) + resourceChangePerTick;
        }
        return hash;
    }

    public override bool Equals(object obj)
    {
        /**
         * Two Game States are equal if they have 
         * - same resource stockpile
         * - same resource income per tick
         */

        // TODO: Consider number of buildings an important field? Maybe how much free land is remaining? 

        BuildingGS otherGS = obj as BuildingGS;
        if (otherGS == null) { return false; }

        // Different number of fields => fail
        if (this.resourceStockpile.Count != otherGS.resourceChangePerTick.Count ||
            this.resourceChangePerTick.Count != otherGS.resourceChangePerTick.Count)
        {
            return false;
        }

        // Check type of resources
        int otherResourceCount;
        foreach (KeyValuePair<ResourceType, int> kvp in this.resourceStockpile) {
            if ( ! otherGS.resourceStockpile.TryGetValue(kvp.Key, out otherResourceCount)) {
                // Try to access a given key in the other dictionary
                // If it fails, return false
                return false;
            }
            if (kvp.Value != otherResourceCount) {
                // If the key is contained, but the stockpile count is different
                return false;
            }
            
            if (this.resourceChangePerTick[kvp.Key] != otherGS.resourceChangePerTick[kvp.Key]) {
                // If the change per tick is different
                return false;
            }

        }
        return true;
    }

}
