using System;
using VivoxUnity;

public class VivoxParticipants
{

    public static event Action<IParticipant> VivoxParticipantAdded;
    public static event Action<IParticipant> VivoxParticipantRemoved;
    public static event Action<IParticipant> VivoxParticipantValueUpdated;

    public static event Action<IParticipant> VivoxUserMuted;
    public static event Action<IParticipant> VivoxUserUnmuted;

    public static event Action<IParticipant> VivoxUserSpeaking;
    public static event Action<IParticipant> VivoxUserNotSpeaking;


    public void SubscribeToParticipants(IChannelSession channelSession)
    {
        channelSession.Participants.AfterKeyAdded += OnParticipantAdded;
        channelSession.Participants.BeforeKeyRemoved += OnParticipantRemoved;
        channelSession.Participants.AfterValueUpdated += OnParticipantValueUpdated;
    }

    public void UnsubscribeFromParticipants(IChannelSession channelSession)
    {
        channelSession.Participants.AfterKeyAdded -= OnParticipantAdded;
        channelSession.Participants.BeforeKeyRemoved -= OnParticipantRemoved;
        channelSession.Participants.AfterValueUpdated -= OnParticipantValueUpdated;
    }



    #region Participant/User Events


    private  void OnVivoxParticipantAdded(IParticipant participant)
    {
        if (participant != null)
        {
            VivoxParticipantAdded?.Invoke(participant);
        }
    }

    private void OnVivoxParticipantRemoved(IParticipant participant)
    {
        if (participant != null)
        {
            VivoxParticipantRemoved?.Invoke(participant);
        }
    }

    private void OnVivoxParticipantValueUpdated(IParticipant participant)
    {
        if (participant != null)
        {
            VivoxParticipantValueUpdated?.Invoke(participant);
        }
    }



    private void OnVivoxUserMuted(IParticipant participant)
    {
        if (participant != null)
        {
            VivoxUserMuted?.Invoke(participant);
        }
    }

    private void OnVivoxUserUnmuted(IParticipant participant)
    {
        if (participant != null)
        {
            VivoxUserUnmuted?.Invoke(participant);
        }
    }

    private void OnVivoxUserSpeaking(IParticipant participant)
    {
        if (participant != null)
        {
            VivoxUserSpeaking?.Invoke(participant);
        }
    }

    private  void OnVivoxUserNotSpeaking(IParticipant participant)
    {
        if (participant != null)
        {
            VivoxUserNotSpeaking?.Invoke(participant);
        }

    }


    #endregion



    private void OnParticipantAdded(object sender, KeyEventArg<string> keyArg)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        var senderIParticipant = source[keyArg.Key];
        OnVivoxParticipantAdded(senderIParticipant);
    }

    private void OnParticipantRemoved(object sender, KeyEventArg<string> keyArg)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        var senderIParticipant = source[keyArg.Key];
        OnVivoxParticipantRemoved(senderIParticipant);
    }

    private void OnParticipantValueUpdated(object sender, ValueEventArg<string, IParticipant> valueArg)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        var senderIParticipant = source[valueArg.Key];
        OnVivoxParticipantValueUpdated(senderIParticipant);
        // todo look at the Vivox Tank example to make this better
        switch (valueArg.PropertyName)
        {
            case "LocalMute":

                if (!senderIParticipant.IsSelf) //can't local mute yourself, so don't check for it
                {
                    if (senderIParticipant.LocalMute)
                    {
                        OnVivoxUserMuted(senderIParticipant);
                    }
                    else
                    {
                        OnVivoxUserUnmuted(senderIParticipant);
                    }
                }
                break;

            case "SpeechDetected":
                {
                    if (senderIParticipant.SpeechDetected)
                    {
                        OnVivoxUserSpeaking(senderIParticipant);
                    }
                    else
                    {
                        OnVivoxUserNotSpeaking(senderIParticipant);
                    }
                    break;
                }
            default:
                break;
        }
    }
}
