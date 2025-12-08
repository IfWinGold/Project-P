using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

public class BGMManager : Singleton<BGMManager>
{
    [Header("BGM Settings")]
    public float defaultFadeTime = 1.5f;
    public float maxVolume = 1f;

    private AudioSource _activeSource;
    private AudioSource _inactiveSource;

    // Addressables 캐싱
    private Dictionary<string, AudioClip> _clipCache = new Dictionary<string, AudioClip>();

    protected override void Awake()
    {
        base.Awake();

        _activeSource = gameObject.AddComponent<AudioSource>();
        _inactiveSource = gameObject.AddComponent<AudioSource>();

        _activeSource.loop = true;
        _inactiveSource.loop = true;

        _activeSource.volume = 0f;
        _inactiveSource.volume = 0f;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 초기 BGM 재생 (필요시)
        PlayAsync("Audio/HomeBGM");
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     ChangeBGMAsync("Audio/HomeBGM");
        //     Debug.Log("Input Alpha1");
        // }
    }

    // ====================================================
    // 🔥 로컬 클립 재생 (페이드 인)
    // ====================================================
    public void Play(AudioClip clip, float fadeTime = -1f)
    {
        if (fadeTime < 0) fadeTime = defaultFadeTime;

        _activeSource.clip = clip;
        _activeSource.volume = 0f;
        _activeSource.Play();

        _activeSource.DOFade(maxVolume, fadeTime);
    }
    public void PlayAsync(string addressKey, float fadeTime = -1f)
    {
        var handle = Addressables.LoadAssetAsync<AudioClip>(addressKey);
        handle.Completed += (op) =>
        {
            if (!op.IsValid() || op.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[BGMManager] Addressables 클립 로드 실패: {addressKey}");
                return;
            }

            Play(op.Result, fadeTime);
        };
    }

    // ====================================================
    // 🔥 Addressables 비동기 로드 + 크로스페이드
    // ====================================================
    public async void ChangeBGMAsync(string addressKey, float fadeTime = -1f)
    {
        if (fadeTime < 0) fadeTime = defaultFadeTime;

        // 이미 로드된 클립은 캐시에서 재사용
        AudioClip clip;

        if (_clipCache.ContainsKey(addressKey))
        {
            clip = _clipCache[addressKey];
        }
        else
        {
            var handle = Addressables.LoadAssetAsync<AudioClip>(addressKey);
            await handle.Task;

            if (!handle.IsValid() || handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[BGMManager] Addressables 클립 로드 실패: {addressKey}");
                return;
            }

            clip = handle.Result;
            _clipCache[addressKey] = clip;
        }

        CrossFade(clip, fadeTime);
    }

    // ====================================================
    // 🔥 CrossFade (끊기지 않는 자연스러운 전환)
    // ====================================================
    private void CrossFade(AudioClip newClip, float fadeTime)
    {
        // swap source
        var old = _activeSource;
        var next = _inactiveSource;

        _activeSource = next;
        _inactiveSource = old;

        // 새 곡 준비
        _activeSource.clip = newClip;
        _activeSource.volume = 0f;
        _activeSource.Play();

        // DOTween 크로스페이드
        _activeSource.DOFade(maxVolume, fadeTime);
        _inactiveSource.DOFade(0f, fadeTime).OnComplete(() =>
        {
            _inactiveSource.Stop();
        });
    }

    // ====================================================
    // 🔥 BGM 정지 (페이드 아웃)
    // ====================================================
    public void StopBGM(float fadeTime = -1f)
    {
        if (fadeTime < 0) fadeTime = defaultFadeTime;

        _activeSource.DOFade(0f, fadeTime)
            .OnComplete(() => _activeSource.Stop());
    }
}
