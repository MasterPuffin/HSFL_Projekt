using System.Reflection;
using System.Text;
using VivoxUnity;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class VivoxManager : VivoxBehaviour
{
    [Tooltip("Server URI Address(API End-Point) From Vivox Developer Portal")]
    [SerializeField] private string vivoxserverURI;
    [Tooltip("Domain From Vivox Developer Portal")]
    [SerializeField] private string vivoxDomain;
    [Tooltip("Issuer From Vivox Developer Portal")]
    [SerializeField] private string vivoxTokenIssuer;
    [Tooltip("Secret Key From Vivox Developer Portal")]
    [SerializeField] private string vivoxTokenKey;

    private void OnApplicationQuit()
    {
        UnsubscribeToVivoxEvents();
        mainClient.Uninitialize();
    }

    private void Awake()
    {
        InitializeClient();
        SubscribeToVivoxEvents();
        StartCoroutine(CheckIsTyping());
        StartCoroutine(TypingTimer(0.3f));
    }


    public void SubscribeToVivoxEvents()
    {
        VivoxLogin.VivoxLoggingIn += OnLoggingIn;
        VivoxLogin.VivoxLoggedIn += OnLoggedIn;
        VivoxLogin.VivoxLoggingOut += OnLoggingOut;
        VivoxLogin.VivoxLoggedOut += OnLoggedOut;

        VivoxChannelVoice.VivoxVoiceChannelConnecting += OnVoiceConnecting;
        VivoxChannelVoice.VivoxVoiceChannelConnected += OnVoiceConnected;
        VivoxChannelVoice.VivoxVoiceChannelDisconnecting += OnVoiceDisconnecting;
        VivoxChannelVoice.VivoxVoiceChannelDisconnected += OnVoiceDisconnected;

        VivoxChannelText.VivoxTextChannelConnecting += OnTextChannelConnecting;
        VivoxChannelText.VivoxTextChannelConnected += OnTextChannelConnected;
        VivoxChannelText.VivoxTextChannelDisconnecting += OnTextChannelDisconnecting;
        VivoxChannelText.VivoxTextChannelDisconnected += OnTextChannelDisconnected;

        VivoxChannel.VivoxChannelConnecting += OnChannelConnecting;
        VivoxChannel.VivoxChannelConnected += OnChannelConnected;
        VivoxChannel.VivoxChannelDisconnecting += OnChannelDisconnecting;
        VivoxChannel.VivoxChannelDisconnected += OnChannelDisconnected;

        VivoxMessages.VivoxChannelMessageRecieved += OnChannelMessageRecieved;

        VivoxMessages.VivoxDirectMessageRecieved += OnDirectMessageRecieved;
        VivoxMessages.VivoxDirectMessageFailed += OnDirectMessageFailed;

        VivoxParticipants.VivoxParticipantAdded += OnParticipantAdded;
        VivoxParticipants.VivoxParticipantRemoved += OnParticipantRemoved;
        VivoxParticipants.VivoxParticipantValueUpdated += OnParticipantValueUpdated;

        VivoxParticipants.VivoxUserMuted += OnUserMuted;
        VivoxParticipants.VivoxUserUnmuted += OnUserUnmuted;
        VivoxParticipants.VivoxUserSpeaking += OnUserSpeaking;
        VivoxParticipants.VivoxUserNotSpeaking += OnUserNotSpeaking;

        VivoxTextToSpeech.VivoxTTSMessageAdded += OnTTSMessageAdded;
        VivoxTextToSpeech.VivoxTTSMessageRemoved += OnTTSMessageRemoved;
        VivoxTextToSpeech.VivoxTTSMessageUpdated += OnTTSMessageUpdated;

        VivoxIsUserTyping += OnUserTyping;

    }

    public void UnsubscribeToVivoxEvents()
    { 
        VivoxLogin.VivoxLoggingIn -= OnLoggingIn;
        VivoxLogin.VivoxLoggedIn -= OnLoggedIn;
        VivoxLogin.VivoxLoggingOut -= OnLoggingOut;
        VivoxLogin.VivoxLoggedOut -= OnLoggedOut;

        VivoxChannelVoice.VivoxVoiceChannelConnecting -= OnVoiceConnecting;
        VivoxChannelVoice.VivoxVoiceChannelConnected -= OnVoiceConnected;
        VivoxChannelVoice.VivoxVoiceChannelDisconnecting -= OnVoiceDisconnecting;
        VivoxChannelVoice.VivoxVoiceChannelDisconnected -= OnVoiceDisconnected;

        VivoxChannelText.VivoxTextChannelConnecting -= OnTextChannelConnecting;
        VivoxChannelText.VivoxTextChannelConnected -= OnTextChannelConnected;
        VivoxChannelText.VivoxTextChannelDisconnecting -= OnTextChannelDisconnecting;
        VivoxChannelText.VivoxTextChannelDisconnected -= OnTextChannelDisconnected;

        VivoxChannel.VivoxChannelConnecting -= OnChannelConnecting;
        VivoxChannel.VivoxChannelConnected -= OnChannelConnected;
        VivoxChannel.VivoxChannelDisconnecting -= OnChannelDisconnecting;
        VivoxChannel.VivoxChannelDisconnected -= OnChannelDisconnected;

        VivoxMessages.VivoxChannelMessageRecieved -= OnChannelMessageRecieved;
       
        VivoxMessages.VivoxDirectMessageRecieved -= OnDirectMessageRecieved;
        VivoxMessages.VivoxDirectMessageFailed -= OnDirectMessageFailed;

        VivoxParticipants.VivoxParticipantAdded -= OnParticipantAdded;
        VivoxParticipants.VivoxParticipantRemoved -= OnParticipantRemoved;
        VivoxParticipants.VivoxParticipantValueUpdated -= OnParticipantValueUpdated;

        VivoxParticipants.VivoxUserMuted -= OnUserMuted;
        VivoxParticipants.VivoxUserUnmuted -= OnUserUnmuted;
        VivoxParticipants.VivoxUserSpeaking -= OnUserSpeaking;
        VivoxParticipants.VivoxUserNotSpeaking -= OnUserNotSpeaking;

        VivoxTextToSpeech.VivoxTTSMessageAdded -= OnTTSMessageAdded;
        VivoxTextToSpeech.VivoxTTSMessageRemoved -= OnTTSMessageRemoved;
        VivoxTextToSpeech.VivoxTTSMessageUpdated -= OnTTSMessageUpdated;

        VivoxIsUserTyping -= OnUserTyping;
    }

    private IEnumerator CheckIsTyping()
    {
        //Bugfix for new input system
        IsTyping = false;
        yield return new WaitUntil(() => IsTyping);
        
        //yield return new WaitUntil(() => Input.anyKeyDown);
        TypingCountdown += .25f;
        if (IsTyping)
        {
            IsTyping = false;
        }
        else
        {
            IsTyping = true;
            OnVivoxIsUserTyping(true);
        }
        StartCoroutine(CheckIsTyping());
    }

    private IEnumerator TypingTimer(float timeToAdd)   // Credit To : Programmer : https://stackoverflow.com/a/38473703/14465032
    {
        yield return new WaitUntil(() => TypingCountdown > 0);
        while (TypingCountdown > 0)
        {
            TypingCountdown -= Time.deltaTime;
            yield return null;
        }
        OnVivoxIsUserTyping(false);
        StartCoroutine(TypingTimer(timeToAdd));
    }


    #region Main Vivox Methods For Implementing In UI or call from code


    public void VivoxRequestAndroidMicrophoneAccess(ILoginSession loginSession)
    {
        RequestAndroidMicPermission();
    }

    //public void VivoxRequestIOSMicrophoneAccess(ILoginSession loginSession)
    //{
    //    // Refer to Vivox Documentation on how to implement this method. Currently a work in progress.NOT SURE IF IT WORKS
    //    // make sure you change the info list refer to vivoc documentation for this to work
    //    // Make sure NSCameraUsageDescription and NSMicrophoneUsageDescription
    //    // are in the Info.plist.
    //    Application.RequestUserAuthorization(UserAuthorization.Microphone);
    //}

    private bool FilterChannelAndUserName(string nameToFilter)
    {
        char[] allowedChars = new char[] { '0','1','2','3', '4', '5', '6', '7', '8', '9',
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n','o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I','J', 'K', 'L', 'M', 'N', 'O', 'P','Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '!', '(', ')', '+','-', '.', '=', '_', '~'};

        List<char> allowed = new List<char>(allowedChars);
        foreach (char c in nameToFilter)
        {
            if (!allowed.Contains(c))
            {
                OnSendLog(MethodBase.GetCurrentMethod(), $"Can't join channel, Channel name has invalid character '{c}'");
                return false;
            }
        }
        return true;
    }

    public void VivoxLoginUser(string username)
    {
        if (FilterChannelAndUserName(username))
        {
            if(serverUri == null)
            {
                serverUri = new Uri(vivoxserverURI);
                domain = vivoxDomain;
                tokenIssuer = vivoxTokenIssuer;
                tokenKey = vivoxTokenKey;
            }

            Login(username);
        }
        // todo see if this should be an option for users or always set
        mainLoginSession.SetTransmissionMode(TransmissionMode.All);
    }
    
    public void VivoxLoginUserProduction(string username)
    {
        if (FilterChannelAndUserName(username))
        {
            // if serverUri is not null then it has been setup thru playmaker or by directly accessing and setting VivoxBehaviour.serverUri;
            if (serverUri == null) 
            {
                serverUri = new Uri(vivoxserverURI);
                domain = vivoxDomain;
                tokenIssuer = vivoxTokenIssuer;
                tokenKey = vivoxTokenKey;
            }

            Login(username);
        }
        // todo see if this should be an option for users or always set
        mainLoginSession.SetTransmissionMode(TransmissionMode.All);
    }

    public void VivoxLogoutSession()
    {
        vivoxLogin.Logout(mainLoginSession);
    }

    public void VivoxJoinChannel(string channelName, bool includeVoice, bool includeText, bool switchToThisChannel)
    {
        if (FilterChannelAndUserName(channelName))
        {
            JoinChannel(channelName, includeVoice, includeText, switchToThisChannel);
        }
    }  

    public void VivoxJoinChannelEcho(string channelName, bool includeVoice, bool includeText, bool switchToThisChannel)
    {
        if (FilterChannelAndUserName(channelName))
        {
            JoinChannelEcho(channelName, includeVoice, includeText, switchToThisChannel);
        }
    }

    public void VivoxJoin3DPositional(string channelName, bool includeVoice, bool includeText, bool switchToThisChannel)
    {
        this.DebugLog($"{mainLoginSession.Presence.Status} {mainLoginSession.Presence.Message}");
        Channel3DProperties channel3DProperties = new Channel3DProperties();
        JoinChannel3DPositional(channelName, includeVoice, includeText, switchToThisChannel, channel3DProperties);
    }

    public void VivoxJoin3DPositional(string channelName, bool includeVoice, bool includeText, bool switchToThisChannel,
        int maxHearingDistance, int minHearingDistance, float voiceFadeOutOverDistance, AudioFadeModel audioFadeModel)
    {
        this.DebugLog($"{mainLoginSession.Presence.Status} {mainLoginSession.Presence.Message}");
        Channel3DProperties channel3DProperties = new Channel3DProperties(maxHearingDistance, minHearingDistance, voiceFadeOutOverDistance, audioFadeModel);
        JoinChannel3DPositional(channelName, includeVoice, includeText, switchToThisChannel, channel3DProperties);

    }

    public void VivoxJoinChannelProductionNonPositional(string channelName, bool includeVoice, bool includeText, bool switchToThisChannel, bool joinMuted = false)
    {
        if (FilterChannelAndUserName(channelName))
        {
            JoinChannelProduction(channelName, includeVoice, includeText, switchToThisChannel, joinMuted);
        }
    }  
    
    public void VivoxJoinChannelProductionNonPositionalMMO(string channelName, bool includeVoice, bool includeText, bool switchToThisChannel, bool joinMuted = false)
    {
        if (FilterChannelAndUserName(channelName))
        {
            JoinChannelProduction(channelName, includeVoice, includeText, switchToThisChannel, joinMuted);
        }
    }

    public void VivoxJoinChannelProductionEcho(string channelName, bool includeVoice, bool includeText, bool switchToThisChannel, bool joinMuted = false)
    {
        if (FilterChannelAndUserName(channelName))
        {
            JoinChannelProductionEcho(channelName, includeVoice, includeText, switchToThisChannel, joinMuted);
        }
    }

    public void VivoxJoinChannelProduction3DPositional(string channelName, bool includeVoice, bool includeText, bool switchToThisChannel,
        Channel3DProperties channel3DProperties = null, bool joinMuted = false)
    {
        if (FilterChannelAndUserName(channelName))
        {
            JoinChannelProduction3DPositional(channelName, includeVoice, includeText, switchToThisChannel, channel3DProperties, joinMuted);
        }
    }

    public void VivoxJoinChannelProduction3DPositionalMMO(bool includeVoice, bool includeText, bool switchToThisChannel,
    string region, string channelName, string squadName, Channel3DProperties channel3DProperties = null, bool joinMuted = false)
    {
        if (FilterChannelAndUserName(channelName))
        {
            JoinChannelProduction3DPositionalMMO(includeVoice, includeText, switchToThisChannel, region, channelName, squadName, channel3DProperties, joinMuted);
        }
    }

    public void VivoxLeaveChannel(string channelname)
    {
        // todo add multiple channel functionality
        LeaveChannel(channelname);
    }

    public void VivoxToggleVoiceInChannel(string channelName, bool toggleOn)
    {
        SetVoiceActiveInChannel(channelName, toggleOn);
    }

    public void VivoxToggleTextInChannel(string channelName, bool toggleOn)
    {
        SetTextActiveInChannel(channelName, toggleOn);
    }

    public void VivoxSendChannelMessage(string channelName, string msg, string commandNameSpace = null, string commandBody = null)
    {
        SendChannelMessage(channelName, msg, commandNameSpace, commandBody);
    }

    public void VivoxSendDirectMessage(string userToMsg, string msg, string commandNameSpace = null, string commandBody = null)
    {
        // todo check message arguements and alert front end
        SendDirectMessage(userToMsg, msg, commandNameSpace, commandBody);
    }

    public void VivoxToggleMuteSelf(bool mute)
    {
        ToggleMuteSelf(mute);
    }
    
    public void VivoxToggleMuteUser(string userName, string channelName)
    {
        ToggleMuteRemoteUser(userName, channelName);
    }

    // This method does not currently work because I am still figuring out how to successfully implement it. 
    // When this method is ok to use I will update this documentation.
    // I looks like this method isnt compatible with Vivox Unity but not 100% sure
    //public void VivoxMuteUserForAllProduction(string userName, string channelName, bool setMute)
    //{
    //    MuteUserForAllProduction(userName, channelName, setMute);
    //}

    public void VivoxAdjustLocalUserVoiceVolume(int volume)
    {
        AdjustLocalUserVolume(volume);
    }

    public void VivoxAdjustRemoteUserVolume(string userName, string channelName, float volume)
    {
        AdjustRemoteUserVolume(userName, channelName, volume);
    }


    // These methods are for using Presence within Vivox UNity SDK but the Presence feature 
    // is currently buggy and doesnt work like it should but when Unity/Vivox fixes this feature
    // these methods will be tested and implemented 
    // feel free to experiment
    // 
    //public void VivoxAddAllowedToTalkSubscription(string userName)
    //{
    //    AddAllowedUser(userName);
    //}
    //public void VivoxRemoveAllowedToTalkSubscription(string userName)
    //{
    //    RemoveAllowedUser(userName);
    //}
    //public void VivoxAddBlockedCantTalkSubscription(string userName)
    //{
    //    BlockUser(userName);
    //}
    //public void VivoxRemoveBlockedCantTalkSubscription(string userName)
    //{
    //    RemoveBlockedUser(userName);
    //}
    //public void VivoxAddFriendSubscription(string userName)
    //{
    //    AddFriend(userName);
    //}
    //public void VivoxRemoveFriendSubscription(string userName)
    //{
    //    RemoveFriend(userName);
    //}

    public void VivoxTTSSpeakMsgLocal(string msgToSpeak)
    {
        SpeakTTS(msgToSpeak);
    }
    
    public void VivoxTTSSpeakMsg(string msgToSpeak, TTSDestination playMode)
    {
        SpeakTTS(msgToSpeak, playMode);
    }

    public void VivoxChooseTTSVoiceGender(VoiceGender voiceGender)
    {
        ChooseVoiceGender(voiceGender);
    }


    #endregion


 
    #region Audio / Text / Channel Callbacks


    public virtual void OnVoiceConnecting(IChannelSession channelSession)
    {
        this.DebugLog($"{channelSession.Channel.Name} Audio Is Connecting In Channel");
    }

    public virtual void OnVoiceConnected(IChannelSession channelSession)
    {
        this.DebugLog($"{channelSession.Channel.Name} Audio Has Connected In Channel");
    }

    public virtual void OnVoiceDisconnecting(IChannelSession channelSession)
    {
        this.DebugLog($"{channelSession.Channel.Name} Audio Is Disconnecting In Channel");
    }

    public virtual void OnVoiceDisconnected(IChannelSession channelSession)
    {
         this.DebugLog($"{channelSession.Channel.Name} Audio Has Disconnected In Channel");
    }




    public virtual void OnTextChannelConnecting(IChannelSession channelSession)
    {
        this.DebugLog($"{channelSession.Channel.Name} Text Is Connecting In Channel");
    }

    public virtual void OnTextChannelConnected(IChannelSession channelSession)
    {
        this.DebugLog($"{channelSession.Channel.Name} Text Has Connected In Channel");
    }

    public virtual void OnTextChannelDisconnecting(IChannelSession channelSession)
    {
         this.DebugLog($"{channelSession.Channel.Name} Text Is Disconnecting In Channel");
    }

    public virtual void OnTextChannelDisconnected(IChannelSession channelSession)
    {
         this.DebugLog($"{channelSession.Channel.Name} Text Has Disconnected In Channel");
    }



    public virtual void OnChannelConnecting(IChannelSession channelSession)
    {
        this.DebugLog($"{channelSession.Channel.Name} Is Connecting");
    }

    public virtual void OnChannelConnected(IChannelSession channelSession)
    {
        this.DebugLog($"{channelSession.Channel.Name} Has Connected");
        this.DebugLog($"Channel Type == {channelSession.Channel.Type.ToString()}");
    }

    public virtual void OnChannelDisconnecting(IChannelSession channelSession)
    {
        this.DebugLog($"{channelSession.Channel.Name} Is Disconnecting");
    }

    public virtual void OnChannelDisconnected(IChannelSession channelSession)
    {
        this.DebugLog($"{channelSession.Channel.Name} Has Disconnected");
        RemoveChannelSession(channelSession.Channel.Name);
    }



    #endregion


    #region User Callbacks


    public virtual void OnParticipantAdded(IParticipant participant)
    {
        this.DebugLog($"{participant.Account.DisplayName} Has Joined The Channel");
    }

    public virtual void OnParticipantRemoved(IParticipant participant)
    {
        this.DebugLog($"{participant.Account.DisplayName} Has Left The Channel");
        
    }
    
    public virtual void OnParticipantValueUpdated(IParticipant participant)
    {
       // this.DebugLog($"{participant.Account.DisplayName} Has updated itself in the channel");
        
    }

    public virtual void OnUserMuted(IParticipant participant)
    {
        // todo add option if statement to display debug messages
       // this.DebugLog($"{participant.Account.DisplayName} Is Muted : (Muted For All : {participant.IsMutedForAll})");
        
    }

    public virtual void OnUserUnmuted(IParticipant participant)
    {
        this.DebugLog($"{participant.Account.DisplayName} Is Unmuted : (Muted For All : {participant.IsMutedForAll})");
        
    }

    public virtual void OnUserSpeaking(IParticipant participant)
    {
        this.DebugLog($"{participant.Account.DisplayName} Is Speaking : Audio Energy {participant.AudioEnergy.ToString()}");
        
    }

    public virtual void OnUserNotSpeaking(IParticipant participant)
    {
        this.DebugLog($"{participant.Account.DisplayName} Is Not Speaking");
    }


    #endregion


    #region Message Callbacks


    public virtual void OnChannelMessageRecieved(IChannelTextMessage textMessage)
    {
        this.DebugLog($"From {textMessage.Sender.DisplayName} : {textMessage.ReceivedTime} : {textMessage.Message}");
    }

    public virtual void OnDirectMessageRecieved(IDirectedTextMessage directedTextMessage)
    {    
        this.DebugLog($"Recived Message From : {directedTextMessage.Sender.DisplayName} : {directedTextMessage.ReceivedTime} : {directedTextMessage.Message}");
    }

    public virtual void OnDirectMessageFailed(IFailedDirectedTextMessage failedMessage)
    {
        this.DebugLog($"Failed To Send Message From : {failedMessage.Sender}");
    }


    public virtual void OnUserTyping(bool isTyping)
    {
        if (isTyping)
        {
            // this callback fires very often so this is commented out so it doesnt spam your Debug Console
           //  this.DebugLog($"User is typing");
        }
        else
        {
            // this callback fires very often so this is commented out so it doesnt spam your Debug Console
            //  this.DebugLog($"User stopped typing");
        }

    }


    #endregion


    #region Login / Logout Callbacks

 
    public virtual void OnLoggingIn(ILoginSession loginSession)
    {
        this.DebugLog($"Logging In : {loginSession.LoginSessionId.DisplayName}");
    }

    public virtual void OnLoggedIn(ILoginSession loginSession)
    {
        this.DebugLog($"Logged in : {loginSession.LoginSessionId.DisplayName}  : Presence = {loginSession.Presence.Status}");
        // must be logged in to call these methods or will result in error
        RequestAndroidMicPermission();
        ChooseVoiceGender(VoiceGender.female);
    }

    public virtual void OnLoggingOut(ILoginSession loginSession)
    {
        this.DebugLog($"Logging out : {loginSession.LoginSessionId.DisplayName}  : Presence = {loginSession.Presence.Status}");
    }

    public virtual void OnLoggedOut(ILoginSession loginSession)
    {
        this.DebugLog($"Logged out : {loginSession.LoginSessionId.DisplayName}  : Presence = {loginSession.Presence.Status}");
    }



    protected virtual void OnLoginAdded(ILoginSession loginSession)
    {
        this.DebugLog($"Login Added : {loginSession.LoginSessionId.DisplayName}  : Presence = {loginSession.Presence.Status}");
    }

    protected virtual void OnLoginRemoved(ILoginSession loginSession)
    {
        this.DebugLog($"Login Removed : {loginSession.LoginSessionId.DisplayName} : Presence = {loginSession.Presence.Status}");
    }

    protected virtual void OnLoginUpdated(ILoginSession loginSession)
    {
        this.DebugLog($"Login Updated : Login Updated : {loginSession.LoginSessionId.DisplayName} : Presence = {loginSession.Presence.Status}");
    }


    #endregion


    #region Subscription / Presence Callbacks

    protected virtual void OnAddAllowedSubscription(AccountId accountId)
    {
        this.DebugLog($"{accountId.DisplayName} User Has Been Allowed Has Been Added");
    }

    protected virtual void OnRemoveAllowedSubscription(AccountId accountId)
    {
         this.DebugLog($"{accountId.DisplayName} User Has Been Allowed Has Been Removed");
    }

    protected virtual void OnAddBlockedSubscription(AccountId accountId)
    {
         this.DebugLog($"{accountId.DisplayName} Block On User Has Been Added");
    }

    protected virtual void OnRemoveBlockedSubscription(AccountId accountId)
    {
        this.DebugLog($"{accountId.DisplayName} Block On User Has Been Removed");
    }

    protected virtual void OnAddPresenceSubscription(AccountId accountId)
    {
        this.DebugLog($"{accountId.DisplayName} Presence Has Been Added");
    }

    protected virtual void OnRemovePresenceSubscription(AccountId accountId)
    {
        this.DebugLog($"{accountId.DisplayName} Presence Has Been Removed");
    }

    protected virtual void OnUpdatePresenceSubscription(ValueEventArg<AccountId, IPresenceSubscription> presence)
    {
        this.DebugLog($"{presence.Value.Key.DisplayName} Presence Has Been Updated");
    }

    #endregion


    #region Text-to-Speech Callbacks

    public virtual void OnTTSMessageAdded(ITTSMessageQueueEventArgs ttsArgs)
    {
        this.DebugLog($"TTS Message Has Been Added : {ttsArgs.Message.Text}");
    }

    public virtual void OnTTSMessageRemoved(ITTSMessageQueueEventArgs ttsArgs)
    {
        this.DebugLog($"TTS Message Has Been Removed : {ttsArgs.Message.Text}");
    }

    public virtual void OnTTSMessageUpdated(ITTSMessageQueueEventArgs ttsArgs)
    {
        this.DebugLog($"TTS Message Has Been Updated : {ttsArgs.Message.Text}");
    }


    #endregion


    #region Error Callbacks


    protected void OnSendLog(MethodBase method, string message)
    {
        StringBuilder parameters = new StringBuilder();
        foreach (ParameterInfo p in method.GetParameters())
        {
            parameters.Append($"{p.ParameterType} {p.Name}, ");
        }
        var result = parameters.ToString().TrimEnd(' ', ',');
        this.DebugLog($"[ {method.ReflectedType.FullName} ] {method.Name}({result}) : {message}");
    }


    #endregion









}

