using System;
using System.Collections.Generic;
using UnityEngine;

public class StartGameScript : MonoBehaviour
{
    private System.Windows.Forms.Timer gameTimer;
    private List<RoundedResource> playerResources = new List<RoundedResource>();
    private long gameTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        gameTimer = new System.Windows.Forms.Timer();
        gameTimer.Tick += new EventHandler(gameTimerTicker);
        gameTimer.Interval = 1000;
        playerResources.Add(new RoundedResource("gold", 5, 100, 2, 2));
        gameTimer.Start();

    }

    // Update is called once per frame
    void gameTimerTicker(object sender, EventArgs e)
    {
        gameTime++;
        RoundedResource gold = playerResources[0];

        Debug.Log("gold: " + gameTime);


    }
}
