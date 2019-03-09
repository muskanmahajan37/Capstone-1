using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Center : MonoBehaviour {
    // Center the camera

    public void Start() {
        center();
    }

    public void center()
    {
        float centerX = GameSetup.BOARD_WIDTH / 2f;
        float centerY = GameSetup.BOARD_HEIGHT / 2f;

        this.transform.position = new Vector3(centerX, centerY, -100);
    }

}
