using System.ComponentModel;
using System.Linq;
using VivoxUnity;
using UnityEngine;
using System;

public class VivoxTextToSpeech
{

    public string maleVoice { get; } = "en_US male";
    public string femaleVoice { get; } = "en_US female";

    public static event Action<ITTSMessageQueueEventArgs> VivoxTTSMessageAdded;
    public static event Action<ITTSMessageQueueEventArgs> VivoxTTSMessageRemoved;
    public static event Action<ITTSMessageQueueEventArgs> VivoxTTSMessageUpdated;

    
    public void Subscribe(ILoginSession loginSession)
    {
        loginSession.TTS.Messages.AfterMessageAdded += OnTTSMessageAdded;
        loginSession.TTS.Messages.BeforeMessageRemoved += OnTTSMessageRemoved;
        loginSession.TTS.Messages.AfterMessageUpdated += OnTTSMessageUpdated;
    }  
    
    public void Unsubscribe(ILoginSession loginSession)
    {
        loginSession.TTS.Messages.AfterMessageAdded -= OnTTSMessageAdded;
        loginSession.TTS.Messages.BeforeMessageRemoved -= OnTTSMessageRemoved;
        loginSession.TTS.Messages.AfterMessageUpdated -= OnTTSMessageUpdated;
    }



    #region Text-to-Speech Events


    private void OnVivoxTTSMessageAdded(ITTSMessageQueueEventArgs ttsArgs)
    {
        if (ttsArgs != null)
        {
            VivoxTTSMessageAdded?.Invoke(ttsArgs);
        }
    }

    private void OnVivoxTTSMessageRemoved(ITTSMessageQueueEventArgs ttsArgs)
    {
        if (ttsArgs != null)
        {
            VivoxTTSMessageRemoved?.Invoke(ttsArgs);
        }
    }

    private void OnVivoxTTSMessageUpdated(ITTSMessageQueueEventArgs ttsArgs)
    {
        if (ttsArgs != null)
        {
            VivoxTTSMessageUpdated?.Invoke(ttsArgs);
        }
    }


    #endregion


    #region Text-To-Speech Methods


    public void TTSChooseVoice(string voiceName, ILoginSession loginSession)
    {
        ITTSVoice voice = loginSession.TTS.AvailableVoices.FirstOrDefault(v => v.Name == voiceName);
        if (voice != null)
        {
            loginSession.TTS.CurrentVoice = voice;
        }
    }

    public void TTSSpeak(string message, TTSDestination destination, ILoginSession loginSession)
    {
        switch (destination)
        {
            case TTSDestination.LocalPlayback:
                message.TTS_Msg_Local_PlayOverCurrent(loginSession);
                break;
            case TTSDestination.RemoteTransmission:
                message.TTS_Msg_Local_Remote_PlayOverCurrent(loginSession);
                break;
            case TTSDestination.RemoteTransmissionWithLocalPlayback:
                message.TTS_Msg_Local_Remote_PlayOverCurrent(loginSession);
                break;
            case TTSDestination.QueuedLocalPlayback:
                message.TTS_Msg_Queue_Local(loginSession);
                break;
            case TTSDestination.QueuedRemoteTransmission:
                message.TTS_Msg_Queue_Remote(loginSession);
                break;
            case TTSDestination.QueuedRemoteTransmissionWithLocalPlayback:
                message.TTS_Msg_Queue_Remote_Local(loginSession);
                break;
            case TTSDestination.ScreenReader:
                message.TTS_Msg_Local_ReplaceCurrentPlaying(loginSession);
                break;
        }
    }


    #endregion


    #region Text-To-Speech Callbacks


    private void OnTTSMessageAdded(object sender, ITTSMessageQueueEventArgs ttsArgs)
    {
        var source = (ITTSMessageQueue)sender;
        if (source.Count > 9)
        {
            Debug.Log("Cant keep over 10 messages in Queue");
        }
        OnVivoxTTSMessageAdded(ttsArgs);
    }

    private void OnTTSMessageRemoved(object sender, ITTSMessageQueueEventArgs ttsArgs)
    {
        var source = (ITTSMessageQueue)sender;
        if (source.Count >= 9)
        {
            Debug.Log("Cant keep over 10 messages in Queue");
        }
        OnVivoxTTSMessageRemoved(ttsArgs);
    }

    private void OnTTSMessageUpdated(object sender, ITTSMessageQueueEventArgs ttsArgs)
    {
        var source = (ITTSMessageQueue)sender;
        if (source.Count >= 9)
        {
            Debug.Log("Cant keep over 10 messages in Queue");
        }
        OnVivoxTTSMessageUpdated(ttsArgs);
    }

    private void On_TTS_PropertyChanged(object sender, PropertyChangedEventArgs ttsPropArgs)
    {
        Debug.Log($"TTS Property Name == {ttsPropArgs.PropertyName.ToString()}");
       // if(ttsPropArgs.PropertyName == "")
       // {
            // todo check documentation and experiment
            Debug.Log(ttsPropArgs.PropertyName);
        //}
        
    }

    #endregion



}
