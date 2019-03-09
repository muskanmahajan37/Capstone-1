using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMove : MonoBehaviour {

    public float speed;
    private float scaledSpeed;

    private void Start() {
        this.scaledSpeed = speed / 100;
    }

    // Update is called once per frame
    void Update() {
        Vector2 delta = new Vector2(
            Input.GetAxis("Horizontal") * scaledSpeed,
            Input.GetAxis("Vertical") * scaledSpeed);

        this.transform.Translate(delta);
    }
}
