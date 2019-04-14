using System;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    private float lastGameTick = 0;

    public static CampaignType campaignType;
    public static AIPersonalityType aiPersonality;
    public static LongTermPlannerType longTermPlanner;

    private readonly int noOfActors = 2;

    private GameState[] gameStates;
    private static int indexHuman = 0;
    private static int indexAi = 1;

    private List<BuildingGS> currentGameStates;
    private List<BuildingGS> targetGameStates;

    private int[] currentTargetGsIndex;

    private Stack<Work> aiCurrentPlan;

    public void Awake()
    {
        setStartGameState();
        setTargetGameStates();
        // TODO: Add display for current targets for both human and AI
        executeNextTargetGsAi();
    }

    private void executeNextTargetGsAi()
    { 
        BuildingGS initialGs = this.currentGameStates[indexAi];
        BuildingGS targetGs = modifyGameStateForPersonality(this.targetGameStates[currentTargetGsIndex[indexAi]], aiPersonality);
        ltpPlan(initialGs, targetGs);
    }

    private void ltpPlan(BuildingGS initialGs, BuildingGS targetGsAi)
    {
        ALongTermPlanner planner;
        switch (God.longTermPlanner)
        {
            case LongTermPlannerType.MemoryBound:
                planner = new MemoryBoundedLongTermPlanner();
                break;
            case LongTermPlannerType.IterativeDeepening:
                planner = new IterativeDeepiningLongTermPlanner();
                break;
            case LongTermPlannerType.Accordion:
                planner = new AccordionLongTermPlanner();
                break;
            default:
                planner = new MemoryBoundedLongTermPlanner();
                break;
        }
        planner.plan(initialGs, targetGsAi, planningCompleteCallBack);
    }

    public bool planningCompleteCallBack(Stack<Work> result)
    {
        Debug.Log("Path found");
        this.aiCurrentPlan = result;
        // TODO : Whole game is being run here. So, get current game state of AI 
        // and pass it to the method as start game state
        return true;
    }

    private BuildingGS modifyGameStateForPersonality(BuildingGS targetGsHuman, AIPersonalityType aiPersonality)
    {
        switch (aiPersonality)
        {
            case AIPersonalityType.GoldDigger:
                // TODO
                break;
            case AIPersonalityType.Pragmatist:
                // TODO
                break;
            case AIPersonalityType.Warrior:
            case AIPersonalityType.ScienceGeek:
            case AIPersonalityType.Conservative:
            default:
                break;
        }
        return targetGsHuman;
    }

    private void setStartGameState()
    {
        BuildingGS initialGs = new BuildingGS();
        initialGs.setStockpile(ResourceType.Gold, 1000);
        initialGs.addResourcePerTick(ResourceType.Gold, 0);
        initialGs.setStockpile(ResourceType.Stone, 1000);
        initialGs.addResourcePerTick(ResourceType.Stone, 0);
        initialGs.setStockpile(ResourceType.Wood, 1000);
        initialGs.addResourcePerTick(ResourceType.Wood, 0);
        this.currentGameStates[indexHuman] = initialGs;
        this.currentGameStates[indexAi] = initialGs;
    }

    private void setTargetGameStates()
    {
        BuildingGS targetGs = new BuildingGS();
        targetGs.setStockpile(ResourceType.Gold, 2000);
        targetGs.addResourcePerTick(ResourceType.Gold, 10);
        targetGs.setStockpile(ResourceType.Stone, 2000);
        targetGs.addResourcePerTick(ResourceType.Stone, 5);
        targetGs.setStockpile(ResourceType.Wood, 2000);
        targetGs.addResourcePerTick(ResourceType.Wood, 6);
        targetGs.setStockpile(ResourceType.Silver, 200);
        targetGs.addResourcePerTick(ResourceType.Silver, 9);
        targetGameStates.Add(targetGs);

        BuildingGS targetGs2 = new BuildingGS();
        targetGs2.setStockpile(ResourceType.Gold, 4000);
        targetGs2.addResourcePerTick(ResourceType.Gold, 10);
        targetGs2.setStockpile(ResourceType.Stone, 4000);
        targetGs2.addResourcePerTick(ResourceType.Stone, 5);
        targetGs2.setStockpile(ResourceType.Wood, 4000);
        targetGs2.addResourcePerTick(ResourceType.Wood, 6);
        targetGs2.setStockpile(ResourceType.Silver, 400);
        targetGs2.addResourcePerTick(ResourceType.Silver, 9);
        targetGameStates.Add(targetGs2);

        BuildingGS targetGs3 = new BuildingGS();
        targetGs3.setStockpile(ResourceType.Gold, 8000);
        targetGs3.addResourcePerTick(ResourceType.Gold, 20);
        targetGs3.setStockpile(ResourceType.Stone, 8000);
        targetGs3.addResourcePerTick(ResourceType.Stone, 20);
        targetGs3.setStockpile(ResourceType.Wood, 8000);
        targetGs3.addResourcePerTick(ResourceType.Wood, 20);
        targetGs3.setStockpile(ResourceType.Silver, 800);
        targetGs3.addResourcePerTick(ResourceType.Silver, 20);
        targetGameStates.Add(targetGs3);
    }

    private void Update()
    {
        if (Time.time - lastGameTick > GameSetup.TICK_LENGHT_SEC)
        {
            // If it's been a while since our last game tick
            // Propogate an "in game tick"

            tick();
            lastGameTick = Time.time;
        }

    }

    private void tick()
    {
        // Simulate one tick of this game
        for (int indexActor = 0; indexActor < noOfActors; indexActor++)
        {
            BuildingGS gameState = this.currentGameStates[indexActor];
            gameState.timePasses(1);
            if (isTargetReached(indexActor))
            {
                targetReached(indexActor);
            }

        }
        // TODO
        //displayAllResourceCount();

        //resourceDisplay.updateTick();
    }

    private void targetReached(int indexActor)
    {
        celebrateTargetReached(indexActor);
        planForNextTarget(indexActor);
    }

    private void planForNextTarget(int indexActor)
    {
        if (this.currentTargetGsIndex[indexActor] < targetGameStates.Count - 1)
        {
            currentTargetGsIndex[indexActor]++;
            if (indexActor == indexAi) {
                executeNextTargetGsAi();
            }
        }
        else
        {
            declareGameWon(indexActor);
        }
    }

    private void declareGameWon(int indexActor)
    {
        // TODO
    }

    private void celebrateTargetReached(int indexActor)
    {
        // TODO
    }

    private bool isTargetReached(int indexActor)
    {
        // TODO
        return false;
    }
}