using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHoled : MonoBehaviour
{
    public Vector3 holdPosition;

    public bool HoldCheck = false;

    public void StartHold(Vector3 vector)
    {
        HoldCheck = true;
        holdPosition = vector;
        StartCoroutine(StartHoldCroutin());
    }

    private IEnumerator StartHoldCroutin()
    {
        yield return new WaitForSeconds(0.1f);

        if (HoldCheck == true)
            transform.position = holdPosition;
        else
        {
            yield break;
        }

        StartCoroutine(StartHoldCroutin());
    }
}