using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartGameScript : MonoBehaviour
{
    private static readonly long fps = 60;
    private readonly long ACTION_INTERVAL = fps * LongTermPlanning.WAIT_TIME;
    private readonly long LOG_INTERVAL = fps * 5;

    private static long gameTime = 0;
    private bool gameStarted = false;

    public Button startGameBtn;
    public Button addWorkerGold, addWorkerStone, addWorkerWood;

    private readonly int noOfActors = 2;

    private GameState[] gameStates;
    private static int indexAI = 0;
    private static int indexPlayer = 1;

    private long[] workerBuildTimes;
    private String[] workerBuildType;

    private Queue<Work> workQAI;

    public Text mainTextDisplay;
    public Text otherTextDisplay;

    // Start is called before the first frame update
    void Start()
    {
        startGameBtn.onClick.AddListener(startGame);
        addWorkerGold.onClick.AddListener(delegate { addWorker("gold"); });
        addWorkerStone.onClick.AddListener(delegate { addWorker("stone"); });
        addWorkerWood.onClick.AddListener(delegate { addWorker("wood"); });
    }

    private void startGame()
    {
        // TODO: Disable/remove start game button here
        gameStarted = true;

        // TODO: Allow player to input initial game conditions
        GameState initialGS = new GameState();
        Resource gold = new RoundedResource(name: "gold", perWorkerTick: 5, resourceCount: 100, workerCount: 0, percision: 2);
        Resource stone = new RoundedResource("stone", 5, 100, 0, 2);
        Resource wood = new RoundedResource("wood", 5, 100, 0, 2);
        initialGS.addResource(gold);
        initialGS.addResource(stone);
        initialGS.addResource(wood);


        GameState targetGS = new GameState();
        Resource targetGold = new RoundedResource("gold", 5, 60000, 5, 2);
        Resource targetStone = new RoundedResource("stone", 5, 60000, 5, 2);
        Resource targetWood = new RoundedResource("wood", 5, 60000, 5, 2);
        targetGS.addResource(targetGold);
        targetGS.addResource(targetStone);
        targetGS.addResource(targetWood);

        // TODO: Show loading game progress bar
        workQAI = LongTermPlanning.plan(initialGS, targetGS);

        // Setup actors for new game
        gameStates = new GameState[noOfActors];
        workerBuildType = new String[noOfActors];
        workerBuildTimes = new long[noOfActors];

        for (int i = 0; i < noOfActors; i++)
        {
            GameState gs = new GameState();
            gs.addResource(new RoundedResource(name: "gold", perWorkerTick: 5, resourceCount: 100, workerCount: 0, percision: 2));
            gs.addResource(new RoundedResource("stone", 5, 100, 0, 2));
            gs.addResource(new RoundedResource("wood", 5, 100, 0, 2));
            gameStates[i] = gs;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            gameTime++;
            if (gameTime % fps == 0)
            {
                for (int i = 0; i < noOfActors; i++)
                {
                    GameState gameState = gameStates[i];
                    gameState.timePasses(1);
                    if(workerBuildTimes[i] == gameTime)
                    {
                        buildWorker(gameState, workerBuildType[i]);
                    }
                }
            }
            if (gameTime % LOG_INTERVAL == 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Current in game time: ");
                sb.Append(gameTime);
                sb.Append('\n');
                sb.Append("Actor game states: \n");
                //Debug.Log("Actor game states:");
                foreach(GameState gs in gameStates) {
                    sb.Append(gs);
                    sb.Append('\n');
                    //Debug.Log(gs);
                }
                mainTextDisplay.text = sb.ToString();
            }
            if (gameTime % ACTION_INTERVAL == 0)
            {
                Work currentWork = workQAI.Dequeue();
                if (currentWork == Work.EMPTY)
                { }
                else if (currentWork == Work.NewGoldMiner)
                {
                    if (canBuildWorker(gameStates[indexAI], workerBuildTimes[indexAI])) {
                        workerBuildTimes[indexAI] = gameTime + LongTermPlanning.workerBuildTime;
                        workerBuildType[indexAI] = "gold";
                        otherTextDisplay.text = "AI's gold workers will be increased by 1. Current: " + gameStates[indexAI].resources["gold"].resourceCount;
                        //Debug.Log("AI's gold workers will be increased by 1. Current: " + gameStates[indexAI].resources["gold"].resourceCount);
                    }
                }
                else if (currentWork == Work.NewStoneMiner)
                {
                    if (canBuildWorker(gameStates[indexAI], workerBuildTimes[indexAI]))
                    {
                        workerBuildTimes[indexAI] = gameTime + LongTermPlanning.workerBuildTime;
                        workerBuildType[indexAI] = "stone";
                        otherTextDisplay.text = "AI's stone workers will be increased by 1. Current: " + gameStates[indexAI].resources["stone"].resourceCount;
                        //Debug.Log("AI's stone workers will be increased by 1. Current: " + gameStates[indexAI].resources["stone"].resourceCount);
                    }
                }
                else if (currentWork == Work.NewWoodsman)
                {
                    if (canBuildWorker(gameStates[indexAI], workerBuildTimes[indexAI]))
                    {
                        workerBuildTimes[indexAI] = gameTime + LongTermPlanning.workerBuildTime;
                        workerBuildType[indexAI] = "wood";
                        otherTextDisplay.text = "AI's wood workers will be increased by 1. Current: " + gameStates[indexAI].resources["wood"].resourceCount;
                        //Debug.Log("AI's wood workers will be increased by 1. Current: " + gameStates[indexAI].resources["wood"].resourceCount);
                    }
                }

                else if (currentWork == Work.Wait)
                { }
            }
        }
    }

    private void addWorker(String resourceType)
    {
        if (canBuildWorker(gameStates[indexPlayer], workerBuildTimes[indexPlayer]))
        {
            // TODO: Disable all add worker buttons till worker built
            workerBuildTimes[indexPlayer] = gameTime + LongTermPlanning.workerBuildTime * fps;
            workerBuildType[indexPlayer] = resourceType;
            otherTextDisplay.text = "Player's " + resourceType + " workers will be increased by 1 to: " + gameStates[indexPlayer].resources[resourceType];
            //Debug.Log("Player's " + resourceType + " workers will be increased by 1 to: " + gameStates[indexPlayer].resources[resourceType]);
        }
        else
        {
            otherTextDisplay.text = "Not enough resources to increase worker count.";
            //Debug.Log("Not enough resources to increase worker count.");
        }
    }

    public bool canBuildWorker(GameState gameState, long workerBuildTime)
    {
        return workerBuildTime < gameTime &&
            gameState.resources["gold"].resourceCount >= LongTermPlanning.workerCostGold &&
            gameState.resources["stone"].resourceCount >= LongTermPlanning.workerCostStone &&
            gameState.resources["wood"].resourceCount >= LongTermPlanning.workerCostWood;
    }

    public void buildWorker(GameState gameState, String resourceType)
    {
        // Subtract the cost of building a worker
        gameState.resources["gold"].resourceCount -= LongTermPlanning.workerCostGold;
        gameState.resources["stone"].resourceCount -= LongTermPlanning.workerCostStone;
        gameState.resources["wood"].resourceCount -= LongTermPlanning.workerCostWood;

        gameState.resources[resourceType].addWorkers(1);
        // TODO: Re-enable add worker buttons
    }


}
