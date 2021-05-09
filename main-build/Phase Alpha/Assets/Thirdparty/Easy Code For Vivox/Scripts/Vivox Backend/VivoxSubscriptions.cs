using System;
using UnityEngine;
using VivoxUnity;

public class VivoxSubscriptions
{
    
    public event Action<AccountId> VivoxSubscriptionAddAllowed;
    public event Action<AccountId> VivoxSubscriptionRemoveAllowed;

    public event Action<AccountId> VivoxSubscriptionAddBlocked;
    public event Action<AccountId> VivoxSubscriptionRemoveBlocked;

    public event Action<AccountId> VivoxSubscriptionAddPresence;
    public event Action<AccountId> VivoxSubscriptionRemovePresence;

    public event Action<ValueEventArg<AccountId, IPresenceSubscription>> VivoxSubscriptionUpdatePresence;


    public void Subscribe(ILoginSession loginSession)
    {
        loginSession.AllowedSubscriptions.AfterKeyAdded += OnAddAllowedSubscription;
        loginSession.AllowedSubscriptions.BeforeKeyRemoved += OnRemoveAllowedSubscription;

        loginSession.BlockedSubscriptions.AfterKeyAdded += OnAddBlockedSubscription;
        loginSession.BlockedSubscriptions.BeforeKeyRemoved += OnRemoveBlockedSubscription;

        loginSession.PresenceSubscriptions.AfterKeyAdded += OnAddPresenceSubscription;
        loginSession.PresenceSubscriptions.BeforeKeyRemoved += OnRemovePresenceSubscription;
        loginSession.PresenceSubscriptions.AfterValueUpdated += OnUpdatedPresenceSubscription;
    }

    public void Unsubscribe(ILoginSession loginSession)
    {
        loginSession.AllowedSubscriptions.AfterKeyAdded -= OnAddAllowedSubscription;
        loginSession.AllowedSubscriptions.BeforeKeyRemoved -= OnRemoveAllowedSubscription;

        loginSession.BlockedSubscriptions.AfterKeyAdded -= OnAddBlockedSubscription;
        loginSession.BlockedSubscriptions.BeforeKeyRemoved -= OnRemoveBlockedSubscription;

        loginSession.PresenceSubscriptions.AfterKeyAdded -= OnAddPresenceSubscription;
        loginSession.PresenceSubscriptions.BeforeKeyRemoved -= OnRemovePresenceSubscription;
        loginSession.PresenceSubscriptions.AfterValueUpdated -= OnUpdatedPresenceSubscription;
    }



    #region Subscription / Presence Events


    private void OnVivoxAddAllowedSubscription(AccountId accountId)
    {
        if (accountId != null)
        {
            VivoxSubscriptionAddAllowed?.Invoke(accountId);
        }
    }

    private void OnVivoxRemoveAllowedSubscription(AccountId accountId)
    {
        if (accountId != null)
        {
            VivoxSubscriptionRemoveAllowed?.Invoke(accountId);
        }
    }

    private void OnVivoxAddPresenceSubscription(AccountId accountId)
    {
        if (accountId != null)
        {
            VivoxSubscriptionAddPresence?.Invoke(accountId);
        }
    }

    private void OnVivoxRemovePresenceSubscription(AccountId accountId)
    {
        if (accountId != null)
        {
            VivoxSubscriptionRemovePresence?.Invoke(accountId);
        }
    }

    private void OnVivoxUpdatePresenceSubscription(ValueEventArg<AccountId, IPresenceSubscription> presence)
    {
        if (presence != null)
        {
            VivoxSubscriptionUpdatePresence?.Invoke(presence);
        }
    }

    private void OnVivoxAddBlockedSubscription(AccountId accountId)
    {
        if (accountId != null)
        {
            VivoxSubscriptionAddBlocked?.Invoke(accountId);
        }
    }

    private void OnVivoxRemoveBlockedSubscription(AccountId accountId)
    {
        if (accountId != null)
        {
            VivoxSubscriptionRemoveBlocked?.Invoke(accountId);
        }
    }


    #endregion



    #region Subscriptions / Presence Methods


    public void AddAllowedSubscription(string userName, ILoginSession loginSession, string tokenIssuer, string domain)
    {
        loginSession.BeginAddAllowedSubscription(new AccountId(tokenIssuer, userName, domain), ar =>
        {
            try
            {
                loginSession.EndAddAllowedSubscription(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }

    public void AddBlockedSubscription(string userName, ILoginSession loginSession, string tokenIssuer, string domain)
    {
        loginSession.BeginAddBlockedSubscription(new AccountId(tokenIssuer, userName, domain), ar =>
        {
            try
            {
                loginSession.EndAddBlockedSubscription(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }

    public void AddAllowPresence(string userName, ILoginSession loginSession, string tokenIssuer, string domain)
    {
        loginSession.BeginAddPresenceSubscription(new AccountId(tokenIssuer, userName, domain), ar =>
        {
            try
            {
                loginSession.EndAddPresenceSubscription(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }


    public void RemoveAllowedSubscription(string userName, ILoginSession loginSession, string tokenIssuer, string domain)
    {
        loginSession.BeginRemoveAllowedSubscription(new AccountId(tokenIssuer, userName, domain), ar =>
        {
            try
            {
                loginSession.EndRemoveAllowedSubscription(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }

    public void RemoveBlockedSubscription(string userName, ILoginSession loginSession, string tokenIssuer, string domain)
    {
        loginSession.BeginRemoveBlockedSubscription(new AccountId(tokenIssuer, userName, domain), ar =>
        {
            try
            {
                loginSession.EndRemoveBlockedSubscription(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }

    public void RemoveAllowedPresence(string userName, ILoginSession loginSession, string tokenIssuer, string domain)
    {
        loginSession.BeginRemovePresenceSubscription(new AccountId(tokenIssuer, userName, domain), ar =>
        {
            try
            {
                loginSession.EndRemovePresenceSubscription(ar);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }


    #endregion



    #region Subscription / Presence Callbacks


    private void OnAddAllowedSubscription(object sender, KeyEventArg<AccountId> keyArgs)
    {
        var source = (IReadOnlyHashSet<AccountId>)sender;

        OnVivoxAddAllowedSubscription(keyArgs.Key);
    }

    private void OnRemoveAllowedSubscription(object sender, KeyEventArg<AccountId> keyArgs)
    {
        var source = (IReadOnlyHashSet<AccountId>)sender;

        OnVivoxRemoveAllowedSubscription(keyArgs.Key);
    }

    private void OnAddPresenceSubscription(object sender, KeyEventArg<AccountId> keyArgs)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<AccountId, IPresenceSubscription>)sender;

        OnVivoxAddPresenceSubscription(keyArgs.Key);
    }

    private void OnRemovePresenceSubscription(object sender, KeyEventArg<AccountId> keyArgs)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<AccountId, IPresenceSubscription>)sender;

        OnVivoxRemovePresenceSubscription(keyArgs.Key);
    }

    private void OnUpdatedPresenceSubscription(object sender, ValueEventArg<AccountId, IPresenceSubscription> keyArgs)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<AccountId, IPresenceSubscription>)sender;

        OnVivoxUpdatePresenceSubscription(keyArgs);
    }

    private void OnAddBlockedSubscription(object sender, KeyEventArg<AccountId> keyArgs)
    {
        var source = (IReadOnlyHashSet<AccountId>)sender;

        OnVivoxAddBlockedSubscription(keyArgs.Key);
    }

    private void OnRemoveBlockedSubscription(object sender, KeyEventArg<AccountId> keyArgs)
    {
        var source = (IReadOnlyHashSet<AccountId>)sender;

        OnVivoxRemoveBlockedSubscription(keyArgs.Key);
    }

    private void OnIncomingSubscriptionRequests(object sender, QueueItemAddedEventArgs<AccountId> subRequests)
    {
        var source = (IReadOnlyQueue<AccountId>)sender;
        Debug.Log(subRequests.Value.DisplayName);
        while (source.Count > 0)
        {
            // todo check if it works
            Debug.Log($"Incoming subscription request from - {source.Dequeue().DisplayName}");
        }
    }


    #endregion




}
