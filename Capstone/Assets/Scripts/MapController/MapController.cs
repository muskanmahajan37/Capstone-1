using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PriorityQueueDemo;

public class MapController : MonoBehaviour {
    // This is the interface through which all map related updates happen

    #region Fields
    // The refrence to this controller object
    // NOTE: This pattern only works if a MapController MonoBehavior is added to the scene
    public static MapController singleton;

    // TODO: Should these be static? 
    public static Tile[,] allTiles;  // allTiles[x, y] is how it's accessed

    // A mapping of all the edges coming out of a given tile
    private Dictionary<Tile, HashSet<TileEdge>> outgoingEdges;
    
    // Given a key (Tile object in network), the value (set of Tile object) is all the Tiles that point to the key
    // Use this in tandom with outgoingEdges to find all incoming edges of a provided Tile
    // Mostly used when making a tile un-walkable or when needed to modify incoming edges of a tile
    private Dictionary<Tile, HashSet<Tile>> incomingTiles;



    public TileDraw tileDraw;

    #endregion

    #region Constructors/ Internal Utility

    private void Awake() {
        buildSingleton();
    }

    private void Start() {
        TileBuilderWrapper tbw = TileGeneration.buildTiles();
        allTiles = tbw.allTiles;
        outgoingEdges = tbw.outgoingEdges;
        incomingTiles = tbw.incomingTiles;

        // We only need to draw the ground once
        Debug.Log("Drawing tiles");
        this.tileDraw.drawGround(allTiles);
    }

    private void buildSingleton() {
        // This method will ensure that MapController.singleton is populated
        if (singleton == null) { singleton = this; }
        else if (singleton.Equals(this)) { return; }
        else {
            // Else the singleton already exist AND this boject isn't it
            // There can only be one...
            GameObject.Destroy(this);
        }
    }
    #endregion
    
    #region Map Changes

    public bool moveDwarf(Dwarf d, Tile t) {
        // The dwarf is requesting to move into the provided tile
        // It's up to us to see if it's a valid tile to stand in
        // or if the dwarf has permission to stand there
        // The dwarf will draw itself and erase its old image

        // NOTE: This doesn't care where the dwarf is coming from
        // It's assumed the dwarf's current position is valid

        // TODO: We can add a lot of complexity to this method
        // TODO: For example, make only one dwarf allowed to stand in a position at a time
        //       For now, many dwarfs to occupying the same tile is valid

        // Check walkability of tile (The provided tile may be out of date or fake)
        return allTiles[t.position.x, t.position.y].isWalkable;
    }

    public void addObstacleTest()
    {

        addObstacle(7, 6);
        addObstacle(7, 5);
        addObstacle(7, 7);
        addObstacle(7, 8);
        addObstacle(7, 9);

        addObstacle(6, 9);
        addObstacle(5, 9);
        addObstacle(4, 9);
        addObstacle(3, 9);

        drawNewRock(new Tile(new Vector2Int(7, 5)));
        drawNewRock(new Tile(new Vector2Int(7, 6)));
        drawNewRock(new Tile(new Vector2Int(7, 7)));
        drawNewRock(new Tile(new Vector2Int(7, 8)));
        drawNewRock(new Tile(new Vector2Int(7, 9)));

        drawNewRock(new Tile(new Vector2Int(6, 9)));
        drawNewRock(new Tile(new Vector2Int(5, 9)));
        drawNewRock(new Tile(new Vector2Int(4, 9)));
        drawNewRock(new Tile(new Vector2Int(3, 9)));
    }

    public void addObstacle(int x, int y) {
        // Add a point to the map that is unwalkable
        Tile targetTile = allTiles[x, y];
        targetTile.isWalkable = false;

        // Cut off all edges pointing into targetTile
        cutAllIncomingEdges(targetTile);
    }
    #endregion

    #region Direct Edge Modifiers

    #region cutting edges
    public int cutAllIncomingEdges(Tile tile) {
        // Cut all edges that point into the provided tile
        // Returns the number of cut edges

        int result = 0;
        foreach (Tile neighbor in incomingTiles[tile]) {
            // Remove all edges from neighbot -> end
            result += cutEdgeOneWay(neighbor, tile);
        }
        return result;
    }

    public int cutEdgeOneWay(Tile start, Tile end) {
        // Cuts the first (and hopefully only) edge from start -> end
        // Returns the number of edges that were cut (hopefully only 1 or 0)
        HashSet<TileEdge> outEdges = outgoingEdges[start];
        int result = outEdges.RemoveWhere((TileEdge e) => { return e.tile.Equals(end); });
        outgoingEdges[start] = outEdges;
        return result;
    }
    
    public int cutEdgeTwoWay(Tile t1, Tile t2) {
        return cutEdgeOneWay(t1, t2) + cutEdgeOneWay(t2, t1);
    }
    #endregion

    #region adding edges
    //TODO: all of this
    #endregion

    #endregion

    #region Edge Accessors (Neighbors)

    public HashSet<TileEdge> getNeighborEdges(Tile t) {
        return this.outgoingEdges[t];
    }

    public HashSet<Tile> getNeighbors(Tile t) {
        // A "neighbor" is anything the provided tile can directly see
        // Returns all the outgoing edges of the provided tile
        HashSet<Tile> result = new HashSet<Tile>();

        foreach (TileEdge e in outgoingEdges[t]) {
            result.Add(e.tile);
        }

        return result;
    }

    #endregion

    #region Node Accessors (Tiles)

    public Tile getTile(int x, int y) {
        return allTiles[x, y];
    }

    public Tile getTile(Tile t) {
        return getTile(t.position.x, t.position.y);
    }

    #endregion

    #region drawing


    public void drawNewRock(Tile t) {
        tileDraw.drawNewRock(t);
    }
    #endregion
}
