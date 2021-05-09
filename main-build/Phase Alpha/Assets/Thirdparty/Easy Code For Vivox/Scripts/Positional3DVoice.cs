using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;
using System.Collections;


public class Positional3DVoice : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform listenerPosition;
    public Transform speakerPosition;
    private Vector3 lastListenerPosition;
    private Vector3 lastSpeakerPosition;

    private bool positionalChannelExists = false;
    private string channelName;


    private void Start()
    {
        StartCoroutine(Handle3DPositionUpdates(.3f));
    }  



    IEnumerator Handle3DPositionUpdates(float nextUpdate)
    {
        yield return new WaitForSeconds(nextUpdate);
        if (VivoxBehaviour.mainLoginSession.State == LoginState.LoggedIn)
        {
            if (positionalChannelExists)
            {
                Update3DPosition();
            }
            else
            {
                CheckIfChannelValid();
            }
        }

        StartCoroutine(Handle3DPositionUpdates(nextUpdate));
    }

    public bool CheckIfChannelValid()
    {
        foreach(KeyValuePair<string, IChannelSession> session in VivoxBehaviour.mainChannelSessions)
        {
            if(session.Value.Channel.Type == ChannelType.Positional)
            {
                channelName = session.Value.Channel.Name;
                if (VivoxBehaviour.mainChannelSessions[channelName].ChannelState == ConnectionState.Connected)
                {
                    if (VivoxBehaviour.mainChannelSessions[channelName].AudioState == ConnectionState.Connected)
                    {
                        positionalChannelExists = true;
                        return true;
                    }
                }
            }
        }
        return false;
    }


    public void Update3DPosition()
    {
        if(listenerPosition.position != lastListenerPosition || speakerPosition.position != lastSpeakerPosition)
        {
            VivoxBehaviour.mainChannelSessions[channelName].Set3DPosition(speakerPosition.position, listenerPosition.position, listenerPosition.forward, listenerPosition.up);
            Debug.Log($"{VivoxBehaviour.mainChannelSessions[channelName].Channel.Name} 3D positon has been updated");
        }
        lastListenerPosition = listenerPosition.position;
        lastSpeakerPosition = speakerPosition.position;
    }



}