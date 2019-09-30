using System.Collections;
using UnityEngine;
using TMPro;

public class GPSTest : MonoBehaviour
{
    private LocationInfo currentGPSPosition;
    private LocationService locationService;
    public TextMeshPro text;
    public double dedetailed_num;

    private void Awake()
    {
        text = FindObjectOfType<TextMeshPro>();
    }

    private IEnumerator Start()
    {
        if (!Input.location.isEnabledByUser)
            yield break;

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // 연결이 실패한
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("GPS not available");
            yield break;
        }
        else
        {
            // 액세스 권한 부여 및 위치 값을 검색 할 수 있음
        }

        currentGPSPosition = Input.location.lastData;

        text.text = (currentGPSPosition.latitude * dedetailed_num).ToString() + "/" +
            (currentGPSPosition.longitude * dedetailed_num).ToString();

        // 위치 업데이트를 지속적으로 쿼리 할 필요가없는 경우 서비스 중지
        Input.location.Stop();
    }
}