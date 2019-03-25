using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildFactoryMonobehavior : MonoBehaviour
{
    public TileBase bankSprite;
    public TileBase masonSprite;
    public TileBase woodCutterSprite;

    // Start is called before the first frame update
    void Start()
    {
        BuildingFactory.bankTile = this.bankSprite;
        BuildingFactory.stoneMasonTile = this.masonSprite;
        BuildingFactory.woodCutterTile = this.woodCutterSprite;
    }
}
