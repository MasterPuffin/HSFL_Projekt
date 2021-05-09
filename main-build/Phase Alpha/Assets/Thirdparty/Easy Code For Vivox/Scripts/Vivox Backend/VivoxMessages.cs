using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VivoxUnity;

public class VivoxMessages
{

    public static event Action VivoxChannelMesssageSent;
    public static event Action<IChannelTextMessage> VivoxChannelMessageRecieved;

    public static event Action VivoxDirectMesssageSent;
    public static event Action<IDirectedTextMessage> VivoxDirectMessageRecieved;
    public static event Action<IFailedDirectedTextMessage> VivoxDirectMessageFailed;


    public void SubscribeToChannelMessages(IChannelSession channelSession)
    {
        channelSession.MessageLog.AfterItemAdded += OnChannelMessageRecieved;
    }  
    
    public void SubscribeToDirectMessages(ILoginSession loginSession)
    {
        loginSession.DirectedMessages.AfterItemAdded += OnDirectMessageRecieved;
        loginSession.FailedDirectedMessages.AfterItemAdded += OnDirectMessageFailedCallback;
    }
    
    public void UnsubscribeFromChannelMessages(IChannelSession channelSession)
    {
        channelSession.MessageLog.AfterItemAdded -= OnChannelMessageRecieved;
    }  
    
    public void UnsubscribeFromDirectMessages(ILoginSession loginSession)
    {
        loginSession.DirectedMessages.AfterItemAdded -= OnDirectMessageRecieved;
        loginSession.FailedDirectedMessages.AfterItemAdded -= OnDirectMessageFailedCallback;
    }



    #region Message Events


    private void OnVivoxChannelMessageRecieved(IChannelTextMessage channelTextMessage)
    {
        if (channelTextMessage != null)
        {
            VivoxChannelMessageRecieved?.Invoke(channelTextMessage);
        }

    }

    private void OnVivoxChannelMessageSent()
    {
        VivoxChannelMesssageSent?.Invoke();
    }

    private void OnVivoxDirectMessageSent()
    {
        VivoxDirectMesssageSent?.Invoke();
    }

    private void OnVivoxDirectMessageRecieved(IDirectedTextMessage message)
    {
        if (message != null)
        {
            VivoxDirectMessageRecieved?.Invoke(message);
        }

    }

    private void OnVivoxDirectMessageFailed(IFailedDirectedTextMessage failedMessage)
    {
        if (failedMessage != null)
        {
            VivoxDirectMessageFailed?.Invoke(failedMessage);
        }

    }


    #endregion


    #region Channel - Text Methods


    public void SendChannelMessage(IChannelSession channel, string inputMsg, string stanzaNameSpace = null, string stanzaBody = null)
    {
        if (channel.TextState == ConnectionState.Disconnected)
        {
            return;
        }

        channel.BeginSendText(null, inputMsg, stanzaNameSpace, stanzaBody, ar =>
        {
            try
            {
                channel.EndSendText(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return;
            }
            finally
            {
                OnVivoxChannelMessageSent();
            }
        });
    }

    public void SendEventMessage(IChannelSession channel, string eventMessage, string stanzaNameSpace, string stanzaBody)
    {
        if (channel.TextState == ConnectionState.Disconnected)
        {
            return;
        }

        channel.BeginSendText(null, eventMessage, stanzaNameSpace, stanzaBody, ar =>
        {
            try
            {
                channel.EndSendText(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return;
            }
        });
    }

    public void SendDirectMessage(ILoginSession login, string targetID, string message, string stanzaNameSpace = null, string stanzaBody = null)
    {
        var targetAccountID = new AccountId(login.LoginSessionId.Issuer, targetID, login.LoginSessionId.Domain);
        login.BeginSendDirectedMessage(targetAccountID, null, message, stanzaNameSpace, stanzaBody, ar =>
        {
            try
            {
                login.EndSendDirectedMessage(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            finally
            {
                OnVivoxDirectMessageSent();
            }
        });
    }

    public void SendDirectMessage(ILoginSession login, Dictionary<string, string> attemptedDirectMessages, string targetID, string message, string stanzaNameSpace = null, string stanzaBody = null)
    {
        var targetAccountID = new AccountId(login.LoginSessionId.Issuer, targetID, login.LoginSessionId.Domain);
        login.BeginSendDirectedMessage(targetAccountID, null, message, stanzaNameSpace, stanzaBody, ar =>
        {
            try
            {
                login.EndSendDirectedMessage(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            finally
            {
                OnVivoxDirectMessageSent();
            }
            attemptedDirectMessages.Add(login.DirectedMessageResult.RequestId,
                message);
        });
    }

    [System.Obsolete] // Vivox is Updating this functionality
    private void ArchiveRequestQuery(ILoginSession login)
    {
        login.BeginAccountArchiveQuery(null, null, null, login.Key, null, 49, null, null, -1, ar =>
        {
            try
            {
                Debug.Log(login.AccountArchiveResult.Running + "  is running");
                login.EndAccountArchiveQuery(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }


    #endregion


    #region Message Callbacks


    private void OnDirectMessageRecieved(object sender, QueueItemAddedEventArgs<IDirectedTextMessage> directMessage)
    {
        var directedMsgs = (IReadOnlyQueue<IDirectedTextMessage>)sender;

        
        while (directedMsgs.Count > 0)
        {
            var msg = directedMsgs.Dequeue();
            if(msg != null)
            {
                OnVivoxDirectMessageRecieved(directMessage.Value);
            }
        }
      //  On_Vivox_Direct_Message_Recieved(directMessage.Value);
    }

    private void OnDirectMessageFailedCallback(object sender, QueueItemAddedEventArgs<IFailedDirectedTextMessage> failedMessage)
    {
        var failed = (IReadOnlyQueue<IFailedDirectedTextMessage>)sender;
        while(failed.Count > 0)
        {
            var msg = failed.Dequeue();
            if(msg != null)
            {
                OnVivoxDirectMessageFailed(msg);
            }
        }
       // On_Vivox_Direct_Message_Failed(failedMessage.Value);
    }

    private void OnChannelMessageRecieved(object sender, QueueItemAddedEventArgs<IChannelTextMessage> channelMessage)
    {
        var messages = (IReadOnlyQueue<IChannelTextMessage>)sender;
        while (messages.Count > 0)
        {
            var msg = messages.Dequeue();
            if (msg != null)
            {
                OnVivoxChannelMessageRecieved(msg);
            }
        }
        //if (Check_Message_For_Events(msg)) return;
        //Check_Msg_For_Args(msg);
       // OnVivoxChannelMessageRecieved(channelMessage.Value);
    }


    #endregion


    [System.Obsolete] //  Vivox is Updating this functionality
    private void OnArchiveResultAdded(object sender, QueueItemAddedEventArgs<IAccountArchiveMessage> queueArchiveMsg)
    {

        var source = (IReadOnlyQueue<IAccountArchiveMessage>)sender;

        while (source.Count > 0)
        {
            Debug.Log(source.Dequeue().Message);
        }
    }
}
