using System;

// TODO: Make this an actual object
public enum Work {
    /**
     * To represent the different orders that the LongTermPlanner can return
     */

    // The one true wait
    Wait,

    // Worker assignment
    BuyWorkerBank,
    BuyWorkerStoneMason,
    BuyWorkerWoodCutter,

    BuyWorkerSilverMine,
    BuyWorkerSteelSmith,
    
    // Building construction
    BuyBuildingBank,
    BuyBuildingStoneMason,
    BuyBuildingWoodCutter,

    BuyBuildingSilverMine,
    BuyBuildingSteelSmith,

    // Depreciated:
    NewGoldMiner,
    NewStoneMiner,
    NewWoodsman,

    EMPTY // To represent a "null" value of work which is different than Wait
}

public static class WorkHelper {
    public static Work assignWorkerTo(BuildingType bt) {
        switch(bt)
        {
            case BuildingType.Bank:       return Work.BuyWorkerBank;
            case BuildingType.StoneMason: return Work.BuyWorkerStoneMason;
            case BuildingType.WoodCutter: return Work.BuyWorkerWoodCutter;

            case BuildingType.SilverMine: return Work.BuyWorkerSilverMine;
            case BuildingType.SteelSmith: return Work.BuyWorkerSteelSmith;

            case BuildingType.NONE:
            default:
                throw new System.Exception("Unknown building type, can't convert to Work enum. BT: " + Enum.GetName(typeof(BuildingType), bt));
        }
    }


    public static Work buildBuilding(IBuilding newBuilding) {
        switch(newBuilding.getBuildingType()) {
            case BuildingType.Bank:       return Work.BuyBuildingBank;
            case BuildingType.StoneMason: return Work.BuyBuildingStoneMason;
            case BuildingType.WoodCutter: return Work.BuyBuildingWoodCutter;

            case BuildingType.SilverMine: return Work.BuyBuildingSilverMine;
            case BuildingType.SteelSmith: return Work.BuyBuildingSteelSmith;

            case BuildingType.NONE:
            default:
                throw new System.Exception("Unknown building type, can't convert to Work enum. BT: " + 
                                           Enum.GetName(typeof(BuildingType), newBuilding.getBuildingType()));
        }
    }


}