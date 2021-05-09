using System;
using System.ComponentModel;
using VivoxUnity;
using UnityEngine;
using VATAuthenticate;

public class VivoxLogin
{
    public static event Action<ILoginSession> VivoxLoggingIn;
    public static event Action<ILoginSession> VivoxLoggedIn;
    public static event Action<ILoginSession> VivoxLoggedOut;
    public static event Action<ILoginSession> VivoxLoggingOut;


    public void Subscribe(ILoginSession loginSession)
    {
        loginSession.PropertyChanged += OnLoginPropertyChanged;
    }

    public void Unsubscribe(ILoginSession loginSession)
    {
        loginSession.PropertyChanged -= OnLoginPropertyChanged;
    }



    #region Login Events


    public void OnVivoxLoggingIn(ILoginSession loginSession)
    {
        if (loginSession != null)
        {
            VivoxLoggingIn?.Invoke(loginSession);
        }
    }

    public void OnVivoxLoggedIn(ILoginSession loginSession)
    {
        if (loginSession != null)
        {
            VivoxLoggedIn?.Invoke(loginSession);
        }
    }


    public void OnVivoxLoggingOut(ILoginSession loginSession)
    {
        if (loginSession != null)
        {
            VivoxLoggingOut?.Invoke(loginSession);
        }

    }

    public void OnVivoxLoggedOut(ILoginSession loginSession)
    {
        if (loginSession != null)
        {
            VivoxLoggedOut?.Invoke(loginSession);

            Unsubscribe(loginSession);
        }
    }


    #endregion


    #region Login Methods


    public void Login(ILoginSession loginSession,
        Uri serverUri, string tokenKey, TimeSpan timeSpan)
    {
        Subscribe(loginSession);

        loginSession.BeginLogin(serverUri,
        loginSession.GetLoginToken(tokenKey, timeSpan), SubscriptionMode.Accept, null, null, null, ar =>
        {
            try
            {
                loginSession.EndLogin(ar);
            }
            catch (Exception e)
            {
                Unsubscribe(loginSession);
                Debug.Log(e.StackTrace);
            }
        });
    }

    public void LoginProduction(ILoginSession loginSession,
        Uri serverUri, string tokenKey, string issuer, int uniqueRequestID)
    {
        Subscribe(loginSession);

        var epoch = VAT.SecondsSinceUnixEpochPlusDuration(TimeSpan.FromSeconds(90));
        var vivoxAccessToken = VAT.Token_f(tokenKey, issuer, (int)epoch,
            "login", uniqueRequestID, null, loginSession.GetSIP(), null);

        loginSession.BeginLogin(serverUri, vivoxAccessToken, ar =>
        {
            try
            {
                loginSession.EndLogin(ar);
            }
            catch (Exception e)
            {
                Unsubscribe(loginSession);
                Debug.Log(e.StackTrace);
            }
        });
    }


    public void Logout(ILoginSession loginSession)
    {
        OnVivoxLoggingOut(loginSession);
        loginSession.Logout();
        OnVivoxLoggedOut(loginSession);
        Debug.Log($"Logging Out... Vivox does not have a Logging Out Event Callback. The events VivoxLoggingOut and VivoxLoggedOut are custom callback events. " +
            $"They will be called before and after the Logout method is called");
    }

    #endregion


    #region Login Callbacks

    // login status changed
    private void OnLoginPropertyChanged(object sender, PropertyChangedEventArgs propArgs)
    {
        var senderLoginSession = (ILoginSession)sender;

        if (propArgs.PropertyName == "State")
        {
            switch (senderLoginSession.State)
            {
                case LoginState.LoggingIn:
                    OnVivoxLoggingIn(senderLoginSession);
                    break;
                case LoginState.LoggedIn:
                    OnVivoxLoggedIn(senderLoginSession);
                    break;
                case LoginState.LoggingOut:
                    OnVivoxLoggingOut(senderLoginSession);
                    break;
                case LoginState.LoggedOut:
                    OnVivoxLoggedOut(senderLoginSession);
                    break;

                default:
                    Debug.Log("Logging Callback Error - Logging In/Out failed");
                    break;
            }
        }
    }


    #endregion
}
