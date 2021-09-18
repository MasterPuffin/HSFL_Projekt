using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VivoxUnity;

/// <summary>
/// Example on how to Inherit from VivoxManager Instead of using 
/// <para>Vivox_Manager  vivoxManager = FindObjectOfType&lt;VivoxManager&gt;();</para>
/// <para>GameObject vivoxManager = this.GetComponent&lt;VivoxManager&gt;();</para>
/// <para>GameObject vivoxManager = GameObject.FindObjectOfType&lt;VivoxManager&gt;();</para>
/// <para>With inheritance you can just override any callbacks you want instead of writing your own methods from scratch and subscribing to events</para>
/// </summary>
public class VivoxManagerInheritedExample : VivoxManager
{
    [SerializeField] Text messageText;
    [SerializeField] Text userNameText;
    [SerializeField] Text channelNameText;
    [SerializeField] Text remotePlayerNameText;
    [SerializeField] Text remotePlayerAudioText;

    [SerializeField] InputField messageInput;
    [SerializeField] InputField channelNameInput;
    [SerializeField] InputField userNameInput;
    [SerializeField] InputField remotePlayerNameInput;
    [SerializeField] InputField switchScenesInput;

    [SerializeField] string serverRegion = "NA";
    [SerializeField] string squadName = "Squad1";

    [SerializeField] Toggle toggleVoiceInChannel;
    [SerializeField] Toggle toggleTextInChannel;

    [SerializeField] Slider localUserAudio;
    [SerializeField] Slider remoteUserAudio;

    private bool isLocalSelfMuted = false;

    
    /// <summary>
    /// To be used in the On End Edit(string) event in a InputField
    /// </summary>
    public void UpdateRemotePlayerAudioText()
    {
        remotePlayerAudioText.text = remotePlayerNameInput.text;
    }


    #region Main Methods


    public void Login()
    {
        VivoxLoginUser(userNameInput.text);
    }

    public void LoginProduction()
    {
        VivoxLoginUserProduction(userNameInput.text);
    }

    public void LogOut()
    {
        VivoxLogoutSession();
    }

    public void JoinChannel()
    {
        VivoxJoinChannel(channelNameInput.text, true, true, true);
    }

    public void JoinChannelEcho()
    {
        VivoxJoinChannelEcho(channelNameInput.text, true, true, true);
    }

    public void JoinChannel3DPositional()
    {
        // refer to Vivox Documentation for AudioFadeModel
        VivoxJoin3DPositional(channelNameInput.text, true, true, false, 32, 1, 1, AudioFadeModel.InverseByDistance);
    }

    public void JoinChannelProduction()
    {
        VivoxJoinChannelProductionNonPositional(channelNameInput.text, true, true, true);
    }

    public void JoinChannelProductionEcho()
    {
        VivoxJoinChannelProductionEcho(channelNameInput.text, true, true, true);
    }

    public void JoinChannelProduction3DPositional()
    {
        // There is an optional parameter for Channel3DProperties
        // Channel3DProperties are settings for how sound should be heard in a 3D channel
        // Refer to Official Vivox Documentation for more understanding
        VivoxJoinChannelProduction3DPositional(channelNameInput.text, true, true, true, new Channel3DProperties(1, 1, 1, AudioFadeModel.InverseByDistance));
    }

    public void JoinChannelProduction3DMMO()
    {
        // Recommended to implement your own version of this method. This is more of an example method
        // This method appends the serverRegion and squadName to the channelName in this format
        // "serverRegion.channelName-squadname"
        // If you use this method make sure to remember to keep track of the channel name in this new format

        VivoxJoinChannelProduction3DPositionalMMO(true, true, true, serverRegion, channelNameInput.text, squadName,
            new Channel3DProperties(10, 10, 1, AudioFadeModel.InverseByDistance));
    }

    public void LeaveChannel()
    {
        VivoxLeaveChannel(channelNameInput.text);
    }

    public void ToggleVoiceInChannel()
    {
        VivoxToggleVoiceInChannel(channelNameInput.text, toggleVoiceInChannel.isOn);
    }

    public void ToggleTextInChannel()
    {
        VivoxToggleTextInChannel(channelNameInput.text, toggleTextInChannel.isOn);
    }


    public void ToggleMuteSelf()
    {
        if (isLocalSelfMuted)
        {
            isLocalSelfMuted = false;
        }
        else
        {
            isLocalSelfMuted = true;
        }
        VivoxToggleMuteSelf(isLocalSelfMuted);
    }

    public void ToggleMuteRemoteUser()
    {
        VivoxToggleMuteUser(remotePlayerNameInput.text, channelNameInput.text);
    }

    public void AdjustVolumeForLocalUser()
    {
        VivoxAdjustLocalUserVoiceVolume((int)localUserAudio.value);
    }

    public void AdjustRemoteUserVolume()
    {
        VivoxAdjustRemoteUserVolume(remotePlayerNameInput.text, channelNameInput.text, remoteUserAudio.value);
    }




    public void TTSSpeakLocal()
    {
        VivoxTTSSpeakMsg(messageInput.text, TTSDestination.LocalPlayback);
    }

    public void TTSSpeakQueuedLocal()
    {
        VivoxTTSSpeakMsg(messageInput.text, TTSDestination.QueuedLocalPlayback);
    }

    public void TTSSpeakRemote()
    {
        VivoxTTSSpeakMsg(messageInput.text, TTSDestination.RemoteTransmission);
    }

    public void TTSSpeakRemoteAndLocal()
    {
        VivoxTTSSpeakMsg(messageInput.text, TTSDestination.RemoteTransmissionWithLocalPlayback);
    }

    public void TTSSpeakQueuedRemote()
    {
        VivoxTTSSpeakMsg(messageInput.text, TTSDestination.QueuedRemoteTransmission);
    }

    public void TTSSpeakQueuedRemoteAndLocal()
    {
        VivoxTTSSpeakMsg(messageInput.text, TTSDestination.QueuedRemoteTransmissionWithLocalPlayback);
    }

    public void TTSSpeakScreenReader()
    {
        VivoxTTSSpeakMsg(messageInput.text, TTSDestination.ScreenReader);
    }





    public void SendChannelMsg()
    {
        VivoxSendChannelMessage(channelNameInput.text, messageInput.text);
    }

    public void SendDirectMsg()
    {
        VivoxSendDirectMessage(remotePlayerNameInput.text, messageInput.text);
    }


    public void SwitchScenes()
    {
        SceneManager.LoadScene(Convert.ToInt32(switchScenesInput.text));
    }


    #endregion




    #region Vivox Event Callbacks That can be Overriden



    public override void OnLoggingIn(ILoginSession loginSession)
    {
        base.OnLoggingIn(loginSession);
    }

    public override void OnLoggedIn(ILoginSession loginSession)
    {
        base.OnLoggedIn(loginSession);
        messageText.text += $"\nLogged In as {loginSession.LoginSessionId.DisplayName}";
    }

    public override void OnLoggingOut(ILoginSession loginSession)
    {
        base.OnLoggingOut(loginSession);
    }

    public override void OnLoggedOut(ILoginSession loginSession)
    {
        base.OnLoggedOut(loginSession);
        messageText.text += $"\nLogged out {loginSession.LoginSessionId.DisplayName}";
        this.DebugLog("Logged Out");
    }

    public override void OnChannelConnecting(IChannelSession channelSession)
    {
        base.OnChannelConnecting(channelSession);
    }

    public override void OnChannelConnected(IChannelSession channelSession)
    {
        base.OnChannelConnected(channelSession);
        messageText.text += $"\nChannel {channelSession.Channel.Name} has connected : {channelSession.Channel.Type}";
    }

    public override void OnChannelDisconnecting(IChannelSession channelSession)
    {
        base.OnChannelDisconnecting(channelSession);
    }

    public override void OnChannelDisconnected(IChannelSession channelSession)
    {
        base.OnChannelDisconnected(channelSession);
        messageText.text += $"\nChannel {channelSession.Channel.Name} has disconnected : {channelSession.Channel.Type}";
    }




    public override void OnVoiceConnecting(IChannelSession channelSession)
    {
        base.OnVoiceConnecting(channelSession);
    }

    public override void OnVoiceConnected(IChannelSession channelSession)
    {
        base.OnVoiceConnected(channelSession);
        messageText.text += $"\nVoice Channel {channelSession.Channel.Name} has connected : {channelSession.Channel.Type}";
    }

    public override void OnVoiceDisconnecting(IChannelSession channelSession)
    {
        base.OnVoiceDisconnecting(channelSession);
    }

    public override void OnVoiceDisconnected(IChannelSession channelSession)
    {
        base.OnVoiceDisconnected(channelSession);
        messageText.text += $"\nVoice Channel {channelSession.Channel.Name} has disconnected : {channelSession.Channel.Type}";
    }




    public override void OnTextChannelConnecting(IChannelSession channelSession)
    {
        base.OnTextChannelConnecting(channelSession);
    }

    public override void OnTextChannelConnected(IChannelSession channelSession)
    {
        base.OnTextChannelConnected(channelSession);
        messageText.text += $"\nTextChannel {channelSession.Channel.Name} has connected : {channelSession.Channel.Type}";
    }

    public override void OnTextChannelDisconnecting(IChannelSession channelSession)
    {
        base.OnTextChannelDisconnecting(channelSession);
    }

    public override void OnTextChannelDisconnected(IChannelSession channelSession)
    {
        base.OnTextChannelDisconnected(channelSession);
        messageText.text += $"\nText Channel {channelSession.Channel.Name} has connected : {channelSession.Channel.Type}";
    }




    public override void OnParticipantAdded(IParticipant participant)
    {
        base.OnParticipantAdded(participant);
        messageText.text += $"\n{participant.Account.DisplayName} has joined {participant.ParentChannelSession.Channel.Name}";
    }

    public override void OnParticipantRemoved(IParticipant participant)
    {
        base.OnParticipantRemoved(participant);
        messageText.text += $"\n{participant.Account.DisplayName} has left {participant.ParentChannelSession.Channel.Name}";
    }

    public override void OnParticipantValueUpdated(IParticipant participant)
    {
        base.OnParticipantValueUpdated(participant);
    }




    public override void OnChannelMessageRecieved(IChannelTextMessage textMessage)
    {
        base.OnChannelMessageRecieved(textMessage);
        messageText.text += $"\n{textMessage.ReceivedTime} : From {textMessage.Sender.DisplayName} : {textMessage.Message}";
    }




    public override void OnDirectMessageRecieved(IDirectedTextMessage directedTextMessage)
    {
        base.OnDirectMessageRecieved(directedTextMessage);
        messageText.text += $"\n{directedTextMessage.ReceivedTime} : From {directedTextMessage.Sender.DisplayName} : {directedTextMessage.Message}";
    }

    public override void OnDirectMessageFailed(IFailedDirectedTextMessage failedMessage)
    {
        base.OnDirectMessageFailed(failedMessage);
        messageText.text += $"\nMessage failed from {failedMessage.Sender.DisplayName} : RequestID - {failedMessage.RequestId} : Status Code - {failedMessage.StatusCode}";
    }





    public override void OnUserMuted(IParticipant participant)
    {
        base.OnUserMuted(participant);
        messageText.text += $"\n{participant.Account.DisplayName} has been muted";
    }

    public override void OnUserUnmuted(IParticipant participant)
    {
        base.OnUserUnmuted(participant);
        messageText.text += $"\n{participant.Account.DisplayName} has been unmuted";
    }

    public override void OnUserSpeaking(IParticipant participant)
    {
        base.OnUserSpeaking(participant);
        messageText.text += $"\n{participant.Account.DisplayName} is speaking";
    }

    public override void OnUserNotSpeaking(IParticipant participant)
    {
        base.OnUserNotSpeaking(participant);
        messageText.text += $"\n{participant.Account.DisplayName} is not speaking";
    }

    public override void OnUserTyping(bool isTyping)
    {
        base.OnUserTyping(isTyping);
    }





    public override void OnTTSMessageAdded(ITTSMessageQueueEventArgs ttsArgs)
    {
        base.OnTTSMessageAdded(ttsArgs);
        messageText.text += $"\nTTS message added : Destination - {ttsArgs.Message.Destination} : Number Of Consumers - {ttsArgs.Message.NumConsumers} : Duration - {ttsArgs.Message.Duration} : State - {ttsArgs.Message.State} : Voice - {ttsArgs.Message.Voice} : TTS Message - {ttsArgs.Message.Text}";
    }

    public override void OnTTSMessageRemoved(ITTSMessageQueueEventArgs ttsArgs)
    {
        base.OnTTSMessageRemoved(ttsArgs);
        messageText.text += $"\nTTS message removed : Destination - {ttsArgs.Message.Destination} : Number Of Consumers - {ttsArgs.Message.NumConsumers} : Duration - {ttsArgs.Message.Duration} : State - {ttsArgs.Message.State} : Voice - {ttsArgs.Message.Voice} : TTS Message - {ttsArgs.Message.Text}";
    }

    public override void OnTTSMessageUpdated(ITTSMessageQueueEventArgs ttsArgs)
    {
        base.OnTTSMessageUpdated(ttsArgs);
        messageText.text += $"\nTTS message updated : Destination - {ttsArgs.Message.Destination} : Number Of Consumers - {ttsArgs.Message.NumConsumers} : Duration - {ttsArgs.Message.Duration} : State - {ttsArgs.Message.State} : Voice - {ttsArgs.Message.Voice} : TTS Message - {ttsArgs.Message.Text}";
    }




#endregion



}
