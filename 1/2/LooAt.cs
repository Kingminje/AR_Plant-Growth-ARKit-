using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LooAt : MonoBehaviour
{
    public Transform Taget;

    private void Awake()
    {
        Taget = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(Taget);
    }
}