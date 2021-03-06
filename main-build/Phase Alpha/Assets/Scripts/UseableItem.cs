using UnityEditor;
using UnityEngine;

public class UseableItem : MonoBehaviour
{
    public string uiName;
    public string id;

    public enum itemTypes {Tablet, Labyrinth, Door, Plate, Other};
    public itemTypes itemType;
    //private GameObject tempGameObject;
    public void onUse() {
        //tempGameObject = new GameObject();
        switch (itemType) {
            case itemTypes.Tablet:
                GetComponent<OnTabletUse>().TabletUse();
                break;
            case itemTypes.Labyrinth:
                GetComponent<ButtonLabyrinth>().ButtonLab();
                break;
            case itemTypes.Door:
                GetComponent<ButtonDoor>().Button();
                break;
            case itemTypes.Plate:
                GetComponent<PlaceBlocksOnPlate>().Pressed();
                break;
            default:
                //do nothing
                break;
        }
    }
}