using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CampaignTypeButton : MonoBehaviour {
    private CampaignType value;

    private Button myButton;
    private HomeScreenController homeScreenController;

    public void Start() {
        this.myButton = this.GetComponent<Button>();
        myButton.interactable = true;

        this.homeScreenController = GameObject.FindWithTag(GameSetup.HOME_SCREEN_CONTROLLER_TAG).GetComponent<HomeScreenController>();

        myButton.onClick.AddListener(delegate { myClick(); });
    }

    // What type of building does this button try to build
    public CampaignTypeButton(CampaignType value) {
        this.value = value;
        Text myText = this.transform.GetChild(0).GetComponent<Text>();
        myText.text = this.value.ToString();
    }

    private void forceTurnOn()  { this.myButton.interactable = true; }
    private void forceTurnOff() { this.myButton.interactable = false; ; }

    private void myClick() {
        homeScreenController.setCampaignType(this.value);
    }
}
