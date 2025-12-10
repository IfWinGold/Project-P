using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }
    
    [SerializeField] private RewardedAdsButton[] rewardedAdsButtons;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InitializeAds()
    {
        Debug.Log("AdsManager: InitializeAds called. TODO: Plug Unity Ads or AdMob initialization here.");
        // Example Unity Ads initialization placeholder:
        // #if UNITY_ADS
        //   Advertisement.Initialize(gameId, testMode);
        // #endif
        
        foreach( var button in rewardedAdsButtons)
        {
            button.LoadAd();
        }
    }

    public void OnLevelStart()
    {
        Debug.Log("AdsManager: OnLevelStart - preload ads if needed.");
    }

    public void OnLevelFail()
    {
        Debug.Log("AdsManager: OnLevelFail - consider showing interstitial or rewarded ad.");
    }

    public void OnLevelComplete()
    {
        Debug.Log("AdsManager: OnLevelComplete - consider showing interstitial or reward opportunity.");
    }

    public void ShowInterstitial(Action onClosed = null)
    {
        Debug.Log("AdsManager: ShowInterstitial called.");
        onClosed?.Invoke();
        // TODO: Replace with Unity Ads or AdMob interstitial show logic.
    }

    public void ShowRewarded(Action onRewarded = null)
    {
        Debug.Log("AdsManager: ShowRewarded called.");
        onRewarded?.Invoke();
        // TODO: Replace with Unity Ads or AdMob rewarded logic and reward callback.
    }
}
