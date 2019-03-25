using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignWorkerButton : MonoBehaviour
{
    private Button myButton;
    private IBuilding focusedBuilding;

    private void Start() {
        myButton = GetComponent<Button>();

        myButton.onClick.AddListener(delegate { this.callGameController(); });
    }

    private void turnOn() { this.myButton.interactable = true; }
    public void tryTurnOn(IBuilding newBuilding) {
        // Only turn on if we have unassigned workers wandering arround
        if (GameController.singleton.anyFreeWorkers() &&
           newBuilding.openWorkerSlots() > 0)
        {
            // If we have free workers wandering arround &&
            //    the building has free slots
            focusedBuilding = newBuilding;
            turnOn();
        } else {
            turnOff();
        }
    }
    
    public void turnOff() {
        focusedBuilding = null;
        this.myButton.interactable = false;
    }
    
    private void callGameController() {
        Debug.Log("Nocab flag AssignWorkerButton 1");
        GameController.singleton.cleanAssignWorker(focusedBuilding);
        tryTurnOn(focusedBuilding);
    }
}
