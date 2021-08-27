using UnityEditor;
using UnityEngine;

public class PickupableItem : MonoBehaviour {
    public string uiName;
    public string id;
    
    public enum pickupTypes {Tablet, Card, Other};
    public pickupTypes pickupType;

    public void onPickup() {
       
        switch (pickupType) {
            case pickupTypes.Tablet:
                GetComponent<UnlockTablet>().OpenRuins();
                break;
            case pickupTypes.Card:
                GetComponent<TestOnPickup>().PickedUp();
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