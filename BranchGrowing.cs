using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.Jiggling;

internal enum BranchGrowState
{ IDLE, WORKING, MIDDLE, END }

public class BranchGrowing : MonoBehaviour
{
    public FJiggling_Grow[] leaf;
    public FJiggling_Grow branch;

    [SerializeField]
    private BranchGrowState State = BranchGrowState.IDLE;

    private void Awake()
    {
        branch.GrowShrinkSpeed = 100f;
        branch.StartShrinking();
        branch.GetComponentInParent<SkinnedMeshRenderer>().enabled = true;
        for (int i = 0; i < leaf.Length; i++)
        {
            leaf[i].GrowShrinkSpeed = 100f;
            leaf[i].StartShrinking();
            leaf[i].GetComponentInChildren<MeshRenderer>().enabled = true;
        }
    }

    private void Start()
    {
        State = BranchGrowState.WORKING;
    }

    private void FixedUpdate()
    {
        if (transform.localScale.x == branch.checkSize && State == BranchGrowState.WORKING)
        {
            GetComponentInParent<SkinnedMeshRenderer>().enabled = true;
            branch.GrowShrinkSpeed = 0.1f;
            branch.StartGrowing();
            State = BranchGrowState.MIDDLE;
        }
        else if (transform.localScale.x >= 0.2f && State == BranchGrowState.MIDDLE)
        {
            Leafcheck();
        }
        else if (transform.localScale.x == 1f && State == BranchGrowState.END)
        {
            Debug.Log("d");
            State = BranchGrowState.IDLE;
            return;
        }
        else
        {
            return;
        }
    }

    public void Leafcheck()
    {
        if (transform.localScale.x >= 0.2f && transform.localScale.x < 0.4f)
        {
            leaf[0].GrowShrinkSpeed = 0.25f;
            leaf[0].StartGrowing();
        }
        else if (transform.localScale.x >= 0.4f && transform.localScale.x < 0.6f)
        {
            leaf[1].GrowShrinkSpeed = 0.25f;
            leaf[1].StartGrowing();
        }
        else if (transform.localScale.x >= 0.6f)
        {
            leaf[2].GrowShrinkSpeed = 0.25f;
            leaf[2].StartGrowing();

            State = BranchGrowState.END;
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

            if (transform.localScale.x == branch.checkSize && State == BranchGrowState.WORKING)
            {
                GetComponentInParent<SkinnedMeshRenderer>().enabled = true;
                branch.GrowShrinkSpeed = 0.1f;
                branch.StartGrowing();
                State = BranchGrowState.MIDDLE;
            }

            if (transform.localScale.x >= 0.3f && State == BranchGrowState.MIDDLE)
            {
                for (int i = 0; i < leaf.Length; i++)
                {
                    yield return new WaitForSeconds(1f);
                    leaf[i].GrowShrinkSpeed = 0.25f;
                    leaf[i].StartGrowing();
                }

                State = BranchGrowState.END;
            }

            if (transform.localScale.x >= 1f && State == BranchGrowState.END)
            {
                Debug.Log("d");
                yield break;
            }
        }
    }
}