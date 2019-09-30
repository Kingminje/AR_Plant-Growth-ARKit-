using UnityEngine;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

using WeatherApp;
using System;

public class GetMyWeather : MonoBehaviour
{
    public string[] GetyWeatherString(string lang, string lon)
    {
        WeatherData weatherData = new WeatherData(lang, lon);
        weatherData.CheckWeather();

        string[] a = new string[6];

        a[0] = weatherData.City; // 도시
        a[1] = weatherData.Temp.ToString();// 평균기온
        a[2] = weatherData.Weather; // 날씨
        //a[3] = weatherData.WindSpeed.ToString();
        //a[4] = weatherData.WindDirectionName;
        //a[5] = weatherData.Humidity.ToString();

        foreach (var item in a)
        {
            Debug.Log(item);
        }

        return a;
    }
}

namespace WeatherApp
{
    internal class WeatherData
    {
        public WeatherData(string lang, string lon)
        {
            Lang = lang;
            Lon = lon;
        }

        private string city; // 도시
        private string lang; // 위도
        private string lon; // 경도
        private float temp; // 평균온도
        private float tempMax; // 고온
        private float tempMin; // 저온
        private float windSpeed; // 바람세기
        private string windDirectionName; // 바람방향
        private float windDirectionValue;
        private float humidity;
        private string weather; // 날씨

        public string City
        {
            get { return city; }
            set { city = value; }
        } // 도시

        public string Lang
        {
            get { return lang; }
            set { lang = value; }
        } // 위도

        public string Lon
        {
            get { return lon; }
            set { lon = value; }
        } // 경도

        public float Temp
        {
            get { return temp; }
            set { temp = value; }
        } // 평균 기온

        public float TempMax
        {
            get { return tempMax; }
            set { tempMax = value; }
        } // 최대 기온

        public float TempMin
        {
            get { return tempMin; }
            set { tempMin = value; }
        } //최소 기온

        public float WindSpeed
        {
            get { return windSpeed; }
            set { windSpeed = value; }
        } //  바람 초속 m/s

        public string WindDirectionName
        {
            get { return windDirectionName; }
            set { windDirectionName = value; }
        } //  바람 방향 이름

        public float WindDirectionValue
        {
            get { return windDirectionValue; }
            set { windDirectionValue = value; }
        } //바람 방향 값

        public float Humidity
        {
            get { return humidity; }
            set { humidity = value; }
        } //습도

        public string Weather
        {
            get { return weather; }
            set { weather = value; }
        } //날씨

        public void CheckWeather()
        {
            WeatherAPI DataAPI = new WeatherAPI(Lang, Lon);

            weather = DataAPI.GetWeather();

            city = DataAPI.GetCity();
            temp = DataAPI.GetTemp();
            windSpeed = DataAPI.GetWindSpeed();
            windDirectionName = DataAPI.GetWindDirectionName();
            windDirectionValue = DataAPI.GetWindDirectionValue();
            humidity = DataAPI.GetHumidity();
        }
    }

    internal class WeatherAPI
    {
        public WeatherAPI(string lang, string lon)
        {
            //SetCurrentURL(city); //테스트 모드 시

            SetCurrentURL2(lang, lon);
            xmlDocument = GetXML(CurrentURL);
        }

        public string GetCity()
        {
            XmlNode tmp_node = xmlDocument.SelectSingleNode("//city");
            XmlAttribute temp_value = tmp_node.Attributes["name"];
            string tmp_string = temp_value.Value;

            return tmp_string;
        }

        public string GetWeather()
        {
            XmlNode tmp_node = xmlDocument.SelectSingleNode("//weather");
            XmlAttribute temp_value = tmp_node.Attributes["value"];
            string tmp_string = temp_value.Value;

            return tmp_string;
        }

        public float GetTemp()
        {
            XmlNode tmp_node = xmlDocument.SelectSingleNode("//temperature");
            XmlAttribute temp_value = tmp_node.Attributes["value"];
            string tmp_string = temp_value.Value;

            return float.Parse(tmp_string);
        }

        public float GetWindSpeed()
        {
            XmlNode tmp_node = xmlDocument.SelectSingleNode("//wind").SelectSingleNode("//speed");
            XmlAttribute temp_value = tmp_node.Attributes["value"];
            string tmp_string = temp_value.Value;

            return float.Parse(tmp_string);
        }

        public string GetWindDirectionName()
        {
            XmlNode tmp_node = xmlDocument.SelectSingleNode("//weather").SelectSingleNode("//direction")/*.SelectSingleNode("//direction")*/;
            XmlAttribute temp_value = tmp_node.Attributes["name"];
            string tmp_string = temp_value.Value;

            return tmp_string;
        }

        public float GetWindDirectionValue()
        {
            XmlNode tmp_node = xmlDocument.SelectSingleNode("//weather").SelectSingleNode("//direction")/*.SelectSingleNode("//direction")*/;
            XmlAttribute temp_value = tmp_node.Attributes["value"];
            string tmp_string = temp_value.Value;

            return float.Parse(tmp_string);
        }

        public float GetHumidity()
        {
            XmlNode tmp_node = xmlDocument.SelectSingleNode("//humidity ");
            XmlAttribute temp_value = tmp_node.Attributes["value"];
            string tmp_string = temp_value.Value;

            return float.Parse(tmp_string);
        }

        private const string APIKEY = "2c8d9bceb95b835d45fddf048889d9b6";

        private string CurrentURL;
        private XmlDocument xmlDocument;

        private void SetCurrentURL(string location)
        {
            CurrentURL = "http://api.openweathermap.org/data/2.5/weather?q="
                + location + "&mode=xml&units=metric&APPID=" + APIKEY;
        }

        private void SetCurrentURL2(string lang, string lon)
        {
            CurrentURL = "http://api.openweathermap.org/data/2.5/weather?lat=" + lang + "&lon=" + lon + "&&mode=xml&units=metric" + "&APPID=" + APIKEY;
            //Debug.Log(CurrentURL);
        }

        private XmlDocument GetXML(string CurrentURL)
        {
            using (WebClient client = new WebClient())
            {
                string xmlContent = client.DownloadString(CurrentURL);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlContent);
                return xmlDocument;
            }
        }
    }
}

//public void GetyWeatherString(string lang, string lon)
//{
//    Debug.Log(0);
//    //if (lang == "0")
//    //{
//    //    InvokeRepeating("GetyWeatherString", 0.0001f, 1.0f);
//    //    Debug.Log(1);
//    //}
//    //else
//    //{
//    Debug.Log(2);
//    WeatherData weatherData = new WeatherData(lang, lon);
//    weatherData.CheckWeather();
//    Debug.Log(6);
//    //getText.textMesh.text += weatherData.WindDirectionValue.ToString() + "°"
//    //    + "/" + weatherData.WindDirectionName + "/" + weatherData.WindSpeed.ToString() + "m/s" + "/" + weatherData.Weather + "/" +
//    //    weatherData.City + "/" + weatherData.Temp.ToString();

//    //}

//    //getGPS.text_latitude.text = weatherData.WindDirectionValue.ToString() + "°";
//    //getGPS.text_longitude.text = weatherData.WindSpeed.ToString() + "m/s";
//    //getGPS.text_refresh.text = weatherData.Weather; // 날씨 전달

//    //getGPS.text_latitude.text = weatherData.City;
//    //getGPS.text_longitude.text = weatherData.Temp.ToString();
//    //getGPS.text_longitude.text = weatherData.Winddirection;
//}

//private void Start()
//{
//    var temp1 = getGPS.Lang.ToString();
//    var temp2 = getGPS.Lon.ToString();
//    GetyWeatherString(temp1, temp2);

//    //GetyWeatherString(temp1, temp2);
//}

//public GetMyWeather(string lang, string lon)
//{
//    GetyWeatherString(lang, lon);
//}