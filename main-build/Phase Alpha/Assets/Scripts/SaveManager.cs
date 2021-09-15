using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;
using MLAPI;

public class SaveManager : MonoBehaviour {
    public void Save() {
        Debug.Log("SaveManager: Saving");
        
        QuickSaveWriter writer = QuickSaveWriter.Create("Player");
        writer.Write("Position", transform.position);
        writer.Commit();
    }

    public void Load(PlayerController playerController) {
        Debug.Log("SaveManager: Loading");
        
        QuickSaveReader reader = QuickSaveReader.Create("Player");
        Vector3 position = reader.Read<Vector3>("Position");
        playerController.TeleportPlayer(position);
    }
}