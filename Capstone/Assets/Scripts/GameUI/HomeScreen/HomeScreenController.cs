using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HomeScreenController : MonoBehaviour
{
    private AIPersonalityType aIPersonality;
    private CampaignType campaignType;

    public List<CampaignTypeButton> campaignTypeButtons;
    public List<AiPersonalityButton> aiPersonalityTypeButtons;
    public Button startGameButton;

    // Start is called before the first frame update
    void Start()
    {
        foreach (CampaignType campaign in Enum.GetValues(typeof(CampaignType))) {
            campaignTypeButtons.Add(new CampaignTypeButton(campaign));
        }
        foreach (AIPersonalityType personality in Enum.GetValues(typeof(AIPersonalityType)))
        {
            aiPersonalityTypeButtons.Add(new AiPersonalityButton(personality));
        }
        startGameButton.onClick.AddListener(delegate { startGame(); });
    }

    private void startGame()
    {
        startGameButton.interactable = false;
        God.campaignType = campaignType;
        God.aiPersonality = aIPersonality;
        God.longTermPlanner = LongTermPlannerType.MemoryBound;
        new God().startGame();
    }

    public void setPersonality(AIPersonalityType value)
    {
        this.aIPersonality = value;
    }

    public AIPersonalityType getPersonality()
    {
        return this.aIPersonality;
    }

    public void setCampaignType(CampaignType value)
    {
        this.campaignType = value;
    }

    public CampaignType getCampaignType()
    {
        return this.campaignType;
    }


}
