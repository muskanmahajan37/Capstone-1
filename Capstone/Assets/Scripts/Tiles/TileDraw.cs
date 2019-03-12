using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class TileDraw : MonoBehaviour {

    public Tilemap groundTileMap;
    public TileBase groundTileBase;
    public void drawGround(Tile t) {
        int posX = Mathf.RoundToInt(t.position.x);
        int posY = Mathf.RoundToInt(t.position.y);
        if (groundTileBase == null)
        {
            throw new SystemException();
        }
        groundTileMap.SetTile(new Vector3Int(posX, posY, 0), groundTileBase);
    }
    public void drawGround(IEnumerable<Tile> tiles) {
        foreach(Tile t in tiles) {
            drawGround(t);
        }
    }
    public void drawGround(Tile[,] tiles) {
        foreach(Tile t in tiles) {
            drawGround(t);
        }
    }



    public Tilemap objectTileMap;
    public TileBase rockTileBase;
    public void drawNewRock(Tile tile) {
        int posX = Mathf.RoundToInt(tile.position.x);
        int posY = Mathf.RoundToInt(tile.position.y);

        objectTileMap.SetTile(new Vector3Int(posX, posY, 0), rockTileBase);

    }
    public void drawNewRock(IEnumerable<Tile> tiles) {
        foreach(Tile t in tiles) {
            drawNewRock(t);
        }
    }


    public void drawOnTile(Tile tile, TileBase picture, Tilemap tilemap) {
        int posX = Mathf.RoundToInt(tile.position.x);
        int posY = Mathf.RoundToInt(tile.position.y);

        tilemap.SetTile(new Vector3Int(posX, posY, 0), picture);
    }
    public void drawOnTile(IEnumerable<Tile> tiles, TileBase picture, Tilemap tilemap) {
        foreach (Tile t in tiles) {
            drawOnTile(t, picture, tilemap);
        }
    }
}
