using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpriteAtlasProvider : MonoBehaviour
{
    public static SpriteAtlasProvider Instance { get; private set; }

    // Addressables에서 로드된 아틀라스 캐싱 (atlasName = key)
    private readonly Dictionary<string, SpriteAtlas> _atlasCache = new();

    // 이미 로딩 중인 요청 관리(중복 로딩 방지)
    private readonly Dictionary<string, List<Action<SpriteAtlas>>> _pendingRequests = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // SpriteAtlasManager의 공식 자동 로딩 이벤트 훅
        SpriteAtlasManager.atlasRequested += OnAtlasRequested;
        SpriteAtlasManager.atlasRegistered += OnAtlasRegistered;
    }

    private void OnDestroy()
    {
        SpriteAtlasManager.atlasRequested -= OnAtlasRequested;
        SpriteAtlasManager.atlasRegistered -= OnAtlasRegistered;
    }

    //--------------------------------------------------------------------
    // ① SpriteAtlasManager가 자동으로 호출하는 부분
    //--------------------------------------------------------------------
    private void OnAtlasRequested(string atlasName, Action<SpriteAtlas> callback)
    {
        // 이미 atlas가 로드되어 있으면 즉시 전달
        if (_atlasCache.TryGetValue(atlasName, out var cached))
        {
            callback?.Invoke(cached);
            return;
        }

        // 로딩 중이라면 대기열에 콜백 추가
        if (_pendingRequests.ContainsKey(atlasName))
        {
            _pendingRequests[atlasName].Add(callback);
            return;
        }

        // 처음 요청된 atlas → 리스트 생성 후 로딩 시작
        _pendingRequests.Add(atlasName, new List<Action<SpriteAtlas>> { callback });

        LoadAtlasAsync(atlasName);
    }

    //--------------------------------------------------------------------
    // ② Addressables 기반 Atlas 로딩
    //--------------------------------------------------------------------
    private async void LoadAtlasAsync(string atlasName)
    {
        var handle = Addressables.LoadAssetAsync<SpriteAtlas>(atlasName);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var atlas = handle.Result;

            _atlasCache[atlasName] = atlas;

            // 모든 대기 콜백 처리
            foreach (var cb in _pendingRequests[atlasName])
                cb?.Invoke(atlas);

            _pendingRequests.Remove(atlasName);

            Debug.Log($"[SpriteAtlasProvider] Loaded Atlas: {atlasName}");
        }
        else
        {
            Debug.LogError($"[SpriteAtlasProvider] Failed to load atlas: {atlasName}");
            _pendingRequests.Remove(atlasName);
        }
    }

    //--------------------------------------------------------------------
    // ③ Atlas 등록 이벤트 (SpriteAtlasManager 내부 시스템 활용)
    //--------------------------------------------------------------------
    private void OnAtlasRegistered(SpriteAtlas atlas)
    {
        if (!_atlasCache.ContainsKey(atlas.name))
        {
            _atlasCache.Add(atlas.name, atlas);
        }
    }

    //--------------------------------------------------------------------
    // ④ 직접 스프라이트 가져오는 API (선택적 사용)
    //--------------------------------------------------------------------
    public Sprite GetSprite(string atlasName, string spriteName)
    {
        if (_atlasCache.TryGetValue(atlasName, out var atlas))
        {
            return atlas.GetSprite(spriteName);
        }

        Debug.LogWarning($"[SpriteAtlasProvider] Atlas not loaded: {atlasName}");
        return null;
    }
    
    public void LoadAtlas(string atlasName)
    {
        var handle = Addressables.LoadAssetAsync<SpriteAtlas>(atlasName);
        handle.Completed += (op) =>
        {
            if (!op.IsValid() || op.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[SpriteAtlasProvider] Addressables 아틀라스 로드 실패: {atlasName}");
                //callback?.Invoke(null);
                return;
            }

            var atlas = op.Result;
            _atlasCache[atlasName] = atlas;
        };
    }
    
    
}
