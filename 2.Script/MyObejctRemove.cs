using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObejctRemove : MonoBehaviour
{
    public void MyRemove()
    {
        var tmp = GameObject.FindObjectOfType<FlowerContoller>();
        tmp.OnPlawerActive();
        Destroy(gameObject);
    }
}