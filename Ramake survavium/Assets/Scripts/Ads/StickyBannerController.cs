using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

public class StickyBannerController : MonoBehaviour
{
    private Banner _banner;
    private const string AdUnitId = "demo-banner-yandex";

    private void Awake()
    {
        RequestBanner();
    }
    private void RequestBanner()
    {
        //Sets COPPA restriction for user age under 13
        MobileAds.SetAgeRestrictedUser(true);

        // Replace demo Unit ID 'demo-banner-yandex' with actual Ad Unit ID
        string adUnitId = AdUnitId;

        if (this._banner != null)
        {
            this._banner.Destroy();
        }
        
        // Set sticky banner width
        BannerAdSize bannerSize = BannerAdSize.StickySize(GetScreenWidthDp());
        // Or set inline banner maximum width and height
        
        // BannerAdSize bannerSize = BannerAdSize.InlineSize(GetScreenWidthDp(), 300);
        this._banner = new Banner(adUnitId, bannerSize, AdPosition.BottomCenter);

        this._banner.OnAdLoaded += this.HandleAdLoaded;

        this._banner.LoadAd(this.CreateAdRequest());
    }

    // Example how to get screen width for request
    private int GetScreenWidthDp()
    {
        int screenWidth = (int)Screen.safeArea.width;
        return ScreenUtils.ConvertPixelsToDp(screenWidth);
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder().Build();
    }
    
    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        this._banner.Show();
    }
    #endregion
}