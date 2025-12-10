using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        // 씬 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void Start()
    {
        BGMManager.Instance.PlayAsync("Audio/HomeBGM");
    }

    private void OnDestroy()
    {
        // 씬 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    // ✔ 새로운 씬이 로드되었을 때
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SceneChangeManager] Loaded: {scene.name}, Mode: {mode}");
        HandleSceneLoad(scene);
    }

    // ✔ 씬이 언로드되기 직전에
    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"[SceneChangeManager] Unloaded: {scene.name}");
        HandleSceneUnload(scene);
    }

    // ✔ 현재 활성 씬이 변경될 때
    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        Debug.Log($"[SceneChangeManager] Active Scene Changed: {oldScene.name} → {newScene.name}");
        HandleActiveSceneChanged(oldScene, newScene);
    }

    // ----------------------------
    // 🔥 여기부터 너가 원하는 행동 정의
    // 씬별 처리하고 싶으면 switch-case로 분기
    // ----------------------------

    private void HandleSceneLoad(Scene scene)
    {
        switch (scene.name)
        {
            case "HomeScene":
            {
                BGMManager.Instance.ChangeBGMAsync("Audio/HomeBGM");
                break;
            }
            case "LevelScene":
            {
                BGMManager.Instance.ChangeBGMAsync("Audio/LevelBGM");
                SpriteAtlasProvider.Instance.LoadAtlas("GameAtlas");
                break;
            }
            case "GameScene":
            {
                BGMManager.Instance.ChangeBGMAsync("Audio/GameBGM");
                break;
            }
            // default: 아무것도 안함
        }
    }

    private void HandleSceneUnload(Scene scene)
    {
        // 예: 특정 씬의 데이터 저장
    }

    private void HandleActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        // 씬 변경 트랜지션 효과 등
    }
}