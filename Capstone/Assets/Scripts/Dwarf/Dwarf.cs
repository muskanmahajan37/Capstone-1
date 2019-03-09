using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dwarf : MonoBehaviour
{
    Tile currentTile;

    public Tilemap tileMap;
    public TileBase selfPicture;

    // Fields for moving a dwarf
    private Queue<Tile> path;
    private Tile targetTile;


    // Start is called before the first frame update
    void Start() {
        currentTile = new Tile(5, 7);
        drawSelf();
    }

    private void drawSelf() {
        // Redraw this dwarf at this.currentTile
        Vector3Int drawPos = new Vector3Int(this.currentTile.position.x, this.currentTile.position.y, GameSetup.CHARACTER_LAYER);
        tileMap.SetTile(drawPos, selfPicture);
    }


    public void startPathingTest() {
        this.startPathing(new Tile(20, 16));
    }
    private void startPathing(Tile targetTile) {
        AStar pathFinder = new AStar(currentTile, targetTile);
        this.path = pathFinder.getPath();
        this.targetTile = targetTile;
        InvokeRepeating("followPath", 0, GameSetup.TICK_RATE);
    }

    private void followPath() {
        // Move my position one down my current path
        if (this.path.Count == 0) {
            // If we've arrived at the end of the path
            CancelInvoke("followPath");
            return;
        }

        // Ask the mapcontroller if the next tile is still open
        if ( MapController.singleton.moveDwarf(this, this.path.Peek()) ) {
            // If we're cleared to move
            this.changePosition(this.path.Dequeue());
        } else {
            // Else, we were denied clearence to move
            // Something must of changed as we were walking
            // Find a new path
            startPathing(targetTile);
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

}
