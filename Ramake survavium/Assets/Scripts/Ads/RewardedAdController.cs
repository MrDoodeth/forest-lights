using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

public class RewardedAdController : MonoBehaviour
{
    private RewardedAdLoader rewardedAdLoader;
    private RewardedAd rewardedAd;
    private const string AdUnitId = "R-M-11232119-3";
    public static Action<Reward> OnGotRewarded;
    
    public void Awake()
    {
        SetupLoader();
        RequestRewardedAd();
    }

    private void SetupLoader()
    {
        this.rewardedAdLoader = new RewardedAdLoader();
        this.rewardedAdLoader.OnAdLoaded += this.HandleAdLoaded;
    }
    private void RequestRewardedAd()
    {
        //Sets COPPA restriction for user age under 13
        MobileAds.SetAgeRestrictedUser(true);

        // Replace demo Unit ID 'demo-rewarded-yandex' with actual Ad Unit ID
        string adUnitId = AdUnitId;

        this.rewardedAdLoader.LoadAd(this.CreateAdRequest(adUnitId));
    }

    private void DestroyRewardedAd()  
    {
        if (rewardedAd != null)
        {
            this.rewardedAd.Destroy();
            this.rewardedAd = null;
        }
    }

    public void ShowRewardedAd()
    {
        if (this.rewardedAd != null)
        {
            this.rewardedAd.Show();
        }
    }
    private AdRequestConfiguration CreateAdRequest(string adUnitId)
    {
        return new AdRequestConfiguration.Builder(adUnitId).Build();
    }

    #region Rewarded Ad callback handlers
    public void HandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
    {
        this.rewardedAd = args.RewardedAd;
        
        this.rewardedAd.OnAdFailedToShow += this.HandleAdFailedToShow;
        this.rewardedAd.OnAdDismissed += this.HandleAdDismissed;
        this.rewardedAd.OnAdImpression += this.HandleImpression;
        this.rewardedAd.OnRewarded += this.HandleRewarded;
    }
    
    public void HandleAdDismissed(object sender, EventArgs args)
    {
        DestroyRewardedAd();
        RequestRewardedAd();
    }
    public void HandleRewarded(object sender, Reward args)
    {
        OnGotRewarded?.Invoke(args);
    }

    public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
    {
        DestroyRewardedAd();
        RequestRewardedAd();
    }
    
    public void HandleImpression(object sender, ImpressionData impressionData)
    {
        var data = impressionData == null ? "null" : impressionData.rawData;
        print(data);
    }
    #endregion
}
