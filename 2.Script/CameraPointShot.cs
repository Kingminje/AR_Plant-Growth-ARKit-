using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPointShot : MonoBehaviour
{
    public Camera taget;

    public RaycastHit Hit;

    public Vector3 RaycastObject()
    {
        return Hit.point;
    }
}