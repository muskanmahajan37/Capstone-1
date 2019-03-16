using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartGameScript : MonoBehaviour
{
    private static readonly int workerCostGold = 100;
    public static readonly int WAIT_TIME = 10;

    private long gameTime = 0;
    private long fps = 60;
    private bool gameStarted = false;

    public Button startGameBtn, increaseWorkerBtn;

    private List<RoundedResource> playerResources = new List<RoundedResource>();
    private RoundedResource playerGold;

    private List<RoundedResource> resourcesAI = new List<RoundedResource>();
    private RoundedResource goldAI;
    private Queue<Work> workQAI;
    private int currentIndex;




    // Start is called before the first frame update
    void Start()
    {
        startGameBtn.onClick.AddListener(startGame);
        increaseWorkerBtn.onClick.AddListener(addWorker);
        playerResources.Add(new RoundedResource("gold", 5, 0, 5, 2));
        playerGold = playerResources[0];
        resourcesAI.Add(new RoundedResource("gold", 5, 0, 5, 2));
        goldAI = resourcesAI[0];
    }

    private void addWorker()
    {
        if (playerGold.resourceCount > workerCostGold)
        {
            playerGold.addWorkers(1);
            playerGold.resourceCount -= workerCostGold;
            Debug.Log("Player's gold workers increased by 1 to: " + playerGold.workerCount);
        }
        else
        {
            Debug.Log("Not enough resources to increase worker count.");
        }
    }

    private void startGame()
    {
        gameStarted = true;
        LongTermPlanning ltp = new LongTermPlanning();


        GameState initialGS = new GameState();
        //Resource goldAI = new RoundedResource("gold", 5, 100, 2, 2);
        initialGS.addResource(new RoundedResource("gold", 5, 0, 5, 2));


        GameState targetGS = new GameState();
        Resource targetGold = new RoundedResource("gold", 5, 5000000, 5, 2);
        targetGS.addResource(targetGold);

        workQAI = ltp.plan(initialGS, targetGS);
        currentIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            gameTime++;
            if (gameTime % fps == 0)
            {
                playerGold.update(1);
                goldAI.update(1);
            }
            if (gameTime % (fps * 5) == 0)
            {
                Debug.Log("golds: " + playerGold.resourceCount + " , " + goldAI.resourceCount);
            }
            if (gameTime % (fps * WAIT_TIME) == 0)
            {
                Work currentWork = workQAI.Dequeue();
                if (currentWork == Work.EMPTY)
                { }
                else if (currentWork == Work.NewGoldMiner)
                {
                    if (playerGold.resourceCount > workerCostGold)
                    {
                        goldAI.addWorkers(1);
                        goldAI.resourceCount -= workerCostGold;
                        Debug.Log("AI's gold workers increased by 1 to: " + goldAI.workerCount);
                    }

                }
                else if (currentWork == Work.Wait)
                { }
            }
        }
    }
}
