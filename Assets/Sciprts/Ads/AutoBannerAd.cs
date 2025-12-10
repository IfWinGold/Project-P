using UnityEngine;
using UnityEngine.Advertisements;

public class AutoBannerAd : MonoBehaviour
{
    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";
    private string _adUnitId = null;

    void Start()
    {
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

        // 배너 위치 세팅
        Advertisement.Banner.SetPosition(_bannerPosition);

        // 자동 로드 시작
        LoadBanner();
    }

    private void LoadBanner()
    {
        BannerLoadOptions loadOptions = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_adUnitId, loadOptions);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner loaded! Auto showing.");

        BannerOptions showOptions = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            showCallback = OnBannerShown,
            hideCallback = OnBannerHidden
        };

        Advertisement.Banner.Show(_adUnitId, showOptions);
    }

    private void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");

        // 기본적으로 실패하면 일정 시간 뒤 재시도
        Invoke(nameof(LoadBanner), 2f);
    }

    private void OnBannerClicked()  => Debug.Log("Banner Clicked");
    private void OnBannerShown()    => Debug.Log("Banner Shown");
    private void OnBannerHidden()   => Debug.Log("Banner Hidden");

    private void OnDestroy()
    {
        Advertisement.Banner.Hide();
    }
}