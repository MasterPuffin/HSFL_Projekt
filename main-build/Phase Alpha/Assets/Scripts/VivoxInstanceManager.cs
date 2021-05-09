using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;
using VivoxUnity;
using Random = UnityEngine.Random;

/// <summary>
/// Example on how to Inherit from VivoxManager Instead of using 
/// <para>Vivox_Manager  vivoxManager = FindObjectOfType&lt;VivoxManager&gt;();</para>
/// <para>GameObject vivoxManager = this.GetComponent&lt;VivoxManager&gt;();</para>
/// <para>GameObject vivoxManager = GameObject.FindObjectOfType&lt;VivoxManager&gt;();</para>
/// <para>With inheritance you can just override any callbacks you want instead of writing your own methods from scratch and subscribing to events</para>
/// </summary>
public class VivoxInstanceManager : VivoxManager {
    // [SerializeField] Text messageText;
    // [SerializeField] Text remotePlayerAudioText;
    // [SerializeField] Text remotePlayerNameInput;

    // [SerializeField] Slider localUserAudio;
    // [SerializeField] Slider remoteUserAudio;

    private bool isLocalSelfMuted = false;

    public string userName = "";
    public string channelName = "";

    public void StartVivox(string user, string channel) {
        Debug.Log("Started Vivox, Channel " + channel);
        userName = user;
        channelName = channel;
        VivoxLoginUser(userName);
    }

    public void EndVivox() {
        VivoxLogoutSession();
    }

    #region Custom Methods

    public override void OnLoggedIn(ILoginSession loginSession) {
        base.OnLoggedIn(loginSession);
        Debug.Log($"\nLogged In as {loginSession.LoginSessionId.DisplayName}");
        VivoxJoinChannel(channelName, true, true, true);
    }


    #endregion

    #region Main Methods

    public void JoinChannel() {
        VivoxJoinChannel(channelName, true, true, true);
    }

    public void LeaveChannel() {
        VivoxLeaveChannel(channelName);
    }

    public void ToggleMuteSelf() {
        if (isLocalSelfMuted) {
            isLocalSelfMuted = false;
        } else {
            isLocalSelfMuted = true;
        }

        VivoxToggleMuteSelf(isLocalSelfMuted);
    }

    public void AdjustVolumeForLocalUser() {
        //TODO: Implement UI Sliders
        // VivoxAdjustLocalUserVoiceVolume((int) localUserAudio.value);
    }

    public void AdjustRemoteUserVolume() {
        //TODO: Implement UI Sliders
        // VivoxAdjustRemoteUserVolume(remotePlayerNameInput.text, channelName, remoteUserAudio.value);
    }

    #endregion


    #region Vivox Event Callbacks That can be Overriden

    public override void OnLoggedOut(ILoginSession loginSession) {
        base.OnLoggedOut(loginSession);
        this.DebugLog($"\nLogged out {loginSession.LoginSessionId.DisplayName}");
    }

    public override void OnChannelConnected(IChannelSession channelSession) {
        base.OnChannelConnected(channelSession);
        this.DebugLog($"\nChannel {channelSession.Channel.Name} has connected : {channelSession.Channel.Type}");
    }

    public override void OnChannelDisconnected(IChannelSession channelSession) {
        base.OnChannelDisconnected(channelSession);
        this.DebugLog($"\nChannel {channelSession.Channel.Name} has disconnected : {channelSession.Channel.Type}");
    }

    public override void OnVoiceConnected(IChannelSession channelSession) {
        base.OnVoiceConnected(channelSession);
        this.DebugLog($"\nVoice Channel {channelSession.Channel.Name} has connected : {channelSession.Channel.Type}");
    }

    public override void OnVoiceDisconnected(IChannelSession channelSession) {
        base.OnVoiceDisconnected(channelSession);
        this.DebugLog(
            $"\nVoice Channel {channelSession.Channel.Name} has disconnected : {channelSession.Channel.Type}");
    }

    public override void OnParticipantAdded(IParticipant participant) {
        base.OnParticipantAdded(participant);
        this.DebugLog(
            $"\n{participant.Account.DisplayName} has joined {participant.ParentChannelSession.Channel.Name}");
    }

    public override void OnParticipantRemoved(IParticipant participant) {
        base.OnParticipantRemoved(participant);
        this.DebugLog($"\n{participant.Account.DisplayName} has left {participant.ParentChannelSession.Channel.Name}");
    }

    public override void OnUserMuted(IParticipant participant) {
        base.OnUserMuted(participant);
        this.DebugLog($"\n{participant.Account.DisplayName} has been muted");
    }

    public override void OnUserUnmuted(IParticipant participant) {
        base.OnUserUnmuted(participant);
        this.DebugLog($"\n{participant.Account.DisplayName} has been unmuted");
    }

    public override void OnUserSpeaking(IParticipant participant) {
        base.OnUserSpeaking(participant);
        // this.DebugLog($"\n{participant.Account.DisplayName} is speaking");
    }

    public override void OnUserNotSpeaking(IParticipant participant) {
        base.OnUserNotSpeaking(participant);
        // this.DebugLog($"\n{participant.Account.DisplayName} is not speaking");
    }

    #endregion
}