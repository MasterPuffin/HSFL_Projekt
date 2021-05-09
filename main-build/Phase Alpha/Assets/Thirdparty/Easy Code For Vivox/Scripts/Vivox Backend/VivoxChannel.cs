using System;
using System.ComponentModel;
using UnityEngine;
using VivoxUnity;
using VATAuthenticate;

public class VivoxChannel
{

    public static event Action<IChannelSession> VivoxChannelConnecting;
    public static event Action<IChannelSession> VivoxChannelConnected;
    public static event Action<IChannelSession> VivoxChannelDisconnecting;
    public static event Action<IChannelSession> VivoxChannelDisconnected;


    public void Subscribe(IChannelSession channelSession)
    {
        channelSession.PropertyChanged += OnChannelStatePropertyChanged;
    }
    
    public void Unsubscribe(IChannelSession channelSession)
    {
        channelSession.PropertyChanged -= OnChannelStatePropertyChanged;
    }



    #region Channel Event Methods


    private void OnVivoxChannelConnecting(IChannelSession channelSession)
    {
        if (channelSession != null)
        {
            VivoxChannelConnecting?.Invoke(channelSession);
        }
    }

    private void OnVivoxChannelConnected(IChannelSession channelSession)
    {
        if (channelSession != null)
        {
            VivoxChannelConnected?.Invoke(channelSession);
        }
    }

    private void OnVivoxChannelDisconnecting(IChannelSession channelSession)
    {
        if (channelSession != null)
        {
            VivoxChannelDisconnecting?.Invoke(channelSession);
        }
    }

    private void OnVivoxChannelDisconnected(IChannelSession channelSession)
    {
        if (channelSession != null)
        {
            VivoxChannelDisconnected?.Invoke(channelSession);

            Unsubscribe(channelSession);
        }
    }

   
    #endregion



    #region Channel Methods


    public void JoinChannel(bool includeVoice, bool includeText, bool switchTransmissionToThisChannel,
   IChannelSession channelSession, string tokenKey, TimeSpan timeSpan)
    {
        Subscribe(channelSession);

        channelSession.BeginConnect(includeVoice, includeText, switchTransmissionToThisChannel,
        channelSession.GetConnectToken(tokenKey, timeSpan), ar =>
        {
            try
            {
                channelSession.EndConnect(ar);
            }
            catch (Exception e)
            {
                Unsubscribe(channelSession);
                Debug.Log(e.StackTrace);
            }
        });
    }

    public void JoinChannelProduction(bool includeVoice, bool includeText, bool switchTransmissionToThisChannel,
       IChannelSession channelSession, string tokenKey, string tokenIssuer, int uniqueRequestID, bool joinMuted = false)
    {
        Subscribe(channelSession);

        var epoch = VAT.SecondsSinceUnixEpochPlusDuration(TimeSpan.FromSeconds(90));
        string channelToken = "Error : Invalid Token";

        if (joinMuted)
        {
            channelToken = VAT.Token_f(tokenKey, tokenIssuer, epoch, "join_muted", uniqueRequestID, null, channelSession.Parent.GetSIP(),
            VivoxAccessTokens.GetChannelSIP(channelSession.Channel.Type, tokenIssuer, channelSession.Channel.Name, channelSession.Parent.Key.Domain));
        }
        else
        {
            channelToken = VAT.Token_f(tokenKey, tokenIssuer, epoch, "join", uniqueRequestID, null, channelSession.Parent.GetSIP(),
            VivoxAccessTokens.GetChannelSIP(channelSession.Channel.Type, tokenIssuer, channelSession.Channel.Name, channelSession.Parent.Key.Domain));
        }

        channelSession.BeginConnect(includeVoice, includeText, switchTransmissionToThisChannel, channelToken, ar =>
        {
            try
            {
                channelSession.EndConnect(ar);
            }
            catch (Exception e)
            {
                Unsubscribe(channelSession);
                Debug.Log(e.StackTrace);
            }
        });
    }

    public void JoinChannelProductionMMO(bool includeVoice, bool includeText, bool switchTransmissionToThisChannel, IChannelSession channelSession,
        string tokenKey, string tokenIssuer, int uniqueRequestID, bool joinMuted = false)
    {
        Subscribe(channelSession);

        var epoch = VAT.SecondsSinceUnixEpochPlusDuration(TimeSpan.FromSeconds(90));
        string channelToken = "Error : Invalid Token";

        if (joinMuted)
        {
            if (channelSession.Channel.Type != ChannelType.Positional)
            {
                channelToken = VAT.Token_f(tokenKey, tokenIssuer, epoch, "join_muted", uniqueRequestID, null, channelSession.Parent.GetSIP(),
                VivoxAccessTokens.GetChannelSIP(channelSession.Channel.Type, tokenIssuer, channelSession.Channel.Name, channelSession.Parent.Key.Domain));
            }
            else
            {
                channelToken = VAT.Token_f(tokenKey, tokenIssuer, epoch, "join_muted", uniqueRequestID, null, channelSession.Parent.GetSIP(),
                VivoxAccessTokens.GetChannelSIP(channelSession.Channel.Type, tokenIssuer, channelSession.Channel.Name, channelSession.Parent.Key.Domain,
                channelSession.Channel.Properties));
            }
        }
        else
        {
            if (channelSession.Channel.Type != ChannelType.Positional)
            {
                channelToken = VAT.Token_f(tokenKey, tokenIssuer, epoch, "join", uniqueRequestID, null, channelSession.Parent.GetSIP(),
                VivoxAccessTokens.GetChannelSIP(channelSession.Channel.Type, tokenIssuer, channelSession.Channel.Name, channelSession.Parent.Key.Domain));
            }
            else
            {
                channelToken = VAT.Token_f(tokenKey, tokenIssuer, epoch, "join", uniqueRequestID, null, channelSession.Parent.GetSIP(),
                VivoxAccessTokens.GetChannelSIP(channelSession.Channel.Type, tokenIssuer, channelSession.Channel.Name, channelSession.Parent.Key.Domain,
                channelSession.Channel.Properties));
            }

            channelSession.BeginConnect(includeVoice, includeText, switchTransmissionToThisChannel, channelToken, ar =>
            {
                try
                {
                    channelSession.EndConnect(ar);
                }
                catch (Exception e)
                {
                    Unsubscribe(channelSession);
                    Debug.Log(e.StackTrace);
                }
            });
        }
    }

    public void LeaveChannel(ILoginSession loginSession, IChannelSession channelToRemove)
    {
        if (channelToRemove != null)
        {
            channelToRemove.Disconnect();

            loginSession.DeleteChannelSession(channelToRemove.Key);
        }
    }


    #endregion



    #region Channel Callbacks


    private void OnChannelStatePropertyChanged(object sender, PropertyChangedEventArgs propArgs)
    {
        var senderIChannelSession = (IChannelSession)sender;

        if (propArgs.PropertyName == "ChannelState")
        {
            switch (senderIChannelSession.ChannelState)
            {
                case ConnectionState.Connecting:
                    OnVivoxChannelConnecting(senderIChannelSession);
                    break;
                case ConnectionState.Connected:
                    OnVivoxChannelConnected(senderIChannelSession);
                    break;
                case ConnectionState.Disconnecting:
                    OnVivoxChannelDisconnecting(senderIChannelSession);
                    break;
                case ConnectionState.Disconnected:
                    OnVivoxChannelDisconnected(senderIChannelSession);
                    break;
            }
        }
    }


    #endregion


}
