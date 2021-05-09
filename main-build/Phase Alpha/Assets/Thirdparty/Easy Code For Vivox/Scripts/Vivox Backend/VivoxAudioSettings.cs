using System;
using System.ComponentModel;
using UnityEngine;
using VivoxUnity;


 public class VivoxAudioSettings
{
    #region Audio Methods

    public void SetAudioDeviceInput(IAudioDevice device, VivoxUnity.Client client)
    {
        client.AudioInputDevices.BeginSetActiveDevice(device, ar =>
        {
            try
            {
                client.AudioInputDevices.EndSetActiveDevice(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }

    public void SetAudioDeviceOutput(IAudioDevice device, VivoxUnity.Client client)
    {
        client.AudioOutputDevices.BeginSetActiveDevice(device, ar =>
        {
            try
            {
                client.AudioOutputDevices.EndSetActiveDevice(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }

    public void SetAudioDevicesInput(VivoxUnity.Client client, IAudioDevice targetInput = null)
    {
        IAudioDevices inputDevices = client.AudioInputDevices;
        if (targetInput != null && targetInput != client.AudioInputDevices.ActiveDevice)
        {
            client.AudioInputDevices.BeginSetActiveDevice(targetInput, ar =>
            {
                {
                    if (ar.IsCompleted)
                    {
                        client.AudioInputDevices.EndSetActiveDevice(ar);
                    }
                }
            });
        }
    }

    public void SetAudioDevicesOutput(VivoxUnity.Client client, IAudioDevice targetOutput = null)
    {
        IAudioDevices outputDevices = client.AudioOutputDevices;
        if (targetOutput != null && targetOutput != client.AudioOutputDevices.ActiveDevice)
        {
            client.AudioOutputDevices.BeginSetActiveDevice(targetOutput, ar =>
            {
                if (ar.IsCompleted)
                {
                    client.AudioOutputDevices.EndSetActiveDevice(ar);
                }
            });

        }
    }

    public void AudioAdjustLocalPlayerVolume(int value, VivoxUnity.Client client)
    {
        client.AudioOutputDevices.BeginRefresh(ar =>
        {
            try
            {
                client.AudioOutputDevices.EndRefresh(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            client.AudioOutputDevices.VolumeAdjustment = value;
        });
    }

    public void AudioAdjustRemotePlayerVolume(string userName, IChannelSession channelSession, float value)
    {
        var userVolumeToUpdate = VivoxAccessTokens.GetUserSIP(channelSession.Parent.Key.Issuer, userName, channelSession.Parent.Key.Domain);
        channelSession.Participants[userVolumeToUpdate].LocalVolumeAdjustment = Mathf.RoundToInt(value);
    }

    public void AudioStartInjection(string wavToInject, ILoginSession loginSession)
    {
        loginSession.StartAudioInjection(wavToInject);
    }

    public void AudioStopInjection(ILoginSession loginSession)
    {
        loginSession.StopAudioInjection();
    }


    public void AudioRefreshAudioDevices(VivoxUnity.Client client)
    {
        client.AudioOutputDevices.BeginRefresh(ar =>
        {
            try
            {
                client.AudioOutputDevices.EndRefresh(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }
    #endregion


    private void OnAudioDeviceAdded(object sender, PropertyChangedEventArgs propArgs) // todo investigate cuz this Doesnt really work
    {
        // todo see if ix update dtheir API and try again
        var a = (IAudioDevices)sender;

        if (propArgs.PropertyName == "EventAfterDeviceAvailableAdded")
        {
            Debug.Log("Device Added");
        }
        if (propArgs.PropertyName == "EventBeforeAvailableDeviceRemoved")
        {
            Debug.Log("Device Removed");
        }
        if (propArgs.PropertyName == "EventEffectiveDeviceChanged")
        {
            Debug.Log("Device effective changed");
        }
        Debug.Log($"Audio device should have been added {a.ToString()}");
    }


}
