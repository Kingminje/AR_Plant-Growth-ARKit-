using UnityEngine;
using TMPro;

public class GetText : MonoBehaviour
{
    public TextMeshPro[] textMeshPros;

    public bool textsCheck = false;

    public void TextSet(string[] a, string time)
    {
        textMeshPros[0].text = a[0] + " / " + time; // 시티 시간
        textMeshPros[1].text = a[1] + "°"; // 평균 온도
        textMeshPros[2].text = a[2]; // 날씨
        //textMeshPros[3].text = a[3] + "m/s /";
        //textMeshPros[3].text += a[4];
    }
}

//public enum TextName
//{
//    cityTime, temperature, weather, humidityWind,
//}
//private void Start()

//textMeshPros[0] = cityTime;
//textMeshPros[1] = temperature;
//textMeshPros[2] = weather;
//textMeshPros[3] = humidityWind;
//StartCoroutine(TextCheck());

//public textmeshpro citytime = null;
//public TextMeshPro temperature = null;
//public TextMeshPro weather = null;
//public TextMeshPro humidityWind = null;

//public IEnumerator TextCheck()
//{
//    if (textsCheck == false)
//    {
//        var tmp = FindObjectsOfType<TextMeshPro>();

//        foreach (var item in tmp)
//        {
//            if (item.tag == "weatherText")
//            {
//                TextSet(tmp);
//                textsCheck = true;
//                Debug.Log("완료'");
//                return null;
//            }
//        }
//        textsCheck = false;
//        new WaitForSeconds(5f);
//        StartCoroutine(TextCheck());
//    }
//    else
//        return null;

//    return null;
//    //if (tmp != "weather")
//    //{
//    //    textsCheck = false;
//    //    new WaitForSeconds(1f);
//    //    StartCoroutine(TextCheck());
//    //    return null;
//    //}
//    //else
//    //{
//    //    textsCheck = true;
//    //    return null;
//    //}

//    //if (this.cityTime == null)
//    //{
//    //    textsCheck = false;
//    //    new WaitForSeconds(5f);

//    //    StartCoroutine(TextCheck());
//    //    return null;
//    //}
//    //else
//    //{
//    //    textsCheck = true;
//    //    return null;
//    //}
//}