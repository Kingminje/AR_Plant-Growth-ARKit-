using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeatherManger : MonoBehaviour
{
    public GetGPS getGPS;
    public GetMyWeather getMyWeather;
    public GetText getText;
    public TimeServer getTime;

    //public Transform stalk;

    //private string savedcity = "Seoul";

    public bool testMod = false;

    private string lang, lon;

    private void Start()
    {
        GPSUpdate();
    }

    public void GPSUpdate()
    {
        getGPS.StartGPS();  //디버그용
        /*getGPS.Lang = 12;*/ //디버그
        /*getGPS.Lon = 12;*/ // 용
        StartCoroutine(GPSReset());
    }

    private IEnumerator GPSReset()
    {
        if (getGPS.Lon != 0)
        {
            lang = getGPS.Lang.ToString();
            lon = getGPS.Lon.ToString();

            getMyWeather.GetyWeatherString(lang, lon); //디버그용
            //getMyWeather.GetyWeatherString("13", "12");
            testMod = true;
            StartCoroutine(SetText());
        }
        else
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(GPSReset());
        }
    }

    private IEnumerator SetText()
    {
        new WaitForSeconds(1f);
        if (testMod)
        {
            var Times = getTime.StartGetTime(); // 만료지정일, 로컬시간, 서버시간
            var timeSet = getMyWeather.GetyWeatherString(lang, lon);

            //timeSet[0] += (Times[1]);

            getText.TextSet(timeSet, Times[1]);
        }
        else
        {
            StartCoroutine(SetText());
        }

        return null;
    }

    private string HangulConverter(string ConverterCity)
    {
        switch (ConverterCity)
        {
            case "light rain":
                return "낮은 비";

            case "sky is clear":
                return "매우 맑은 날씨";

            case "broken clouds":
                return "구름 없는 맑은 날씨";

            case "moderate rain":
                return "보통의 비";

            //case "moderate rain":
            //    return "맑은날씨";

            //case "light rain":
            //    return "맑은날씨";

            case "clear sky":
                return "맑은 날씨";

                //case:
        }

        return ConverterCity;
    }

    public void up(Transform stalk)
    {
        //stalk.lossyScale.x =
    }

    //private IEnumerator SetTime()
    //{
    //    new WaitForSeconds(60f);

    //    getGPS.
    //}

    //public bool ResetGameManger()
    //{
    //    //for (int i = 0; i < textMeshPros.Length; i++)
    //    //{
    //    //    textMeshPros[i]
    //    //}
    //}

    //public void Selectcity()
    //{
    //    PlayerPrefs.DeleteAll();
    //    //var tmpCity = myWeather.GetyWeatherString("");
    //    //PlayerPrefs.SetString("city", tmpCity);

    //    //text.text = HangulConverter(tmpCity);
    //    Debug.Log("성공");
    //}

    //// Update is called once per frame
    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0) && testMod == true)
    //    {
    //        Selectcity();
    //    }
    //}
}