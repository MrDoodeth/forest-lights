using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

public class InterstitialController : MonoBehaviour
{
    [SerializeField] private float adCallbackTime = 30f;
    private InterstitialAdLoader _interstitialAdLoader;
    private Interstitial _interstitial;
    private const string AdUnitId = "R-M-11232119-2";

    private static float _currentElapsedTime;

    public void Awake()
    {
        SetupLoader();
        RequestInterstitial();
    }

    private void SetupLoader()
    {
        this._interstitialAdLoader = new InterstitialAdLoader();
        this._interstitialAdLoader.OnAdLoaded += this.HandleAdLoaded;
    }
    private void RequestInterstitial()
    {
        //Sets COPPA restriction for user age under 13
        MobileAds.SetAgeRestrictedUser(true);

        // Replace demo Unit ID 'demo-interstitial-yandex' with actual Ad Unit ID
        string adUnitId = AdUnitId;

        if (this._interstitial != null)
        {
            this._interstitial.Destroy();
        }

        this._interstitialAdLoader.LoadAd(this.CreateAdRequest(adUnitId));
    }

    public void ShowInterstitial()
    {
        if (_currentElapsedTime >= adCallbackTime)
        {
            Debug.LogWarning("Show interstitial");
            this._interstitial?.Show();
            _currentElapsedTime = 0f;
        }
        else
        {
            Debug.LogWarning($"Wait {adCallbackTime - _currentElapsedTime}");
        }
        
    }

    private void DestroyInterstitialAd()
    {
        if (_interstitial != null)
        {
            _interstitial.Destroy();
            _interstitial = null;
        }
    }
    private AdRequestConfiguration CreateAdRequest(string adUnitId)
    {
        return new AdRequestConfiguration.Builder(adUnitId).Build();
    }

    #region Interstitial callback handlers

    public void HandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
    {
        this._interstitial = args.Interstitial;

        this._interstitial.OnAdFailedToShow += this.HandleAdFailedToShow;
        this._interstitial.OnAdDismissed += this.HandleAdDismissed;
    }
    
    public void HandleAdDismissed(object sender, EventArgs args)
    {
        DestroyInterstitialAd();
        RequestInterstitial();
    }
    
    public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
    {
        DestroyInterstitialAd();
        RequestInterstitial();
    }
    #endregion

    private void Update()
    {
        _currentElapsedTime += Time.deltaTime;
    }
}