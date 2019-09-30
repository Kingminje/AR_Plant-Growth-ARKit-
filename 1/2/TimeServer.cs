﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeServer : MonoBehaviour
{
    [SerializeField]
    private string _comment = "만료시킬 날짜를 적으세요 (한국시각 기준)";

    public int _yyyy, _mm, _dd;
    private DateTime _expireDateTime, _nowServerDateTime, _nowLocalDateTime;
    private TimeSpan _duration;

    private void Start()
    {
        StartGetTime();
    }

    public string[] StartGetTime()
    {
        // 한국 시각
        _duration = System.TimeSpan.FromHours(9);
        _expireDateTime = new DateTime(Mathf.Clamp(_yyyy, 1900, 3000), Mathf.Clamp(_mm, 1, 12), Mathf.Clamp(_dd, 1, 31));

        _nowLocalDateTime = DateTime.Now;
        _nowServerDateTime = GetNISTDate().Add(_duration);

        if (Debug.isDebugBuild)
        {
            Debug.LogWarning("만료지정일 : " + _expireDateTime);
            Debug.LogWarning("현재 로컬 시각 :" + _nowLocalDateTime);
            Debug.LogWarning("현재 서버 시각 :" + _nowServerDateTime);
        }

        if (_nowLocalDateTime < _expireDateTime)
        {
            if (_nowServerDateTime < _expireDateTime) // 현재 서버 시간이 만료 시간보다 크다면
            {
                Debug.Log("실행");
                //text.text = "실행";
            }
            else
            {
                //text.text = "서버 체크 결과 만료 됨";
                Debug.Log("서버 체크 결과 만료 됨");
            }
        }
        else
        {
            //text.text = _nowserverdatetime.tostring();
            Debug.Log("로컬 체크 결과 만료 됨");
        }

        var tmpstirngs = new string[3];
        tmpstirngs[0] = _expireDateTime.ToString();
        tmpstirngs[1] = _nowLocalDateTime.ToString();
        tmpstirngs[2] = _nowServerDateTime.ToString();

        return tmpstirngs;
    }

    #region NTPTIME

    //NTP time 을 NIST 에서 가져오기
    // 4초 이내에 한번 이상 요청 하면, ip가 차단됩니다.

    public static DateTime GetDummyDate()
    {
        return new DateTime(2019, 1, 24); //우리는 온라인 데이트 여부를 확인합니다.
    }

    public static DateTime GetNISTDate()
    {
        System.Random ran = new System.Random(DateTime.Now.Millisecond);
        DateTime date = GetDummyDate();
        string serverResponse = string.Empty;

        // NIST 서버 목록
        string[] servers = new string[] {
        "time.bora.net",
        //"time.nuri.net",
        //"ntp.kornet.net",
        //"time.kriss.re.kr",
        //"time.nist.gov",
        //"maths.kaist.ac.kr",
        "nist1-ny.ustiming.org",
        "time-a.nist.gov",
        "nist1-chi.ustiming.org",
        "time.nist.gov",
        "ntp-nist.ldsbc.edu",
        "nist1-la.ustiming.org"
        };

        // 너무 많은 요청으로 인한 차단을 피하기 위해 한 서버씩 순환한다. 5번만 시도한다.
        for (int i = 0; i < 5; i++)
        {
            try
            {
                // StreamReader(무작위 서버)
                StreamReader reader = new StreamReader(new System.Net.Sockets.TcpClient(servers[ran.Next(0, servers.Length)], 13).GetStream());
                serverResponse = reader.ReadToEnd();
                reader.Close();

                // 서버 리스폰스를 표시한다. (디버그 확인용)
                if (Debug.isDebugBuild)
                    Debug.Log(serverResponse);

                // 시그니처가 있는지 확인한다.
                if (serverResponse.Length > 47 && serverResponse.Substring(38, 9).Equals("UTC(NIST)"))
                {
                    // 날짜 파싱
                    int jd = int.Parse(serverResponse.Substring(1, 5));
                    int yr = int.Parse(serverResponse.Substring(7, 2));
                    int mo = int.Parse(serverResponse.Substring(10, 2));
                    int dy = int.Parse(serverResponse.Substring(13, 2));
                    int hr = int.Parse(serverResponse.Substring(16, 2));
                    int mm = int.Parse(serverResponse.Substring(19, 2));
                    int sc = int.Parse(serverResponse.Substring(22, 2));

                    if (jd > 51544)
                        yr += 2000;
                    else
                        yr += 1999;

                    date = new DateTime(yr, mo, dy, hr, mm, sc);

                    // Exit the loop
                    break;
                }
            }
            catch (Exception e)
            {
                /* 아무것도 하지 않고 다음 서버를 시도한다. */
            }
        }
        return date;
    }

    #endregion NTPTIME
}