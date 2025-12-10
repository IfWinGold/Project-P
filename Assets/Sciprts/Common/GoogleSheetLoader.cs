using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetLoader : Singleton<GoogleSheetLoader>
{
    [Header("Google Sheet CSV URL")]
    public string sheetUrl = "https://docs.google.com/spreadsheets/d/XXXXXX/gviz/tq?tqx=out:csv&sheet=ItemData";

    void Start()
    {
        StartCoroutine(LoadSheetSafe());
    }

    IEnumerator LoadSheetSafe()
    {
        // 1) 기본 네트워크 체크
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogError("인터넷 연결 없음 (기본 Reachability 체크 실패)");
            yield break;
        }

        // 2) 실제 인터넷 연결 여부 HTTP 체크
        bool connected = false;
        yield return StartCoroutine(CheckInternetHTTP(result => connected = result));

        if (!connected)
        {
            Debug.LogError("인터넷은 연결됐지만 실제 요청 불가 (방화벽/연결 차단)");
            yield break;
        }

        // 3) 연결 OK → Google Sheet 가져오기
        yield return StartCoroutine(LoadSheet());
    }

    IEnumerator CheckInternetHTTP(System.Action<bool> callback)
    {
        UnityWebRequest req = UnityWebRequest.Get("https://www.google.com/generate_204");
        req.timeout = 3;
        yield return req.SendWebRequest();

        bool success = req.result == UnityWebRequest.Result.Success;
        callback(success);
    }

    IEnumerator LoadSheet()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(sheetUrl))
        {
            www.timeout = 5;

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[GoogleSheet] 요청 실패: " + www.error);
                yield break;
            }

            string csv = www.downloadHandler.text;
            Debug.Log("CSV Loaded:\n" + csv);

            List<string[]> rows = ParseCSV(csv);
            foreach (var row in rows)
            {
                Debug.Log(string.Join(" / ", row));
            }
        }
    }

    List<string[]> ParseCSV(string csv)
    {
        List<string[]> rows = new List<string[]>();
        string[] lines = csv.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] values = line.Split(',');
            rows.Add(values);
        }

        return rows;
    }
}
