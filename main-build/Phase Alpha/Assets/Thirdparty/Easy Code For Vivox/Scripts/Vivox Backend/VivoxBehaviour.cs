using System;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public abstract class VivoxBehaviour : MonoBehaviour
{

    #region Vivox Credentials and Global Variables


    public static readonly VivoxUnity.Client mainClient = new Client();

    public static Uri serverUri;
    public static string domain;
    public static string tokenIssuer;
    public static string tokenKey;
    public static TimeSpan timeSpan = TimeSpan.FromSeconds(90);

    public static ILoginSession mainLoginSession;
    public static Dictionary<string, IChannelSession> mainChannelSessions = new Dictionary<string, IChannelSession>();

    public static Dictionary<string, string> attemptedDirectMessages = new Dictionary<string, string>(); // player, msg
    public static List<string> FailedDirectMessagesRequestIDs = new List<string>();

    public static Dictionary<string, List<AccountId>> presenceSubscriptions = new Dictionary<string, List<AccountId>>();
    public static Dictionary<string, List<AccountId>> blockedSubscriptions = new Dictionary<string, List<AccountId>>();
    public static Dictionary<string, List<AccountId>> allowedSubscriptions = new Dictionary<string, List<AccountId>>();

    public static Dictionary<string, IAudioDevice> audioCaptureDevices = new Dictionary<string, IAudioDevice>();
    public static Dictionary<string, IAudioDevice> audioRenderDevices = new Dictionary<string, IAudioDevice>();


    protected static bool isClientInitialized = false;

    private static int uniqueRequest_ID = 0;


    #endregion


    #region Vivox Backend Functionality Classes, Enums, Events


    public enum VoiceGender { male, female }

    protected VivoxLogin vivoxLogin = new VivoxLogin();
    protected VivoxChannel vivoxChannel = new VivoxChannel();
    protected VivoxChannelVoice vivoxChannelVoice = new VivoxChannelVoice();
    protected VivoxChannelText vivoxChannelText = new VivoxChannelText();
    protected VivoxParticipants vivoxParticipants = new VivoxParticipants();
    protected VivoxMessages vivoxMessages = new VivoxMessages();
    internal VivoxAudioSettings vivoxAudioSettings = new VivoxAudioSettings();
    protected VivoxMute vivoxMute = new VivoxMute();
    protected VivoxSubscriptions vivoxSubscriptions = new VivoxSubscriptions();
    protected VivoxTextToSpeech vivoxTextToSpeech = new VivoxTextToSpeech();

    public event Action<bool> VivoxIsUserTyping;
    protected bool IsTyping { get; set; } = false;
    protected float TypingCountdown = 0f;

    // todo add functionality seperate channel and direct message functionality if possible, send event message every 2-3 seconds to check types
    // todo provide option to turn off for performance
    protected void OnVivoxIsUserTyping(bool isTyping)
    {
        IsTyping = isTyping;
        VivoxIsUserTyping?.Invoke(isTyping);
    }


    #endregion

   
    // guarantees to only Initialize client once
    protected void InitializeClient()
    {
        if (isClientInitialized)
        {

            Debug.Log($"{nameof(VivoxManager)} : Client is already initialized, skipping...");
            return;
        }
        else
        {
            if (!mainClient.Initialized)
            {
                mainClient.Uninitialize();
                mainClient.Initialize();
                isClientInitialized = true;
                Debug.Log("Client Initialzed");
            }
        }
    }

    public static int GetUniqueRequestID
    {
        get
        {
            uniqueRequest_ID++;
            return uniqueRequest_ID;
        }
    }


    #region Main Methods


    protected void RequestAndroidMicPermission()
    {
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#endif
    }

    protected IChannelSession GetChannelSession(string channelName, ChannelType channelType = ChannelType.NonPositional, Channel3DProperties channel3DProperties = null)
    {
        // max channels is 11
        // for games 1 channel should be positional and 10 should be non-positional
        if (mainChannelSessions.ContainsKey(channelName))
        {
            GetExistingChannelSession(channelName, channelType, channel3DProperties);
        }
        else
        {
            if (mainChannelSessions.Count <= 11)
            {
                CheckChannelType(channelName, channelType, channel3DProperties);
            }
            else
            {
                this.DebugLog($"Too Many Channels Already Active : Positional Max is 1, Non-Positional Max is 10, Leave One and Try Again");
                return null;
            }
        }
        return mainChannelSessions[channelName];
    }

    public IChannelSession GetExistingChannelSession(string channelName, ChannelType channelType, Channel3DProperties channel3DProperties)
    {
        if (mainChannelSessions[channelName].ChannelState == ConnectionState.Disconnected || mainChannelSessions[channelName] == null)
        {
            if (channelType == ChannelType.Positional)
            {
                mainChannelSessions[channelName] = mainLoginSession.GetChannelSession(new ChannelId(tokenIssuer, channelName, domain, channelType, channel3DProperties));
            }
            else
            {
                mainChannelSessions[channelName] = mainLoginSession.GetChannelSession(new ChannelId(tokenIssuer, channelName, domain, channelType));
            }
        }

         return mainChannelSessions[channelName];
    }

    public IChannelSession CheckChannelType(string channelName, ChannelType channelType, Channel3DProperties channel3DProperties)
    {
        if (channelType == ChannelType.Positional)
        {
            foreach (KeyValuePair<string, IChannelSession> channel in mainChannelSessions)
            {
                if (channel.Value.Channel.Type == ChannelType.Positional)
                {
                    Debug.Log($"{channel.Value.Channel.Name} Is already a Positional Channel. Can Only Have One 3D Positional Channel. Refer To Vivox Documentation Returning Null");
                    return null;
                }
            }

            mainChannelSessions.Add(channelName, mainLoginSession.GetChannelSession(new ChannelId(tokenIssuer, channelName, domain, channelType, channel3DProperties)));
            return mainChannelSessions[channelName];
        }
        else
        {
            mainChannelSessions.Add(channelName, mainLoginSession.GetChannelSession(new ChannelId(tokenIssuer, channelName, domain, channelType)));
            return mainChannelSessions[channelName];
        }
    }

    public ChannelId GetChannelID(string channelName)
    {
        foreach(KeyValuePair<string, IChannelSession> channel in mainChannelSessions)
        {
            if(channelName == channel.Key)
            {
                return channel.Value.Channel;
            }
        }
        return new ChannelId(tokenIssuer, channelName, domain);
    }


    protected void RemoveChannelSession(string channelName)
    {
        if (mainChannelSessions.ContainsKey(channelName))
        {
            mainChannelSessions.Remove(channelName);
        }
    }



    protected void Login(string userName)
    {
        try
        {
            mainLoginSession = mainClient.GetLoginSession(new AccountId(tokenIssuer, userName, domain));

            vivoxMessages.SubscribeToDirectMessages(mainLoginSession);
            vivoxTextToSpeech.Subscribe(mainLoginSession);
            vivoxSubscriptions.Subscribe(mainLoginSession);

            vivoxLogin.Login(mainLoginSession, serverUri, tokenKey, timeSpan);
        }
        catch(Exception e)
        {
            vivoxSubscriptions.Unsubscribe(mainLoginSession);
            vivoxMessages.UnsubscribeFromDirectMessages(mainLoginSession);
            vivoxTextToSpeech.Unsubscribe(mainLoginSession);
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
        }

    }
    
    protected void LoginProduction(string userName)
    {
        try
        {
            mainLoginSession = mainClient.GetLoginSession(new AccountId(tokenIssuer, userName, domain));

            vivoxMessages.SubscribeToDirectMessages(mainLoginSession);
            vivoxTextToSpeech.Subscribe(mainLoginSession);
            vivoxSubscriptions.Subscribe(mainLoginSession);

            vivoxLogin.LoginProduction(mainLoginSession, serverUri, tokenKey, tokenIssuer, GetUniqueRequestID);
        }
        catch(Exception e)
        {
            vivoxSubscriptions.Unsubscribe(mainLoginSession);
            vivoxMessages.UnsubscribeFromDirectMessages(mainLoginSession);
            vivoxTextToSpeech.Unsubscribe(mainLoginSession);
            this.DebugLog(e.StackTrace);
        }

    }

    protected void Logout()
    {
        if(mainLoginSession.State == LoginState.LoggedIn)
        {
            vivoxSubscriptions.Unsubscribe(mainLoginSession);
            vivoxMessages.UnsubscribeFromDirectMessages(mainLoginSession);
            vivoxTextToSpeech.Unsubscribe(mainLoginSession);
            vivoxLogin.Logout(mainLoginSession);
        }
        else
        {
            Debug.Log($"Not logged in");
        }
    }

    protected void JoinChannel(string channelName, bool includeVoice, bool includeText, bool switchTransmissionToThisChannel)
    {
        IChannelSession channelSession = GetChannelSession(channelName, ChannelType.NonPositional);

        try
        {
            vivoxChannelText.Subscribe(channelSession);
            vivoxChannelVoice.Subscribe(channelSession);
            vivoxParticipants.SubscribeToParticipants(channelSession);
            vivoxMessages.SubscribeToChannelMessages(channelSession);

            vivoxChannel.JoinChannel(includeVoice, includeText, switchTransmissionToThisChannel, channelSession, tokenKey, timeSpan);
        }
        catch(Exception e)
        {
            vivoxChannelText.Unsubscribe(channelSession);
            vivoxChannelVoice.Unsubscribe(channelSession);
            vivoxParticipants.UnsubscribeFromParticipants(channelSession);
            vivoxMessages.UnsubscribeFromChannelMessages(channelSession);
            this.DebugLog(e.StackTrace);
        }
    }   
    
    protected void JoinChannelEcho(string channelName, bool includeVoice, bool includeText, bool switchTransmissionToThisChannel)
    {
        IChannelSession channelSession = GetChannelSession(channelName, ChannelType.Echo);

        try
        {
            vivoxChannelText.Subscribe(channelSession);
            vivoxChannelVoice.Subscribe(channelSession);
            vivoxParticipants.SubscribeToParticipants(channelSession);
            vivoxMessages.SubscribeToChannelMessages(channelSession);

            vivoxChannel.JoinChannel(includeVoice, includeText, switchTransmissionToThisChannel, channelSession, tokenKey, timeSpan);
        }
        catch(Exception e)
        {
            vivoxChannelText.Unsubscribe(channelSession);
            vivoxChannelVoice.Unsubscribe(channelSession);
            vivoxParticipants.UnsubscribeFromParticipants(channelSession);
            vivoxMessages.UnsubscribeFromChannelMessages(channelSession);
            this.DebugLog(e.StackTrace);
        }
    }

    protected void JoinChannel3DPositional(string channelName, bool includeVoice, bool includeText, bool switchTransmissionToThisChannel, Channel3DProperties channel3D)
    {
        IChannelSession channelSession = GetChannelSession(channelName, ChannelType.Positional, channel3D);

        try
        {
            vivoxChannelText.Subscribe(channelSession);
            vivoxChannelVoice.Subscribe(channelSession);
            vivoxParticipants.SubscribeToParticipants(channelSession);
            vivoxMessages.SubscribeToChannelMessages(channelSession);

            vivoxChannel.JoinChannel(includeVoice, includeText, switchTransmissionToThisChannel, channelSession, tokenKey, timeSpan);
        }
        catch(Exception e)
        {
            vivoxChannelText.Unsubscribe(channelSession);
            vivoxChannelVoice.Unsubscribe(channelSession);
            vivoxParticipants.UnsubscribeFromParticipants(channelSession);
            vivoxMessages.UnsubscribeFromChannelMessages(channelSession);
            this.DebugLog(e.StackTrace);
        }

    }

    protected void JoinChannelProduction(string channelName, bool includeVoice, bool includeText, bool switchTransmissionToThisChannel, bool joinMuted = false)
    {
        IChannelSession channelSession = GetChannelSession(channelName, ChannelType.NonPositional);

        try
        {
            vivoxChannelText.Subscribe(channelSession);
            vivoxChannelVoice.Subscribe(channelSession);
            vivoxParticipants.SubscribeToParticipants(channelSession);
            vivoxMessages.SubscribeToChannelMessages(channelSession);

            vivoxChannel.JoinChannelProduction(includeVoice, includeText, switchTransmissionToThisChannel, channelSession,
            tokenKey, tokenIssuer, GetUniqueRequestID, joinMuted);
        }
        catch(Exception e)
        {
            vivoxChannelText.Unsubscribe(channelSession);
            vivoxChannelVoice.Unsubscribe(channelSession);
            vivoxParticipants.UnsubscribeFromParticipants(channelSession);
            vivoxMessages.UnsubscribeFromChannelMessages(channelSession);
            this.DebugLog(e.StackTrace);
        }
    }

    protected void JoinChannelProductionEcho(string channelName, bool includeVoice, bool includeText, bool switchTransmissionToThisChannel, bool joinMuted = false)
    {
        IChannelSession channelSession = GetChannelSession(channelName, ChannelType.Echo);

        try
        {
            vivoxChannelText.Subscribe(channelSession);
            vivoxChannelVoice.Subscribe(channelSession);
            vivoxParticipants.SubscribeToParticipants(channelSession);
            vivoxMessages.SubscribeToChannelMessages(channelSession);

            // investigtae the proper token to join echo channel
            vivoxChannel.JoinChannelProduction(includeVoice, includeText, switchTransmissionToThisChannel, channelSession,
            tokenKey, tokenIssuer, GetUniqueRequestID, joinMuted);
        }
        catch(Exception e)
        {
            vivoxChannelText.Unsubscribe(channelSession);
            vivoxChannelVoice.Unsubscribe(channelSession);
            vivoxParticipants.UnsubscribeFromParticipants(channelSession);
            vivoxMessages.UnsubscribeFromChannelMessages(channelSession);
            this.DebugLog(e.StackTrace);
        }
    }

    protected void JoinChannelProduction3DPositional(string channelName, bool includeVoice, bool includeText, bool switchTransmissionToThisChannel,
        Channel3DProperties channel3DProperties = null, bool joinMuted = false)
    {
        IChannelSession channelSession = GetChannelSession(channelName, ChannelType.Positional, channel3DProperties);

        try
        {
            vivoxChannelText.Subscribe(channelSession);
            vivoxChannelVoice.Subscribe(channelSession);
            vivoxParticipants.SubscribeToParticipants(channelSession);
            vivoxMessages.SubscribeToChannelMessages(channelSession);

            if (channel3DProperties == null)
            {
                channel3DProperties = new Channel3DProperties();
            }
            // use different channelsessions
            // use different channel name
            // log into positional first, wait till logged in and then log into nonpositional
            vivoxChannel.JoinChannelProduction(includeVoice, includeText, switchTransmissionToThisChannel, channelSession,
            tokenKey, tokenIssuer, GetUniqueRequestID, joinMuted);
        }
        catch(Exception e)
        {
            vivoxChannelText.Unsubscribe(channelSession);
            vivoxChannelVoice.Unsubscribe(channelSession);
            vivoxParticipants.UnsubscribeFromParticipants(channelSession);
            vivoxMessages.UnsubscribeFromChannelMessages(channelSession);
            this.DebugLog(e.StackTrace);
        }
    }

    protected void JoinChannelProduction3DPositionalMMO(bool includeVoice, bool includeText, bool switchTransmissionToThisChannel,
        string region, string channelName, string squadname, Channel3DProperties channel3DProperties = null, bool joinMuted = false)
    {
        IChannelSession channelSession = GetChannelSession($"{region}.{channelName}-{squadname}", ChannelType.Positional, channel3DProperties);

        try
        {
            vivoxChannelText.Subscribe(channelSession);
            vivoxChannelVoice.Subscribe(channelSession);
            vivoxParticipants.SubscribeToParticipants(channelSession);
            vivoxMessages.SubscribeToChannelMessages(channelSession);

            if (channel3DProperties == null)
            {
                channel3DProperties = new Channel3DProperties(1, 1, 1, AudioFadeModel.InverseByDistance);
            }
            // use different channelsessions
            // use different channel name
            // log into positional first, wait till logged in and then log into nonpositional
            vivoxChannel.JoinChannelProductionMMO(includeVoice, includeText, switchTransmissionToThisChannel, channelSession,
               tokenKey, tokenIssuer, GetUniqueRequestID, joinMuted);
        }
        catch(Exception e)
        {
            vivoxChannelText.Unsubscribe(channelSession);
            vivoxChannelVoice.Unsubscribe(channelSession);
            vivoxParticipants.UnsubscribeFromParticipants(channelSession);
            vivoxMessages.UnsubscribeFromChannelMessages(channelSession);
            this.DebugLog(e.StackTrace);
        }
    }

    protected void LeaveChannel(string channelName)
    {
        if (mainChannelSessions.ContainsKey(channelName))
        {
            vivoxParticipants.UnsubscribeFromParticipants(mainChannelSessions[channelName]);
            vivoxMessages.UnsubscribeFromChannelMessages(mainChannelSessions[channelName]);
            vivoxChannel.LeaveChannel(mainLoginSession, mainChannelSessions[channelName]);
        }
    }

    protected void SetVoiceActiveInChannel(string channelName, bool connect)
    {
        // todo fix error where channel disconects if both text and voice are disconnected and when you try and toggle 
        // you gte an object null refenrce because channel name exists but channelsession doesnt exist
        IChannelSession channelSession = mainLoginSession.GetChannelSession(GetChannelID(channelName));
        vivoxChannelVoice.ToggleAudioChannelActive(channelSession, connect);
    }

    protected void SetTextActiveInChannel(string channelname, bool connect)
    {
        // todo fix error where channel disconects if both text and voice are disconnected and when you try and toggle 
        // you gte an object null refenrce because channel name exists but channelsession doesnt exist
        IChannelSession channelSession = mainLoginSession.GetChannelSession(GetChannelID(channelname));
        vivoxChannelText.ToggleTextChannelActive(channelSession, connect);
    }
    protected void SendChannelMessage(string channelname, string msg, string commandNameSpace = null, string commandBody = null)
    {
        vivoxMessages.SendChannelMessage(GetChannelSession(channelname),
            msg, commandNameSpace, commandBody);
    }

    protected void SendDirectMessage(string userToMsg, string msg, string commandNameSpace = null, string commandBody = null)
    {
        // todo check if user is blocked and alert front end users
        vivoxMessages.SendDirectMessage(mainLoginSession, userToMsg, msg, commandNameSpace, commandBody);
    }

    protected void ToggleMuteSelf(bool mute)
    {
        vivoxMute.LocalToggleMuteSelf(mainClient, mute);
    } 
    
    protected void ToggleMuteRemoteUser(string userName, string channelName)
    {
        vivoxMute.LocalToggleMuteRemoteUser(userName, GetChannelSession(channelName));
    }

    //protected void MuteUserForAllProduction(string userName, string channelName, bool setMute)
    //{
    //   // vivoxMute.ToggleMuteForAllRemoteUserProduction(userName, setMute, GetChannelSession(channelName));
    //}

    protected void AdjustLocalUserVolume(int volume)
    {
        vivoxAudioSettings.AudioAdjustLocalPlayerVolume(volume, mainClient);
    }

    protected void AdjustRemoteUserVolume(string userName, string channelName, float volume)
    {
        IChannelSession channelSession = mainLoginSession.GetChannelSession(GetChannelID(channelName));
        vivoxAudioSettings.AudioAdjustRemotePlayerVolume(userName, channelSession, volume);
    }

    protected void AddFriend(string userName)
    {
        vivoxSubscriptions.AddAllowPresence(userName, mainLoginSession, tokenIssuer, domain);
    }

    protected void RemoveFriend(string userName)
    {
        vivoxSubscriptions.RemoveAllowedPresence(userName, mainLoginSession, tokenIssuer, domain);
    }

    protected void AddAllowedUser(string userName)
    {
        vivoxSubscriptions.AddAllowedSubscription(userName, mainLoginSession, tokenIssuer, domain);
    }
    
    protected void RemoveAllowedUser(string userName)
    {
        vivoxSubscriptions.RemoveAllowedSubscription(userName, mainLoginSession, tokenIssuer, domain);
    }

    protected void BlockUser(string userName)
    {
        vivoxSubscriptions.AddBlockedSubscription(userName, mainLoginSession, tokenIssuer, domain);
    }

    protected void RemoveBlockedUser(string userName)
    {
        vivoxSubscriptions.RemoveBlockedSubscription(userName, mainLoginSession, tokenIssuer, domain);
    }

    protected void SpeakTTS(string msg)
    {
        vivoxTextToSpeech.TTSSpeak(msg, TTSDestination.QueuedLocalPlayback, mainLoginSession);
    }

    protected void SpeakTTS(string msg, TTSDestination playMode)
    {
        vivoxTextToSpeech.TTSSpeak(msg, playMode, mainLoginSession);
    }

    protected void ChooseVoiceGender(VoiceGender voiceGender)
    {
        switch (voiceGender)
        {
            case VoiceGender.male:
            vivoxTextToSpeech.TTSChooseVoice(vivoxTextToSpeech.maleVoice, mainLoginSession);
            break;

            case VoiceGender.female:
            vivoxTextToSpeech.TTSChooseVoice(vivoxTextToSpeech.femaleVoice, mainLoginSession);
            break;
        }
    }



    #endregion


}
