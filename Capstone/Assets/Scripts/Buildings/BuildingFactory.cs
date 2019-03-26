using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public static class BuildingFactory {


    // TODO: Whenever a new building is added to this factory make sure to update this function
    public static HashSet<ResourceChange> costToBuild(BuildingType bt) {
        // TODO: In the future, buildings may cost differently depending on when they were built
        switch(bt) {
            case BuildingType.Bank: return bankBuildCost;
            case BuildingType.StoneMason: return stoneMasonBuildCost;
            case BuildingType.WoodCutter: return swoodCutterBuildCost;

            case BuildingType.NONE:
            default: throw new System.Exception("Unknown building type, can't return cost. Building Type: " + Enum.GetName(typeof(BuildingType), bt));
        }
    }

    // Builds a new Positionless building
    public static IBuilding buildNew(BuildingType bt) {
        switch (bt) {
            case BuildingType.Bank: return buildNewBank();
            case BuildingType.StoneMason: return buildNewStoneMason();
            case BuildingType.WoodCutter: return buildNewWoodCutter();

            case BuildingType.NONE:
            default: throw new System.Exception("Unknown building type, can't build. Building Type: " + Enum.GetName(typeof(BuildingType), bt));
        }
    }

    public static IBuilding buildNew(BuildingType bt, int x, int y) {
        switch (bt) {
            case BuildingType.Bank: return buildNewBank(x, y);
            case BuildingType.StoneMason: return buildNewStoneMason(x, y);
            case BuildingType.WoodCutter: return buildNewWoodCutter(x, y);

            case BuildingType.NONE:
            default: throw new System.Exception("Unknown building type, can't build. Building Type: " + Enum.GetName(typeof(BuildingType), bt));
        }
    }

    #region bank
    public static TileBase bankTile;  // All sprites are loaded from BuildFactoryMonobehavior.cs monobehavior object
    public static readonly int bankProdPerPop = 5;
    public static readonly int bankMaxPop = 3;
    public static readonly HashSet<ResourceChange> bankBuildCost = new HashSet<ResourceChange>() {
        new ResourceChange(ResourceType.Stone, 100),
        new ResourceChange(ResourceType.Gold, 10),
        new ResourceChange(ResourceType.Wood, 150)
    };
    public static IBuilding buildNewBank(int x, int y) {
        return new SimpleBuilding(
            BuildingType.Bank,
            ResourceType.Gold, 
            bankProdPerPop, 
            bankMaxPop,
            bankTile, 
            new Vector2Int(x, y),
            bankBuildCost);
    }
    public static IBuilding buildNewBank() {
        return new PositionlessSimpleBuilding(
            BuildingType.Bank,
            ResourceType.Gold,
            bankProdPerPop,
            bankMaxPop,
            bankTile,
            bankBuildCost);
    }

    #endregion

    #region stone mason
    public static TileBase stoneMasonTile;
    public static readonly int stoneMasonProdPerPop = 1;
    public static readonly int stoneMasonMaxPop = 6;
    public static readonly HashSet<ResourceChange> stoneMasonBuildCost = new HashSet<ResourceChange>()
    {
        new ResourceChange(ResourceType.Gold, 150),
        new ResourceChange(ResourceType.Wood, 50)
    };
    public static IBuilding buildNewStoneMason(int x, int y) {
        return new SimpleBuilding(
            BuildingType.StoneMason,
            ResourceType.Stone,
            stoneMasonProdPerPop,
            stoneMasonMaxPop,
            stoneMasonTile,
            new Vector2Int(x, y),
            stoneMasonBuildCost);
    }
    public static IBuilding buildNewStoneMason() {
        return new PositionlessSimpleBuilding(
            BuildingType.StoneMason,
            ResourceType.Stone,
            stoneMasonProdPerPop,
            stoneMasonMaxPop,
            stoneMasonTile,
            stoneMasonBuildCost);
    }
    #endregion

    #region wood cutter
    public static TileBase woodCutterTile;
    public static readonly int woodCutterProdPerPop = 3;
    public static readonly int woodCutterMaxPop = 2;
    public static readonly HashSet<ResourceChange> swoodCutterBuildCost = new HashSet<ResourceChange>() {
        new ResourceChange(ResourceType.Gold, 75)
    };
    public static IBuilding buildNewWoodCutter(int x, int y) {
        return new SimpleBuilding(
            BuildingType.WoodCutter,
            ResourceType.Wood, 
            woodCutterProdPerPop, 
            woodCutterMaxPop,
            woodCutterTile, 
            new Vector2Int(x, y),
            swoodCutterBuildCost);
    }
    public static IBuilding buildNewWoodCutter()
    {
        return new PositionlessSimpleBuilding(
            BuildingType.WoodCutter,
            ResourceType.Wood, 
            woodCutterProdPerPop, 
            woodCutterMaxPop,
            woodCutterTile, 
            swoodCutterBuildCost);
    }

    #endregion


    public static List<BuildingType> allBuildings = new List<BuildingType>(3) {
        BuildingType.Bank,
        BuildingType.StoneMason,
        BuildingType.WoodCutter,
    };

    public static string getName(BuildingType bt) {
        switch(bt) {
            case BuildingType.Bank:       return "Bank";
            case BuildingType.StoneMason: return "Stone Mason";
            case BuildingType.WoodCutter: return "Wood Cutter";
                
            case BuildingType.NONE: return "NONE";
            default: return Enum.GetName(typeof(BuildingType), bt);
        }
    }

}
