using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.FTail;
using FIMSpace.Jiggling;

public class TestingGrow : MonoBehaviour
{
    public FJiggling_Grow[] bones;
    public int cnt = 1;
    public FJiggling_Grow now;

    public bool check = false;

    private void Start()
    {
        bones = GetComponentsInChildren<FJiggling_Grow>();

        now = bones[0];

        foreach (FJiggling_Grow a in bones)
        {
            a.GrowShrinkSpeed = 100f;
            a.checkSize = 0.5f;
            a.StartShrinking();
        }

        check = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (check == true)
        {
            StartCoroutine(UP());
            check = false;
        }
    }

    public void Pingpong()
    {
        foreach (FJiggling_Grow a in bones)
        {
            a.GrowShrinkSpeed = 0.05f;
        }

        if (bones[0].GetComponent<Transform>().transform.localScale.x <= 0.01f)
        {
            foreach (FJiggling_Grow a in bones)
            {
                a.StartGrowing();
            }
        }
        if (bones[0].GetComponent<Transform>().transform.localScale.x >= 1f)
        {
            foreach (FJiggling_Grow a in bones)
            {
                a.StartShrinking();
            }
        }
    }

    public IEnumerator UP()
    {
        yield return new WaitForSeconds(1f);
        int branchcnt = 12;

        foreach (FJiggling_Grow a in bones)
        {
            a.GrowShrinkSpeed = 0.3f;
        }

        now.StartGrowing();

        while (cnt < 13)
        {
            if (now.GetComponent<Transform>().transform.localScale.x >= 1f)
            {
                yield return new WaitForFixedUpdate();

                switch (now.gameObject.tag)
                {
                    case "branch":

                        bones[branchcnt].StartGrowing();
                        branchcnt--;
                        break;

                    case "end":

                        now.GetComponentInChildren<MegaCacheOBJ>().animate = true;
                        break;
                }

                if (cnt == 12)
                {
                    Debug.Log(cnt);
                    yield break;
                }

                now = bones[cnt];
                now.StartGrowing();
                cnt++;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}