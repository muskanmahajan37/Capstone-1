using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildFactoryMonobehavior : MonoBehaviour
{
    public Sprite bankSprite;
    public Sprite masonSprite;
    public Sprite woodCutterSprite;

    // Start is called before the first frame update
    void Start()
    {
        BuildingFactory.bankSprite = this.bankSprite;
        BuildingFactory.stoneMasonSprite = this.masonSprite;
        BuildingFactory.woodCutterSprite = this.woodCutterSprite;
    }
}
