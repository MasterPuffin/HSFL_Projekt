using UnityEditor;
using UnityEngine;

public class UseableItem : MonoBehaviour
{
    public string uiName;
    public string id;

    public enum itemTypes {Other};
    public itemTypes itemType;

    public void onUse() {
        switch (itemType) {
            default:
                //do nothing
                break;
        }
    }
}