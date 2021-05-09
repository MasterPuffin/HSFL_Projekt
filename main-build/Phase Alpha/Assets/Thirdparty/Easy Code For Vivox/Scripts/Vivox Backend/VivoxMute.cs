using System;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

public class VivoxMute
{
    public static event Action<bool> VivoxUserMuted;
    public static event Action<bool> VivoxUserUnmuted;

    private void OnVivoxUserMuted(bool isMuted)
    {
        VivoxUserMuted?.Invoke(isMuted);
    }

    private void OnVivoxUserUnmuted(bool isMuted)
    {
        VivoxUserUnmuted?.Invoke(isMuted);
    }

    public void LocalToggleMuteRemoteUser(string userName, IChannelSession channelSession)
    {
        
        var participants = channelSession.Participants;
        string userToMute = VivoxAccessTokens.GetUserSIP(channelSession.Channel.Issuer, userName, channelSession.Channel.Domain);
        Debug.Log($"Sip address - {userToMute}");
        if (participants[userToMute].InAudio && !participants[userToMute].IsSelf)
        {
            if (participants[userToMute].LocalMute)
            {
                participants[userToMute].LocalMute = false;
            }
            else
            {
                participants[userToMute].LocalMute = true;
            }
        }
        else
        {
            Debug.Log($"Failed to mute {participants[userToMute].Account.DisplayName}");
        }
    }  
    
    //public void ToggleMuteForAllRemoteUserProduction(string userToMute, bool setMute, IChannelSession channelSession)
    //{
    //    // todo check functionality
    //    // used for admin purposes. Use local mute for muting players on client machine
    //    // this will mute that user so no one else in the channel will hear them
    //    // account handle is account name(username to mute)
    //    var participants = channelSession.Participants;
    //    string localParticipantSIP = VivoxBehaviour.mainLoginSession.GetSIP();
    //    var localParticipant = participants[localParticipantSIP];

    //    string userToMuteSIP = VivoxAccessTokens.GetUserSIP(channelSession.Channel.Issuer,
    //userToMute, channelSession.Channel.Domain);

    //    var epoch = VivoxToken.SecondsSinceUnixEpochPlusDuration(TimeSpan.FromSeconds(90));
    //    string accessToken = VivoxToken.Token_f(VivoxBehaviour.tokenKey, VivoxBehaviour.tokenIssuer, epoch, "mute", VivoxBehaviour.GetUniqueRequestID, userToMuteSIP,
    //        VivoxBehaviour.mainLoginSession.GetSIP(), $"sip:confctl-g-{channelSession.Channel.Issuer}.{channelSession.Channel.Name}@{channelSession.Channel.Domain}");
        
    //    localParticipant.SetIsMuteForAll(userToMuteSIP, setMute, accessToken, ar => 
    //    {
    //        try
    //        {
    //            Debug.Log($"{userToMuteSIP} should be muted");
    //        }
    //        catch(Exception e)
    //        {
    //            Debug.Log(e.Message);
    //            Debug.Log(e.StackTrace);
    //        }
    //    });
    //}

    public void LocalToggleMuteSelf(VivoxUnity.Client client, bool mute)
    {
        // cant localmute self
        // mutes local users microphone across all connected channels
        if (mute)
        {
            client.AudioInputDevices.Muted = true;
            OnVivoxUserMuted(mute);
        }
        else
        {
            client.AudioInputDevices.Muted = false;
            OnVivoxUserUnmuted(mute);
        }
    }

    public void CrossMuteUser(ILoginSession loginSession, bool mute)
    {
        if (mute)
        {
            Debug.Log($"Muting {loginSession.LoginSessionId.DisplayName}");
        }
        else
        {
            Debug.Log($"Unmuting {loginSession.LoginSessionId.DisplayName}");
        }
        // todo check if it works
        loginSession.SetCrossMutedCommunications(loginSession.LoginSessionId, mute, CrossMuteResult);
    }

    public void CrossMuteUsers(ILoginSession loginSessionToMute, bool mute)
    {
        List<AccountId> accountIds = new List<AccountId>();
        loginSessionToMute.SetCrossMutedCommunications(accountIds, mute, CrossMuteResult);
        // todo check if this actually works
        // add this callback listener
        // loginSessionToMute.CrossMutedCommunications.AfterKeyAdded +=
    }

    public void ClearAllCurrentCrossMutedAccounts(ILoginSession loginSession)
    {
        loginSession.ClearCrossMutedCommunications(CrossMuteResult);
    }

    public void CrossMuteResult(IAsyncResult ar)
    {
        try
        {
            Debug.Log("Successful");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return;
        }
    }

}
