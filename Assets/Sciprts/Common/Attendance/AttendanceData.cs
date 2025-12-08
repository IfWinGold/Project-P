using UnityEngine;
using System;

public class AttendanceData
{
    public DateTime lastAttendanceDate;
    public int streak; // 연속 출석 일수
}

public static class AttendanceStorage
{
    private const string KEY_DATE = "ATT_LAST";
    private const string KEY_STREAK = "ATT_STREAK";

    public static void Save(DateTime date, int streak)
    {
        PlayerPrefs.SetString(KEY_DATE, date.ToString("yyyy-MM-dd"));
        PlayerPrefs.SetInt(KEY_STREAK, streak);
        PlayerPrefs.Save();
    }

    public static AttendanceData Load()
    {
        AttendanceData data = new AttendanceData();

        if (PlayerPrefs.HasKey(KEY_DATE))
        {
            data.lastAttendanceDate = DateTime.Parse(PlayerPrefs.GetString(KEY_DATE));
            data.streak = PlayerPrefs.GetInt(KEY_STREAK);
        }
        else
        {
            data.lastAttendanceDate = DateTime.MinValue;
            data.streak = 0;
        }

        return data;
    }
}