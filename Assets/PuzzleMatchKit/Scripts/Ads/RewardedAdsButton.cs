using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class RewardedAdsButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";

    public UnityEvent onRewardComplete;

    string _adUnitId = null;
    bool _isAdReady = false;      // 광고 준비 여부
    bool _isInitialized = false;  // Ads 초기화 여부

    void Awake()
    {
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

        if (_showAdButton == null)
        {
            Debug.LogError("RewardedAdsButton: ShowAdButton is NULL!");
            enabled = false;
            return;
        }

        _showAdButton.interactable = false;
        _showAdButton.onClick.RemoveAllListeners();
        _showAdButton.onClick.AddListener(ShowAd);
    }

    private void OnEnable()
    {
        TryLoadAd();
    }

    private void TryLoadAd()
    {
        // Unity Ads 초기화 체크
        if (!Advertisement.isInitialized)
        {
            Debug.Log("Ads not initialized yet. Waiting...");
            // 초기화 스크립트에서 Init 완료 후 다시 LoadAd() 호출해줘야 함.
            return;
        }

        LoadAd();
    }

    public void LoadAd()
    {
        if (_isAdReady)
            return;

        Debug.Log($"Loading Ad: {_adUnitId}");
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string loadedAdUnitId)
    {
        if (!loadedAdUnitId.Equals(_adUnitId)) return;

        Debug.Log("Rewarded Ad Loaded Successfully");
        _isAdReady = true;

        _showAdButton.interactable = true;
    }

    public void ShowAd()
    {
        if (!_isAdReady)
        {
            Debug.LogWarning("Rewarded Ad NOT ready.");
            return;
        }

        _showAdButton.interactable = false;
        _isAdReady = false;

        Debug.Log("Showing Rewarded Ad...");
        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState state)
    {
        if (adUnitId.Equals(_adUnitId))
        {
            if (state == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("Reward Completed");
                onRewardComplete?.Invoke();
            }

            // 광고 한 번 보면 반드시 다시 로드해야 함
            Debug.Log("Reloading Rewarded Ad...");
            LoadAd();
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Failed To Load Rewarded Ad: {error} | {message}");
        _isAdReady = false;

        // 잠시 후 재시도
        Invoke(nameof(LoadAd), 2f);
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Failed To Show Rewarded Ad: {error} | {message}");
        _isAdReady = false;

        // 오류 시 재로드
        LoadAd();
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        if (_showAdButton != null)
            _showAdButton.onClick.RemoveListener(ShowAd);
    }
}
