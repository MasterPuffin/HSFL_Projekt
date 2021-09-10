using UnityEditor;
using UnityEngine;

public class PickupableItem : MonoBehaviour {
    public string uiName;
    public string id;
    public GameObject audioManager;
    public enum pickupTypes {Tablet, Card, Artefact, Other};
    public pickupTypes pickupType;

    public void onPickup()
    {
        audioManager.GetComponent<AudioManager>().PickSomethingUpSound();
        switch (pickupType) {
            case pickupTypes.Tablet:
                GetComponent<UnlockTablet>().OpenRuins();
                break;
            case pickupTypes.Card:
                GetComponent<TestOnPickup>().PickedUp();
                break;
            case pickupTypes.Artefact:
                GetComponent<QuestTrigger>().ManualTrigger();
                break;
            default:
                //do nothing
                break;
        }
    }

    /*
     //Currently there is no discard method
    public void onDiscard() {
        switch (pickupType) {
            case pickupTypes.Tablet:
                break;
            default:
                //do nothing
                break;
        }
    }
    */
}