using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafMake : MonoBehaviour
{
    //public Transform[] leafTr;
    //public GameObject leaf;

    //public bool[] check;

    //[Tooltip("성장중")]
    //public float satisfyBonScale_0 = 0f;

    //[Tooltip("성장후")]
    //public float satisfyBonScale_1 = 0f;

    public float changeSize = 0f;

    //public float _satisfyBonScale
    //{
    //    get { return satisfyBonScale; }
    //    set { satisfyBonScale = value; }
    //}

    public bool Checkbonreafon(float bonscale, float bonCheck0, float bonCheck1)
    {
        if (bonscale > bonCheck0)
        {
            float tmpfloat = 0f;

            if (bonscale > bonCheck1)
            {
                Debug.Log("Stop 02");

                SetGameObjectOn();

                changeSize = 1f;

                return true;
            }

            Debug.Log("Stop 01");

            float tmpPer = bonscale / bonCheck1 * 100f;

            /*tmpfloat = 1f * (1f + tmpPer / 100f);*//*(bonscale * tmpPer * 0.01f);*/
            tmpfloat = (tmpPer * 0.01f / bonCheck1) / 100f;

            //tmpfloat = tmpPer * 0.01f;
            Debug.Log(tmpPer + "%");
            Debug.Log(tmpfloat);

            changeSize = tmpfloat;

            SetGameObjectOn();

            //transform.localScale = new Vector3(changeSize, changeSize, changeSize);

            //Debug.Log(tmpfloat);

            return true;
        }

        Debug.Log("실패");
        //SetGameObjectOff();
        return false;
    }

    private void SetGameObjectOn()
    {
        this.gameObject.SetActive(true);
        //this.transform.parent.GetComponent<SkinnedMeshRenderer>().enabled = true;
    }

    private void SetGameObjectOff()
    {
        this.gameObject.SetActive(false);
        //this.transform.parent.parent.gameObject.SetActive(false);
        //this.transform.parent.GetComponent<SkinnedMeshRenderer>().enabled = false;
    }
}

//private void Start()
//{
//    tr = gameObject.GetComponent<Transform>();

//    check = new bool[3];
//    for (int i = 0; i < 3; i++)
//    {
//        check[i] = false;
//    }

//    StartCoroutine(LeafInst());
//}

//public IEnumerator LeafInst()
//{
//    yield return new WaitForSeconds(1f);

//    if (check[2] == true && check[1] == true && check[0] == true)
//    {
//        yield break;
//    }

//    float a;

//    for (; tr.localScale.z <= 1f;)
//    {
//        a = Mathf.Floor(tr.localScale.z * 10f) * 0.1f;

//        if (a >= 0.5f)
//        {
//            InstLeaf(check[0], 0);
//            check[0] = true;
//        }

//        if (a >= 0.7f)
//        {
//            InstLeaf(check[1], 1);
//            check[1] = true;
//        }

//        if (a >= 0.9f)
//        {
//            InstLeaf(check[2], 2);
//            check[2] = true;
//        }

//        yield return new WaitForSeconds(0.01f);
//    }
//}

//public void InstLeaf(bool a, int c)
//{
//    if (a == false)
//    {
//        GameObject leafGO = leafTr[c].GetChild(1).gameObject;

//        leafGO.SetActive(true);

//        //Instantiate(leaf, leafTr[c]);
//    }
//    else
//    {
//        return;
//    }
//}

//public int[] cnt;
//cnt = new int[3];
//for (int i = 0; i < 3; i++)
//{
//    cnt[i] = 0;
//}

//int b = Mathf.FloorToInt(transform.localScale.y * 10f);
// float a = Mathf.Floor(transform.localScale.y * 10f) * 0.1f;
//0.5,  0.7,  0.9

//    while (tr.localScale.z <= 1f)
//        {
//            a = Mathf.Floor(tr.localScale.z* 10f) * 0.1f;

//            Debug.Log(a);

//            if (a == 0.5f)
//            {
//                Debug.Log(a);
//                InstLeaf(check[0], 0);
//    check[0] = true;
//            }

//            if (a == 0.7f)
//            {
//                Debug.Log(a);
//                InstLeaf(check[1], 1);
//check[1] = true;
//            }

//            if (a == 0.9f)
//            {
//                Debug.Log(a);
//                InstLeaf(check[2], 2);
//check[2] = true;
//            }

//            yield return new WaitForSeconds(0.01f);
//        }