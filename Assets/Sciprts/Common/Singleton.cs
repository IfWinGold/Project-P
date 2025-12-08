using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에서 싱글톤 탐색
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    Debug.LogError($"[Singleton] {typeof(T).Name} 싱글톤이 씬에 존재하지 않습니다. 반드시 씬에 추가해야 합니다.");
                    return null;
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            //DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[Singleton] {typeof(T).Name} 중복 인스턴스가 발견되어 제거됨.");
            Destroy(gameObject);
        }
        Debug.Log("Singleton, Awake");
    }
    

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}