using System;
using System.ComponentModel;
using UnityEngine;
using VivoxUnity;

public class VivoxChannelText
{

    public static event Action<IChannelSession> VivoxTextChannelConnecting;
    public static event Action<IChannelSession> VivoxTextChannelConnected;
    public static event Action<IChannelSession> VivoxTextChannelDisconnecting;
    public static event Action<IChannelSession> VivoxTextChannelDisconnected;


    public void Subscribe(IChannelSession channelSession)
    {
        channelSession.PropertyChanged += OnChannelTextPropertyChanged;
    }

    public void Unsubscribe(IChannelSession channelSession)
    {
        channelSession.PropertyChanged -= OnChannelTextPropertyChanged;
    }



    #region Text Channel Events


    private void OnVivoxTextChannelConnecting(IChannelSession channelSession)
    {
        if (channelSession != null)
        {
            VivoxTextChannelConnecting?.Invoke(channelSession);
        }
    }

    private void OnVivoxTextChannelConnected(IChannelSession channelSession)
    {
        if (channelSession != null)
        {
            VivoxTextChannelConnected?.Invoke(channelSession);
        }
    }

    private void OnVivoxTextChannelDisconnecting(IChannelSession channelSession)
    {
        if (channelSession != null)
        {
            VivoxTextChannelDisconnecting?.Invoke(channelSession);
        }
    }

    private void OnVivoxTextChannelDisconnected(IChannelSession channelSession)
    {
        if (channelSession != null)
        {
            VivoxTextChannelDisconnected?.Invoke(channelSession);

            Unsubscribe(channelSession);
        }
    }


    #endregion



    #region Channel - Text Methods


    public void ToggleTextChannelActive(IChannelSession channelSession, bool join)
    {
        if (join)
        {
            Subscribe(channelSession);
        }

        channelSession.BeginSetTextConnected(join, ar =>
        {
            try
            {
                channelSession.EndSetTextConnected(ar);
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



    #region Channel - Text Callbacks


    private void OnChannelTextPropertyChanged(object sender, PropertyChangedEventArgs propArgs)
    {
        var senderIChannelSession = (IChannelSession)sender;

        if (propArgs.PropertyName == "TextState")
        {
            switch (senderIChannelSession.TextState)
            {
                case ConnectionState.Connecting:
                    OnVivoxTextChannelConnecting(senderIChannelSession);
                    break;

                case ConnectionState.Connected:
                    OnVivoxTextChannelConnected(senderIChannelSession);
                    break;

                case ConnectionState.Disconnecting:
                    OnVivoxTextChannelDisconnecting(senderIChannelSession);
                    break;

                case ConnectionState.Disconnected:
                    OnVivoxTextChannelDisconnected(senderIChannelSession);
                    break;
            }
        }
    }



    #endregion

}
