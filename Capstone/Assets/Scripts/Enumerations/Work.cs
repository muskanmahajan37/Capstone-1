using System;

// TODO: Make this an actual object
public enum Work {
    /**
     * To represent the different orders that the LongTermPlanner can return
     */

    Wait,
    BuyWorkerBank,
    BuyWorkerStoneMason,
    BuyWorkerWoodCutter,

    EMPTY // To represent a "null" value of work which is different than Wait
}

public static class WorkHelper {
    public static Work assignWorkerTo(BuildingType bt) {
        switch(bt)
        {
            case BuildingType.Bank:       return Work.BuyWorkerBank;
            case BuildingType.StoneMason: return Work.BuyWorkerStoneMason;
            case BuildingType.WoodCutter: return Work.BuyWorkerWoodCutter;
                
            case BuildingType.NONE:
            default:
                throw new System.Exception("Unknown building type, can't convert to Work enum. BT: " + Enum.GetName(typeof(BuildingType), bt));
        }


    }

}