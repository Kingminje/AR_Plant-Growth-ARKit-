using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniTimeSet : MonoBehaviour
{
    public float animSpeed = 5.0f;
    public Animator animator;
    private DateTime _nowLocalDateTime, _checkDateTime;

    private void Start()
    {
        animator.Play("New Animation", 0, 0.2f);
        _nowLocalDateTime = DateTime.Now;
        GetDate();

        Debug.Log("현재 로컬 시각 :" + _nowLocalDateTime);
        Debug.Log("저장된 시각 :" + _checkDateTime);

        TimeSpan calTime = _nowLocalDateTime - _checkDateTime;

        Debug.Log(calTime);
    }

    public void ButtonEvent()
    {
        animator.Play("New Animation", 0, 0.5f);
    }

    public void Q()
    {
        _nowLocalDateTime = DateTime.Now;
        PlayerPrefs.SetInt("Year", _nowLocalDateTime.Year);
        PlayerPrefs.SetInt("Month", _nowLocalDateTime.Month);
        PlayerPrefs.SetInt("Day", _nowLocalDateTime.Day);
        PlayerPrefs.SetInt("Hour", _nowLocalDateTime.Hour);
        PlayerPrefs.SetInt("Min", _nowLocalDateTime.Minute);
        PlayerPrefs.SetInt("Sec", _nowLocalDateTime.Second);
        PlayerPrefs.Save();

        Application.Quit();
    }

    public void GetDate()
    {
        if (_checkDateTime == null)
        {
            return;
        }
        else
        {
            int year = PlayerPrefs.GetInt("Year");
            int mon = PlayerPrefs.GetInt("Month");
            int day = PlayerPrefs.GetInt("Day");
            int hour = PlayerPrefs.GetInt("Hour");
            int min = PlayerPrefs.GetInt("Min");
            int sec = PlayerPrefs.GetInt("Sec");
            _checkDateTime = new DateTime(year, mon, day, hour, min, sec);
        }
    }
}