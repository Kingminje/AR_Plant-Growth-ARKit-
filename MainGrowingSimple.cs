using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.Jiggling;

internal enum MGrowState
{ IDLE, WORKING, MIDDLE, END }

public class MainGrowingSimple : MonoBehaviour
{
    public GameObject[] branch;

    public FJiggling_Grow mainbranch;

    [SerializeField]
    private MGrowState State = MGrowState.IDLE;

    private void Awake()
    {
        mainbranch.GrowShrinkSpeed = 100f;
        mainbranch.StartShrinking();
        //mainbranch.GetComponentInParent<SkinnedMeshRenderer>().enabled = true;
    }

    private void Start()
    {
        State = MGrowState.WORKING;
    }

    private void FixedUpdate()
    {
        if (transform.localScale.x == mainbranch.checkSize && State == MGrowState.WORKING)
        {
            Debug.Log("1");
            GetComponentInParent<SkinnedMeshRenderer>().enabled = true;
            mainbranch.GrowShrinkSpeed = 0.05f;
            mainbranch.StartGrowing();
            State = MGrowState.MIDDLE;
        }
        if (transform.localScale.x >= 0.1f && State == MGrowState.MIDDLE)
        {
            Leafcheck();
        }
        if (transform.localScale.x == 1f && State == MGrowState.END)
        {
            Debug.Log("d");
            State = MGrowState.IDLE;
            return;
        }
    }

    public void Leafcheck()
    {
        if (transform.localScale.x >= 0.1f && transform.localScale.x < 0.3f)
        {
            branch[0].GetComponent<BranchGrowing>().enabled = true;
            var tmp = branch[0].transform.parent.GetComponent<PositionHoled>();
            tmp.StartHold(branch[0].transform.position);
        }
        else if (transform.localScale.x >= 0.3f && transform.localScale.x < 0.5f)
        {
            branch[1].GetComponent<BranchGrowing>().enabled = true;
            var tmp = branch[1].transform.parent.GetComponent<PositionHoled>();
            tmp.StartHold(branch[1].transform.position);
            State = MGrowState.END;
        }
        else
        {
            return;
        }
    }

    public IEnumerator ScaleCheckMain()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(1f);

            if (transform.localScale.x == mainbranch.checkSize && State == MGrowState.WORKING)
            {
                GetComponentInParent<SkinnedMeshRenderer>().enabled = true;
                mainbranch.GrowShrinkSpeed = 0.1f;
                mainbranch.StartGrowing();
                State = MGrowState.MIDDLE;
            }

            if (transform.localScale.x >= 0f && State == MGrowState.MIDDLE)
            {
                for (int i = 0; i < branch.Length; i++)
                {
                    yield return new WaitForSeconds(0.5f);
                    branch[i].GetComponent<BranchGrowing>().enabled = true;
                    //var tmp = gameObject.GetComponentInChildren<IkControl>();
                    //tmp.FlowerIKSet(transform.localScale.x); 봉-인
                }

                State = MGrowState.END;
            }

            if (transform.localScale.x >= 1f && State == MGrowState.END)
            {
                var tmps = GameObject.FindObjectsOfType<PositionHoled>();
                foreach (var item in tmps)
                {
                    item.HoldCheck = false;
                }
                Debug.Log("d");
                yield break;
            }
        }
    }
}