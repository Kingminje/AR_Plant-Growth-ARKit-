using UnityEngine;
using TMPro;

public class GetGPS : MonoBehaviour
{
    private bool gpsInit = false;

    private LocationInfo currentGPSPosition;

    private int gps_connect = 0;

    private double detailed_num = 1.0;//기존 좌표는 float형으로 소수점 자리가 비교적 자세히 출력되는 double을 곱하여 자세한 값을 구합니다.

    private double lang;

    private double lon;

    public double Lang
    {
        get { return lang; }
        set { lang = value; }
    }

    public double Lon
    {
        get { return lon; }
        set { lon = value; }
    }

    public TextMeshPro text_latitude;

    //public Text text_longitude;

    //public Text text_refresh;

    //private void Start()
    //{
    //    StartGPS();
    //}

    public void StartGPS()
    {
        Input.location.Start(0.5f);

        int wait = 1000; // 기본 값

        // Checks if the GPS is enabled by the user (-> Allow location )

        if (/*Testmod() */Input.location.isEnabledByUser)//사용자에 의하여 좌표값을 실행 할 수 있을 경우
        {
            while (Input.location.status == LocationServiceStatus.Initializing && wait > 0)//초기화 진행중이면
            {
                wait--; // 기다리는 시간을 뺀다
            }

            //GPS를 잡는 대기시간

            if (Input.location.status != LocationServiceStatus.Failed)//GPS가 실행중이라면

            {
                gpsInit = true;

                // We start the timer to check each tick (every 3 sec) the current gps position

                InvokeRepeating("RetrieveGPSData", 0.0001f, 1.0f);//0.0001초에 실행하고 1초마다 해당 함수를 실행합니다.
            }
        }

        else//GPS가 없는 경우 (GPS가 없는 기기거나 안드로이드 GPS를 설정 하지 않았을 경우

        {
            text_latitude.text = "GPS not available";
            Input.location.Stop();

            //text_longitude.text = "GPS not available";

            Debug.Log("GPS not available");
        }
    }

    private void RetrieveGPSData()
    {
        currentGPSPosition = Input.location.lastData;//gps를 데이터를 받습니다.

        Lang = currentGPSPosition.latitude * detailed_num;
        Lon = currentGPSPosition.longitude * detailed_num;

        //text_latitude.text = "위도 " + (currentGPSPosition.latitude * detailed_num).ToString();//위도 값을 받아,텍스트에 출력합니다
        //text_longitude.text = "경도 " + (currentGPSPosition.longitude * detailed_num).ToString();//경도 값을 받아, 텍스트에 출력합니다.

        gps_connect++;

        text_latitude.text = "갱신 횟수 : " + gps_connect.ToString();

        int num = (int)Input.location.status;
        Debug.Log(num);

        if (Lang != 0)
        {
            CancelInvoke("RetrieveGPSData");
            Input.location.Stop();
            Lang = (currentGPSPosition.latitude * detailed_num);
            Lon = (currentGPSPosition.longitude * detailed_num);
            text_latitude.text = "gps 설정 완료되었습니다.";
            //var tmp = gameObject;
            //var tmp2 = tmp.AddComponent<GetMyWeather>();
            //var tmp2 = new GetMyWeather(Lang.ToString(), Lon.ToString());
        }
    }

    //private void Start()
    //{
    //    var tmp = gameObject;
    //}
}