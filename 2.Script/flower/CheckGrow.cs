using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.FTail;
using FIMSpace.Jiggling;

public class CheckGrow : MonoBehaviour
{
    public FJiggling_Grow fjGrow;

    public float a;

    public float b;

    public void Awake()
    {
    }

    public void Start()
    {
        StartCoroutine(StartSet());
    }

    public IEnumerator StartSet()
    {
        fjGrow.checkSize = a;

        yield return new WaitForSeconds(0.01f);
        fjGrow.GrowShrinkSpeed = 1000f;
        fjGrow.StartShrinking();
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.001f);
        fjGrow.GrowShrinkSpeed = 0.05f;
        GetComponent<MeshRenderer>().enabled = true;

        StartCoroutine(CheckScale());
    }

    public IEnumerator CheckScale()
    {
        yield return new WaitForSeconds(0.1f);
        fjGrow.StartGrowing();

        while (transform.localScale.z <= 1f)
        {
            if (transform.localScale.z == 1f)
            {
                b = Mathf.Floor(transform.localScale.z * 10f) * 0.1f;
                Debug.Log(b);
                yield break;
            }
            b = Mathf.Floor(transform.localScale.z * 10f) * 0.1f;
            Debug.Log(b);
            yield return new WaitForSeconds(0.1f);
        }
    }
}