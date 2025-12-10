using UnityEngine;
using System;
using System.Collections;

public class AttendanceSystem : Singleton<AttendanceSystem>
{
    private AttendanceData data;

    void Start()
    {
        Debug.Log("AttendanceSystem Start");
        StartCoroutine(CheckAttendance());
    }

    IEnumerator CheckAttendance()
    {
        // 1) 서버 시간 불러오기
        DateTime today = DateTime.UtcNow;
        
        if(TimeManager.Instance == null)
        {
            Debug.LogError("TimeManager 인스턴스가 없습니다.");
        }
        
        yield return StartCoroutine(TimeManager.Instance.GetServerTime(t => today = t.Date));

        // 2) 저장된 데이터 로드
        data = AttendanceStorage.Load();

        // 3) 이미 출석한 날인가?
        if (data.lastAttendanceDate == today)
        {
            Debug.Log("오늘 이미 출석함");
            yield break;
        }

        // 4) 연속 출석 계산
        if (data.lastAttendanceDate == today.AddDays(-1))
        {
            data.streak++;
        }
        else
        {
            data.streak = 1;
        }

        // 5) 보상 지급
        GiveReward(data.streak);

        // 6) 저장
        data.lastAttendanceDate = today;
        AttendanceStorage.Save(data.lastAttendanceDate, data.streak);
    }

    void GiveReward(int streak)
    {
        Debug.Log("출석 보상 지급! 연속: " + streak);

        // 여기서 실제 아이템 지급 로직 실행
        // Ex: Inventory.AddItem("Gem", 10);
    }
}