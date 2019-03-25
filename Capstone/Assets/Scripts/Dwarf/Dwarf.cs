using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class Dwarf : MonoBehaviour {


    Tile currentTile;


    private static float MOVEMENT_SPEED = 0.2f; // How much time in seconds should the dwarf wait before moving to a new tile? 
    private static Tilemap tileMap;
    private static TileBase selfPicture;

    // Fields for moving a dwarf
    private Queue<Tile> path;
    private Tile targetTile;

    public static void staticInitalize(Tilemap tileMap, TileBase picture) {
        Dwarf.tileMap = tileMap;
        Dwarf.selfPicture = picture;
    }

    public void initalize(Tile startingTile) {
        currentTile = startingTile;
        drawSelf();
    }

    private void drawSelf() {
        // Redraw this dwarf at this.currentTile
        Vector3Int drawPos = new Vector3Int(this.currentTile.position.x, this.currentTile.position.y, GameSetup.CHARACTER_LAYER);
        tileMap.SetTile(drawPos, selfPicture);
    }

    #region Pathfinding
    
    public IEnumerator startPathing(Tile targetTile) {
        Debug.Log("Nocab flag Dwarf 2");
        updatePath(targetTile);
        yield return StartCoroutine(followPath());
    }

    private void updatePath(Tile targetTile) {
        AStar pathFinder = new AStar(currentTile, targetTile);
        this.path = pathFinder.getPath();
        this.targetTile = targetTile;
    }

    private IEnumerator followPath() {
        Debug.Log("Nocab flag dwarf 3");
        Debug.Log("Nocab dwarf pathCount: " + this.path.Count);
        while (this.path.Count > 0) {
            Debug.Log("Nocab flag dwarf 4");
            // Ask the mapcontroller if the next tile is still open
            if (MapController.singleton.moveDwarf(this, this.path.Peek())) {
                // If we're cleared to move
                this.changePosition(this.path.Dequeue());
                yield return new WaitForSeconds(MOVEMENT_SPEED);
            } else {
                // Else, we were denied clearence to move
                // Something must of changed as we were walking
                // Find a new path
                updatePath(targetTile);
            }

            yield return null;
        }
    }

    private void changePosition(Tile newPos) {
        // Draw yourself onto the map at the provided Tile

        // First erase your old position
        Vector3Int erasePos = new Vector3Int(this.currentTile.position.x, this.currentTile.position.y, GameSetup.CHARACTER_LAYER);
        tileMap.SetTile(erasePos, null);

        // Now we're clear to update our current tile
        this.currentTile = newPos;

        // Draw your new position
        drawSelf();
    }

    #endregion
    
    public IEnumerator assignWork(IBuilding building, Func<Dwarf, IBuilding, bool> callback) {
        Debug.Log("Dwarf nocab flag assingWork 1");
        Vector2Int buildingPos = building.position();
        yield return StartCoroutine(startPathing(new Tile(buildingPos.x, buildingPos.y)));

        // Once we're ready tell the GameController that we're ready for assignment
        callback(this, building);
        yield break;
    }

}
