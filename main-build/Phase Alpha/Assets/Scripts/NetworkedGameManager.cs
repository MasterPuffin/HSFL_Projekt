using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

/*
 * This script controls all networked variables
 */

public class NetworkedGameManager : NetworkBehaviour {
    private static readonly NetworkVariableSettings nws_ServerOnly = new NetworkVariableSettings {WritePermission = NetworkVariablePermission.ServerOnly};

    public NetworkVariable<string> vivoxChannel = new NetworkVariable<string>(nws_ServerOnly, "");
}