using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerContoller : MonoBehaviour
{
    private void Start()
    {
        OnPlawerActive();
    }

    public void FixedUpdate()
    {
        //if (ud == true)
        //{
        //    if (obj.transform.localPosition == pos[1].localPosition)
        //    {
        //        return;
        //    }

        //    Vector3 center = (obj.transform.localPosition + pos[1].localPosition) * 0.5f;

        //    center -= new Vector3(0, 1, 0);

        //    Vector3 a1 = obj.transform.localPosition - center;
        //    Vector3 a2 = pos[1].localPosition - center;

        //    obj.transform.localPosition = Vector3.Slerp(a1, a2, Time.deltaTime);
        //    obj.transform.localPosition += center;
        //}
        //else
        //{
        //    if (obj.transform.localPosition == pos[0].localPosition)
        //    {
        //        return;
        //    }

        //    Vector3 center = (obj.transform.localPosition + pos[0].localPosition) * 0.5f;

        //    center -= new Vector3(0, 1, 0);

        //    Vector3 a1 = obj.transform.localPosition - center;
        //    Vector3 a2 = pos[0].localPosition - center;

        //    obj.transform.localPosition = Vector3.Slerp(a1, a2, Time.deltaTime);
        //    obj.transform.localPosition += center;
        //}
    }

    public void OnPlawerActive()
    {
        //tmp.gameObject.SetActive(true);
        //애니메이션 실행
        // 아래 웨잇
        if (transform.GetChild(0).gameObject.activeSelf == false)
        {
            transform.GetChild(0).gameObject.gameObject.SetActive(true);
        }
    }
}