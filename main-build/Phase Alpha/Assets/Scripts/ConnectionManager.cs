using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;

/*
 * This Script handles the connection on GUI button press as well as the GUI status
 * elements for the network status 
 */

public class ConnectionManager : MonoBehaviour {
    private static string ip = "127.0.0.1";

    void OnGUI() {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
            StartButtons();
        } else {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    static void StartButtons() {
        ip = GUILayout.TextField(ip);
        
        if (GUILayout.Button("Host")) {
            //Create unique Vivox Channel ID
            GameObject.Find("NetworkedGameManager").GetComponent<NetworkedGameManager>().vivoxChannel.Value = Helpers.RandomString(10, true);
            NetworkManager.Singleton.StartHost();
        }
        
        if (GUILayout.Button("Client") && ip != "") {
            Debug.Log("Connecting to " + ip);
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ip;
            NetworkManager.Singleton.StartClient();
        }
    }
    

    static void StatusLabels() {
        var mode = NetworkManager.Singleton.IsHost ? "Host" :
            NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
                        NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
}