using System;
using System.ComponentModel;
using UnityEngine;
using VivoxUnity;

public class VivoxChannelVoice
{
    public static event Action<IChannelSession> VivoxVoiceChannelConnecting;
    public static event Action<IChannelSession> VivoxVoiceChannelConnected;
    public static event Action<IChannelSession> VivoxVoiceChannelDisconnecting;
    public static event Action<IChannelSession> VivoxVoiceChannelDisconnected;   


    public void Subscribe(IChannelSession channelSession)
    {
        channelSession.PropertyChanged += OnChannelAudioPropertyChanged;
    }

    public void Unsubscribe(IChannelSession channelSession)
    {
        channelSession.PropertyChanged -= OnChannelAudioPropertyChanged;
    }



    #region Voice Channel Events


    private void OnVivoxAudioChannelConnecting(IChannelSession channelSession)
    {
        if (channelSession != null)
        {
            VivoxVoiceChannelConnecting?.Invoke(channelSession);
        }
    }

    private void OnVivoxAudioChannelConnected(IChannelSession channelSession)
    {
        if (channelSession != null)
        {
            VivoxVoiceChannelConnected?.Invoke(channelSession);
        }
    }

    private void OnVivoxAudioChannelDisconnecting(IChannelSession channelSession)
    {
        if (channelSession != null)
        {
            VivoxVoiceChannelDisconnecting?.Invoke(channelSession);
        }
    }

    private void OnVivoxAudioChannelDisconnected(IChannelSession channelSession)
    {
        if (channelSession != null)
        {
            VivoxVoiceChannelDisconnected?.Invoke(channelSession);

            Unsubscribe(channelSession);
        }
    }


    #endregion



    #region Channel - Voice Methods


    public void ToggleAudioChannelActive(IChannelSession channelSession, bool join)
    {
        if (join)
        {
            Subscribe(channelSession);
        }

        channelSession.BeginSetAudioConnected(join, true, ar =>
        {
            try
            {
                channelSession.EndSetAudioConnected(ar);
            }
            catch (Exception e)
            {
                Unsubscribe(channelSession);
                Debug.Log(e.Message);
            }
        });

        if(!join)
        {
            Unsubscribe(channelSession);
        }
    }


    #endregion



    #region Channel - Voice Callbacks


    private void OnChannelAudioPropertyChanged(object sender, PropertyChangedEventArgs propArgs)
    {
        var senderIChannelSession = (IChannelSession)sender;

        if (propArgs.PropertyName == "AudioState")
        {
            switch (senderIChannelSession.AudioState)
            {
                case ConnectionState.Connecting:
                    OnVivoxAudioChannelConnecting(senderIChannelSession);
                    break;

                case ConnectionState.Connected:
                    OnVivoxAudioChannelConnected(senderIChannelSession);
                    break;

                case ConnectionState.Disconnecting:
                    OnVivoxAudioChannelDisconnecting(senderIChannelSession);
                    break;

                case ConnectionState.Disconnected:
                    OnVivoxAudioChannelDisconnected(senderIChannelSession);
                    break;
            }
        }
    }


    #endregion

}
