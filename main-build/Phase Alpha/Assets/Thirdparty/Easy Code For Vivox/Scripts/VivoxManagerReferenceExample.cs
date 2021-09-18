using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VivoxUnity;

public class VivoxManagerReferenceExample : MonoBehaviour
{
    [SerializeField] VivoxManager vivoxManager;

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

    private void Awake()
    {
        SubscribeToVivoxEvents();
    }


    public void SubscribeToVivoxEvents()
    {
        // Subscribe to Login Related Events
        VivoxLogin.VivoxLoggingIn += OnLoggingIn;
        VivoxLogin.VivoxLoggedIn += OnLoggedIn;
        VivoxLogin.VivoxLoggingOut += OnLoggingOut;
        VivoxLogin.VivoxLoggedOut += OnLoggedOut;

        // Subscribe to Channel Related Events
        VivoxChannelVoice.VivoxVoiceChannelConnecting += OnVoiceConnecting;
        VivoxChannelVoice.VivoxVoiceChannelConnected += OnVoiceConnected;
        VivoxChannelVoice.VivoxVoiceChannelDisconnecting += OnVoiceDisconnecting;
        VivoxChannelVoice.VivoxVoiceChannelDisconnected += OnVoiceDisconnected;

        // Subscribe to Channel Related Events
        VivoxChannelText.VivoxTextChannelConnecting += OnTextChannelConnecting;
        VivoxChannelText.VivoxTextChannelConnected += OnTextChannelConnected;
        VivoxChannelText.VivoxTextChannelDisconnecting += OnTextChannelDisconnecting;
        VivoxChannelText.VivoxTextChannelDisconnected += OnTextChannelDisconnected;

        // Subscribe to Channel Related Events
        VivoxChannel.VivoxChannelConnecting += OnChannelConnecting;
        VivoxChannel.VivoxChannelConnected += OnChannelConnected;
        VivoxChannel.VivoxChannelDisconnecting += OnChannelDisconnecting;
        VivoxChannel.VivoxChannelDisconnected += OnChannelDisconnected;

        // Subscribe to Channel Message Related Events
        VivoxMessages.VivoxChannelMessageRecieved += OnChannelMessageRecieved;

        // Subscribe to Direct Message Related Events
        VivoxMessages.VivoxDirectMessageRecieved += OnDirectMessageRecieved;
        VivoxMessages.VivoxDirectMessageFailed += OnDirectMessageFailed;

        // User/Participant related Events
        VivoxParticipants.VivoxParticipantAdded += OnParticipantAdded;
        VivoxParticipants.VivoxParticipantRemoved += OnParticipantRemoved;
        VivoxParticipants.VivoxParticipantValueUpdated += OnParticipantValueUpdated;

        // Subscribe to User/Participant Audio Related Events
        VivoxParticipants.VivoxUserMuted += OnUserMuted;
        VivoxParticipants.VivoxUserUnmuted += OnUserUnmuted;
        VivoxParticipants.VivoxUserSpeaking += OnUserSpeaking;
        VivoxParticipants.VivoxUserNotSpeaking += OnUserNotSpeaking;

        // Subscribe to Text-To-Speech related events
        VivoxTextToSpeech.VivoxTTSMessageAdded += OnTTSMessageAdded;
        VivoxTextToSpeech.VivoxTTSMessageRemoved += OnTTSMessageRemoved;
        VivoxTextToSpeech.VivoxTTSMessageUpdated += OnTTSMessageUpdated;

        // Subscribe to Typing related Event - Custom Event - Vivox's implementation doesnt work. This is a local event
        // Event info is not sent over the network but can be implemented by sending messages with/or without hidden messages
        vivoxManager.VivoxIsUserTyping += OnUserTyping;
    }

    public void UnsubscribeToVivoxEvents()
    {
        // Unsubscribe to Login Related Events
        VivoxLogin.VivoxLoggingIn -= OnLoggingIn;
        VivoxLogin.VivoxLoggedIn -= OnLoggedIn;
        VivoxLogin.VivoxLoggingOut -= OnLoggingOut;
        VivoxLogin.VivoxLoggedOut -= OnLoggedOut;

        // Unsubscribe to Channel Related Events
        VivoxChannelVoice.VivoxVoiceChannelConnecting -= OnVoiceConnecting;
        VivoxChannelVoice.VivoxVoiceChannelConnected -= OnVoiceConnected;
        VivoxChannelVoice.VivoxVoiceChannelDisconnecting -= OnVoiceDisconnecting;
        VivoxChannelVoice.VivoxVoiceChannelDisconnected -= OnVoiceDisconnected;

        // Unsubscribe to Channel Related Events
        VivoxChannelText.VivoxTextChannelConnecting -= OnTextChannelConnecting;
        VivoxChannelText.VivoxTextChannelConnected -= OnTextChannelConnected;
        VivoxChannelText.VivoxTextChannelDisconnecting -= OnTextChannelDisconnecting;
        VivoxChannelText.VivoxTextChannelDisconnected -= OnTextChannelDisconnected;

        // Subscribe to Channel Related Events
        VivoxChannel.VivoxChannelConnecting -= OnChannelConnecting;
        VivoxChannel.VivoxChannelConnected -= OnChannelConnected;
        VivoxChannel.VivoxChannelDisconnecting -= OnChannelDisconnecting;
        VivoxChannel.VivoxChannelDisconnected -= OnChannelDisconnected;

        // Unsubscribe to Channel Message Related Events
        VivoxMessages.VivoxChannelMessageRecieved -= OnChannelMessageRecieved;

        // Unsubscribe to Direct Message Related Events
        VivoxMessages.VivoxDirectMessageRecieved -= OnDirectMessageRecieved;
        VivoxMessages.VivoxDirectMessageFailed -= OnDirectMessageFailed;

        // User/Participant related Events
        VivoxParticipants.VivoxParticipantAdded -= OnParticipantAdded;
        VivoxParticipants.VivoxParticipantRemoved -= OnParticipantRemoved;
        VivoxParticipants.VivoxParticipantValueUpdated -= OnParticipantValueUpdated;

        // Unsubscribe to User/Participant Audio Related Events
        VivoxParticipants.VivoxUserMuted -= OnUserMuted;
        VivoxParticipants.VivoxUserUnmuted -= OnUserUnmuted;
        VivoxParticipants.VivoxUserSpeaking -= OnUserSpeaking;
        VivoxParticipants.VivoxUserNotSpeaking -= OnUserNotSpeaking;

        // Unsubscribe to Text-To-Speech related events
        VivoxTextToSpeech.VivoxTTSMessageAdded -= OnTTSMessageAdded;
        VivoxTextToSpeech.VivoxTTSMessageRemoved -= OnTTSMessageRemoved;
        VivoxTextToSpeech.VivoxTTSMessageUpdated -= OnTTSMessageUpdated;

        // Unsubscribe to Typing related Event - Custom Event - Vivox's implementation doesnt work. This is a local event
        // Event info is not sent over the network but can be implemented by sending messages with/or without hidden messages
        vivoxManager.VivoxIsUserTyping -= OnUserTyping;
    }




    public void UpdateRemotePlayerAudioText()
    {
        remotePlayerAudioText.text = remotePlayerNameInput.text;
    }


    #region Main Methods


    public void Login()
    {
        vivoxManager.VivoxLoginUser(userNameInput.text);
    }   
    
    public void LoginProduction()
    {
        vivoxManager.VivoxLoginUserProduction(userNameInput.text);
    }

    public void LogOut()
    {
        vivoxManager.VivoxLogoutSession();
    }




    public void JoinChannel()
    {
        vivoxManager.VivoxJoinChannel(channelNameInput.text, true, true, true);
    }

    public void JoinChannelEcho()
    {
        vivoxManager.VivoxJoinChannelEcho(channelNameInput.text, true, true, true);
    }

    public void JoinChannel3DPositional()
    {
        vivoxManager.VivoxJoin3DPositional(channelNameInput.text, true, true, false, 32, 1, 1, AudioFadeModel.InverseByDistance);
    }

    public void JoinChannelProduction()
    {
        vivoxManager.VivoxJoinChannelProductionNonPositional(channelNameInput.text, true, true, true);
    }

    public void JoinChannelProductionEcho()
    {
        vivoxManager.VivoxJoinChannelProductionEcho(channelNameInput.text, true, true, true);
    }

    public void JoinChannelProduction3DPositional()
    {
        vivoxManager.VivoxJoinChannelProduction3DPositional(channelNameInput.text, true, true, true);
    }

    public void JoinChannelProduction3DPositionalMMO()
    {
        // Recommended to implement your own version of this method. This is more of an example method
        // This method appends the serverRegion and squadName to the channelName in this format
        // "serverRegion.channelName-squadname"
        // If you use this method make sure to remember to keep track of the channel name in this new format

        vivoxManager.VivoxJoinChannelProduction3DPositionalMMO(true, true, true, serverRegion, channelNameInput.text, squadName, 
            new Channel3DProperties(1, 1, 1, AudioFadeModel.InverseByDistance));
    }

    public void LeaveChannel()
    {
        vivoxManager.VivoxLeaveChannel(channelNameInput.text);
    }

    public void ToggleVoiceInChannel()
    {
        vivoxManager.VivoxToggleVoiceInChannel(channelNameInput.text, toggleVoiceInChannel.isOn);
    }

    public void ToggleTextInChannel()
    {
        vivoxManager.VivoxToggleTextInChannel(channelNameInput.text, toggleTextInChannel.isOn);
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
        vivoxManager.VivoxToggleMuteSelf(isLocalSelfMuted);
    }

    public void ToggleMuteRemoteUser()
    {
        vivoxManager.VivoxToggleMuteUser(remotePlayerNameInput.text, channelNameInput.text);
    }

    public void AdjustVolumeForLocalUser()
    {
        vivoxManager.VivoxAdjustLocalUserVoiceVolume((int)localUserAudio.value);
    }

    public void AdjustRemoteUserVolume()
    {
        vivoxManager.VivoxAdjustRemoteUserVolume(remotePlayerNameInput.text, channelNameInput.text, remoteUserAudio.value);
    }




    public void TTSSpeakLocal()
    {
        vivoxManager.VivoxTTSSpeakMsg(messageInput.text, TTSDestination.LocalPlayback);
    }  

    public void TTSSpeakQueuedLocal()
    {
        vivoxManager.VivoxTTSSpeakMsg(messageInput.text, TTSDestination.QueuedLocalPlayback);
    }  
    
    public void TTSSpeakRemote()
    {
        vivoxManager.VivoxTTSSpeakMsg(messageInput.text, TTSDestination.RemoteTransmission);
    }

    public void TTSSpeakRemoteAndLocal()
    {
        vivoxManager.VivoxTTSSpeakMsg(messageInput.text, TTSDestination.RemoteTransmissionWithLocalPlayback);
    }

    public void TTSSpeakQueuedRemote()
    {
        vivoxManager.VivoxTTSSpeakMsg(messageInput.text, TTSDestination.QueuedRemoteTransmission);
    }
    
    public void TTSSpeakQueuedRemoteAndLocal()
    {
        vivoxManager.VivoxTTSSpeakMsg(messageInput.text, TTSDestination.QueuedRemoteTransmissionWithLocalPlayback);
    }
    
    public void TTSSpeakScreenReader()
    {
        vivoxManager.VivoxTTSSpeakMsg(messageInput.text, TTSDestination.ScreenReader);
    }





    public void SendChannelMsg()
    {
        vivoxManager.VivoxSendChannelMessage(channelNameInput.text, messageInput.text);
    }

    public void SendDirectMsg()
    {
        vivoxManager.VivoxSendDirectMessage(remotePlayerNameInput.text, messageInput.text);
    }





    public void SwitchScenes()
    {
        SceneManager.LoadScene(Convert.ToInt32(switchScenesInput.text));
    }


    #endregion



    #region Vivox Callbacks


    #region Audio / Text / Channel Callbacks


    public void OnVoiceConnecting(IChannelSession channelSession)
    {

    }

    public void OnVoiceConnected(IChannelSession channelSession)
    {
        messageText.text += $"\nChannel {channelSession.Channel.Name} - Voice has connected";
    }

    public void OnVoiceDisconnecting(IChannelSession channelSession)
    {

    }

    public void OnVoiceDisconnected(IChannelSession channelSession)
    {
        messageText.text += $"\nChannel {channelSession.Channel.Name} - Voice has disconnected";
    }




    public void OnTextChannelConnecting(IChannelSession channelSession)
    {

    }

    public void OnTextChannelConnected(IChannelSession channelSession)
    {
        messageText.text += $"\nChannel {channelSession.Channel.Name} - Text has connected";
    }

    public void OnTextChannelDisconnecting(IChannelSession channelSession)
    {

    }

    public void OnTextChannelDisconnected(IChannelSession channelSession)
    {
        messageText.text += $"\nChannel {channelSession.Channel.Name} - Text has disconnected";
    }



    public void OnChannelConnecting(IChannelSession channelSession)
    {

    }

    public void OnChannelConnected(IChannelSession channelSession)
    {
        messageText.text += $"\nChannel {channelSession.Channel.Name} has connected : Type {channelSession.Channel.Type}";
    }

    public void OnChannelDisconnecting(IChannelSession channelSession)
    {

    }

    public void OnChannelDisconnected(IChannelSession channelSession)
    {
        messageText.text += $"\nChannel {channelSession.Channel.Name} has disconnected : Type {channelSession.Channel.Type}";
    }



    #endregion


    #region User Callbacks


    public void OnParticipantAdded(IParticipant participant)
    {
        messageText.text += $"\n{participant.Account.DisplayName} has joined";
    }

    public void OnParticipantRemoved(IParticipant participant)
    {
        messageText.text += $"\n{participant.Account.DisplayName} has left";
    }

    public void OnParticipantValueUpdated(IParticipant participant)
    {

    }

    public void OnUserMuted(IParticipant participant)
    {
        messageText.text += $"\n{participant.Account.DisplayName} has been muted";
    }

    public void OnUserUnmuted(IParticipant participant)
    {
        messageText.text += $"\n{participant.Account.DisplayName} has been unmuted";
    }

    public void OnUserSpeaking(IParticipant participant)
    {
       // this callback fires very often so this is commented out so it doesnt spam your Debug Console
       // messageText.text += $"\n{participant.Account.DisplayName} is Speaking";
    }

    public void OnUserNotSpeaking(IParticipant participant)
    {
        // this callback fires very often so this is commented out so it doesnt spam your Debug Console
        //  messageText.text += $"\n{participant.Account.DisplayName} is not Speaking";
    }


    #endregion


    #region Message Callbacks


    public void OnChannelMessageRecieved(IChannelTextMessage textMessage)
    {
        messageText.text += $"\n{textMessage.ReceivedTime} : {textMessage.Sender.DisplayName} : {textMessage.Message}";
    }

    public void OnDirectMessageRecieved(IDirectedTextMessage directedTextMessage)
    {
        messageText.text += $"\n{directedTextMessage.ReceivedTime} : {directedTextMessage.Sender.DisplayName} : {directedTextMessage.Message}";
    }

    public void OnDirectMessageFailed(IFailedDirectedTextMessage failedMessage)
    {
        messageText.text += $"\nDirect Message Failed to Send From {failedMessage.Sender.DisplayName} : Request ID - {failedMessage.RequestId} : Status Code - {failedMessage.StatusCode}";
    }


    public void OnUserTyping(bool isTyping)
    {
        if (isTyping)
        {
            // this callback fires very often so this is commented out so it doesnt spam your Debug Console
            // messageText.text += $"";
        }
        else
        {
            // this callback fires very often so this is commented out so it doesnt spam your Debug Console
            // messageText.text += $"";
        }

    }


    #endregion


    #region Login / Logout Callbacks


    public void OnLoggingIn(ILoginSession loginSession)
    {

    }

    public void OnLoggedIn(ILoginSession loginSession)
    {
        messageText.text += $"\n{loginSession.LoginSessionId.DisplayName} has Logged In";
    }

    public void OnLoggingOut(ILoginSession loginSession)
    {
        messageText.text += $"\n{loginSession.LoginSessionId.DisplayName} has Logged Out";
    }

    public void OnLoggedOut(ILoginSession loginSession)
    {

    }


    #endregion


    #region Text-to-Speech Callbacks

    public void OnTTSMessageAdded(ITTSMessageQueueEventArgs ttsArgs)
    {
        messageText.text += $"\nAdded TTS : Voice - {ttsArgs.Message.Voice} : Destination - {ttsArgs.Message.Destination} : Duration - {ttsArgs.Message.Duration} : Consumers - {ttsArgs.Message.NumConsumers} : Message - {ttsArgs.Message.Text}";
    }

    public void OnTTSMessageRemoved(ITTSMessageQueueEventArgs ttsArgs)
    {
        messageText.text += $"\nRemoved Voice - {ttsArgs.Message.Voice} : Destination - {ttsArgs.Message.Destination} : Duration - {ttsArgs.Message.Duration} : Consumers - {ttsArgs.Message.NumConsumers} : Message - {ttsArgs.Message.Text}";
    }

    public void OnTTSMessageUpdated(ITTSMessageQueueEventArgs ttsArgs)
    {

    }


    #endregion



    #region Subscription / Presence Callbacks
    
    // The Presence feature in Vivox does not work as intended so these callback methods
    // are commented out until this feature is properly working
    // feel free to test or modify



    //protected virtual void OnAddAllowedSubscription(AccountId accountId)
    //{

    //}

    //protected virtual void OnRemoveAllowedSubscription(AccountId accountId)
    //{

    //}

    //protected virtual void OnAddBlockedSubscription(AccountId accountId)
    //{

    //}

    //protected virtual void OnRemoveBlockedSubscription(AccountId accountId)
    //{

    //}

    //protected virtual void OnAddPresenceSubscription(AccountId accountId)
    //{

    //}

    //protected virtual void OnRemovePresenceSubscription(AccountId accountId)
    //{

    //}

    //protected virtual void OnUpdatePresenceSubscription(ValueEventArg<AccountId, IPresenceSubscription> presence)
    //{

    //}

    #endregion


    #endregion






}
