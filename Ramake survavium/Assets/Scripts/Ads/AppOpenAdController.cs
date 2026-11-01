using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

public class AppOpenAdController : MonoBehaviour
{
    private AppOpenAdLoader _appOpenAdLoader;
    private AppOpenAd _appOpenAd;
    private const string AdUnitId = "R-M-11232119-1";
    private static bool _isAdShowOnColdStart;

    public void Awake()
    {
        SetupLoader();
        RequestAppOpenAd();
        
        // Use the AppStateObserver to listen to application open/close events.
        AppStateObserver.OnAppStateChanged += HandleAppStateChanged;
    }

    private void SetupLoader()
    {
        this._appOpenAdLoader = new AppOpenAdLoader();
        this._appOpenAdLoader.OnAdLoaded += this.HandleAdLoaded;
    }
    public void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks.
        AppStateObserver.OnAppStateChanged -= HandleAppStateChanged;
    }
    
    private void RequestAppOpenAd()
    {
        //Sets COPPA restriction for user age under 13
        MobileAds.SetAgeRestrictedUser(true);

        // Replace demo Unit ID 'demo-appOpenAd-yandex' with actual Ad Unit ID
        string adUnitId = AdUnitId;

        this._appOpenAd?.Destroy();

        this._appOpenAdLoader.LoadAd(this.CreateAdRequestConfiguration(adUnitId));
    }

    private void ShowAppOpenAd()
    {
        this._appOpenAd?.Show();
    }

    private void DestroyAppOpenAd()
    {
        if (_appOpenAd != null)
        {
            this._appOpenAd.Destroy();
            this._appOpenAd = null;
        }
    }

    private AdRequestConfiguration CreateAdRequestConfiguration(string adUnitId)
    {
        return new AdRequestConfiguration.Builder(adUnitId).Build();
    }
    
    #region AppOpenAd callback handlers

    public void HandleAppStateChanged(object sender, AppStateChangedEventArgs args)
    {
        if (!args.IsInBackground)
        {
            ShowAppOpenAd();
        }
    }

    public void HandleAdLoaded(object sender, AppOpenAdLoadedEventArgs args)
    {
        this._appOpenAd = args.AppOpenAd;
        
        this._appOpenAd.OnAdFailedToShow += this.HandleAdFailedToShow;
        this._appOpenAd.OnAdDismissed += this.HandleAdDismissed;

        if (!_isAdShowOnColdStart)
        {
            ShowAppOpenAd();
            _isAdShowOnColdStart = true;
        }
    }

    public void HandleAdDismissed(object sender, EventArgs args)
    {
        DestroyAppOpenAd();
        RequestAppOpenAd();
    }
    
    public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
    {
        DestroyAppOpenAd();
        RequestAppOpenAd();
    }
    #endregion
}
