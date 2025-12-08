using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TimeManager : Singleton<TimeManager>
{
    const string TIME_API = "https://worldtimeapi.org/api/timezone/Etc/UTC";

    public IEnumerator GetServerTime(Action<DateTime> callback)
    {
        UnityWebRequest req = UnityWebRequest.Get(TIME_API);
        req.timeout = 3;

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            // 실패 시 로컬 시간 fallback
            callback(DateTime.UtcNow);
            yield break;
        }

        var json = req.downloadHandler.text;
        var wrapper = JsonUtility.FromJson<TimeApiResponse>(json);

        DateTime serverTime = DateTime.Parse(wrapper.datetime);
        callback(serverTime);
    }
}

[Serializable]
public class TimeApiResponse
{
    public string datetime;
}