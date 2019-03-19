using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingFactory
{

    public static Sprite bankSprite;  // All sprites are loaded from BuildFactoryMonobehavior.cs monobehavior object
    public static readonly int bankProdPerPop = 5;
    public static readonly HashSet<ResourceChange> bankBuildCost = new HashSet<ResourceChange>() {
        new ResourceChange(ResourceType.Stone, 100),
        new ResourceChange(ResourceType.Gold, 10),
        new ResourceChange(ResourceType.Wood, 150)
    };

    public static IBuilding buildNewBank() {
        return new SimpleBuilding(ResourceType.Gold, bankProdPerPop, bankSprite, bankBuildCost);
    }



    public static Sprite stoneMasonSprite;
    public static readonly int stoneMasonProdPerPop = 1;
    public static readonly HashSet<ResourceChange> stoneMasonBuildCost = new HashSet<ResourceChange>()
    {
        new ResourceChange(ResourceType.Gold, 150),
        new ResourceChange(ResourceType.Wood, 50)
    };
    public static IBuilding buildNewStoneMason() {
        return new SimpleBuilding(ResourceType.Stone, stoneMasonProdPerPop, stoneMasonSprite, stoneMasonBuildCost);
    }


    public static Sprite woodCutterSprite;
    public static readonly int woodCutterProdPerPop = 3;
    public static readonly HashSet<ResourceChange> swoodCutterBuildCost = new HashSet<ResourceChange>() {
        new ResourceChange(ResourceType.Gold, 75)
    };

    public static IBuilding buildNewWoodCutter() {
        return new SimpleBuilding(ResourceType.Wood, woodCutterProdPerPop, woodCutterSprite, swoodCutterBuildCost);
    }


    public static 

}

// TODO: Make this work
public enum BuildingTypes
{
    Bank,
    WoodCutter,
    StoneMason,

    NONE
}