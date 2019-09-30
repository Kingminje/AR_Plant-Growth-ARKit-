using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingObjects : MonoBehaviour
{
    public Transform leafTaget; // 줄기의 위치 특정하기 위해 넣어준 위치값

    public Transform baseTaget; // 본의 스킨메쉬 랜더러 접근하기 위한 위치값

    public Transform BaselostTaget;

    public Material baseMaterial; // 베이스가 되는 메트리얼

    public LeafSet[] leafSets;

    private DataBase dataBase;

    private PrefabManager prefabManager;

    public bool startLeaf = false;

    private void Awake()
    {
        dataBase = GameObject.FindObjectOfType<DataBase>();

        //prefabManager = dataBase.GetComponent<PrefabManager>();
    }

    private void Start()
    {
        //if (dataBase.saveCheck) // 세이브 데이터가 있을 경우
        //{
        //    LoadSettingScript();
        //}
        //else
        //{
        //    //dataBase.ResetDataBase();
        //    LoadSettingScript();
        //}

        //
        TreeInput(dataBase._branchTree);
        //TreeInput("20212012");

        leafSets = gameObject.GetComponentsInChildren<LeafSet>();
        LeafValueSetting(leafSets);

        //Array.Reverse(leafMakes);
        //StartCoroutine(LateMakeInput());
    }

    private void LeafValueSetting(LeafSet[] leafs)
    {
        float bonData = dataBase._bonsLoaleScale;
        //float bonData = 0.5f;

        float value = 1f;
        float value2 = leafs.Length;
        float value3 = value / value2;
        Array.Reverse(leafs);
        //Array.Reverse()
        //leafSets

        Debug.Log(value2);

        for (int i = 0; i < (int)value2; i++)
        {
            float tmp = value3 * (float)i;

            leafSets[i].satisfyBonScale_0 = tmp;
            leafSets[i].satisfyBonScale_1 = tmp + value3;
            leafSets[i].ChildLeafGet();
            leafSets[i].ChildLeafSet(bonData);
            Debug.Log(tmp);
            Debug.Log(tmp + value3);
        }
        //startLeaf = true;
        Array.Reverse(leafs);
        StartCoroutine(LeafOneLateCorotin());
        StartCoroutine(AutoSave());
    }

    private IEnumerator LeafOneLateCorotin()
    {
        yield return new WaitForSeconds(0.1f);

        float bonX = leafTaget.localScale.x;

        var count = 0;

        //Debug.Log(bonX);

        for (int i = 0; i < leafSets.Length; i++)
        {
            var tmpLeafSet = leafSets[i];
            if (tmpLeafSet.ChildLeafSet(bonX))
            {
                ++count;
                // 트루일때 작업
            }
        }
        if (count != 0)
        {
            Array.Resize<LeafSet>(ref leafSets, leafSets.Length - count);

            // Debug.Log(leafSets.Length - count);

            if ((leafSets.Length - count) == -1)
            {
                Debug.Log("리프 코루틴 끝");

                yield return null;
            }
        }

        StartCoroutine(LeafOneLateCorotin());
    }

    private void SaveData()
    {
        var savefunction = dataBase;
        savefunction.SaveProcessor(leafTaget.localScale.x, BaselostTaget.localEulerAngles.x, 0f); // 본스케일, 로테이션값, 안쓰는값 0f
    }

    private IEnumerator AutoSave()
    {
        yield return new WaitForSeconds(5f);

        if (dataBase.saveCheck) // 세이브 할 준비가 되었는지 확인
        {
            SaveData();
            StartCoroutine("AutoSave");
        }
        else
            Debug.LogError("세이브 값 저장 준비가 되지 않았습니다.");
    }

    //private void MakeInput()
    //{
    //    //var tmpScale = dataBase._bonsLoaleScale;
    //    var tmpScale = 0.5f;
    //    int _count = 0;
    //    //var tmpScale2 = 0.7f; // 테스트
    //    //        LeafMake[] tmpScipit = new LeafMake[20];
    //    Debug.Log(leafMakes.Length);
    //    for (int i = 0; i < leafMakes.Length; i++)
    //    {
    //        var tmpData = leafMakes[i];
    //        if (tmpData.Checkbonreafon(tmpScale))
    //        {
    //            tmpData.num = i;
    //            tmpData.transform.GetComponent<MainGrowing>().enabled = true;
    //            var inRenderer = tmpData.transform.parent.GetComponent<SkinnedMeshRenderer>();
    //            MateralInput(inRenderer);
    //            ++_count;
    //            //leafMakes[i] = leafMakes[]; 넣는곳
    //        }
    //    }

    //    //if (_count != 0)
    //    //{
    //    //    Debug.Log(_count);

    //    //    var tmpleafMAkes = leafMakes;
    //    //    leafMakes = new LeafMake[_count];
    //    //    for (int i = 0; i < _count; i++)
    //    //    {
    //    //        leafMakes[i] = tmpleafMAkes[i];
    //    //        Debug.Log(leafMakes);
    //    //    }
    //    //}

    //    //foreach (var item in leafMakes)
    //    //{
    //    //    if (item.Checkbonreafon(tmpScale2))
    //    //    {
    //    //        item.transform.GetComponent<MainGrowing>().enabled = true;
    //    //    }
    //    //    else
    //    //    {
    //    //        tmpSCipit[] =
    //    //    }
    //    //    //else
    //    //    //{
    //    //    //    //leafMakes[count] = item;
    //    //    //    //item.transform.GetComponent<MainGrowing>().enabled = true;
    //    //    //}

    //    //    // 테스트 코드
    //    //}
    //}

    //private void LeafScriptSet(LeafMake currentScript, int i)
    //{
    //    leafMakes[i] = currentScript;
    //}

    //private IEnumerator LateMakeInput()
    //{
    //    new WaitForSeconds(1f);

    //    var tmpScale = leafTaget.localScale.x;
    //    var count = 1;

    //    LeafMake[] tmpScipit = leafMakes;

    //    for (int i = 0; i < leafMakes.Length; i++)
    //    {
    //        var tmpData = leafMakes[i];
    //        if (tmpData == null)
    //        {
    //            continue;
    //        }

    //        if (tmpData.Checkbonreafon(tmpScale))
    //        {
    //            tmpData.transform.GetComponent<MainGrowing>().enabled = true;
    //            var inRenderer = tmpData.transform.parent.GetComponent<SkinnedMeshRenderer>();
    //            MateralInput(inRenderer);
    //        }
    //        else
    //        {
    //            tmpScipit[count] = tmpData;
    //            ++count;
    //        }
    //    }
    //    leafMakes = tmpScipit;

    //    StartCoroutine(LateMakeInput());

    //    return null;
    //}

    /// <summary>
    /// MakeInput에서 받은 스킨메쉬 랜더러의 값의 메트리얼을 변경
    /// </summary>
    /// <param name="skinnedMeshRenderer">메트리얼의 값을 바꿀 스킨메쉬 랜더러</param>
    /// <param name="g_value">초기값 204f 여기서 낮아질수록 죽는거임</param>
    private void MateralInput(SkinnedMeshRenderer skinnedMeshRenderer, float g_value = 204f)
    {
        var _material = new Material(baseMaterial);

        Debug.Log(_material.color);
        Color tmpC = _material.color;
        _material.color = new Color(tmpC.r, g_value / 255f, tmpC.b, tmpC.a);

        Debug.Log(_material.color);

        skinnedMeshRenderer.material = _material;
    }

    private void LoadSettingScript()
    {
        var baseGrowing = GameObject.Find("Bon[0]").transform.GetComponent<MainGrowing>(); //임시로 잡아 놓음.

        // 로컬 값 가져와서 씀
        baseGrowing.fjGrow.checkSize = dataBase._bonsLoaleScale;
        baseGrowing.TimeSet(2);
        // 해당 스크립트가 작동 하기 전 변경 해줘야 하는 값

        float RotateData = dataBase._bonCurrentRotateValue;
        int MaxRotateData = dataBase._bonMaxRotateValue;

        // 데이터 베이스 로드가 되었든 말든 무조건 돌아감
        if (dataBase._xyVectoerCheck == 0)
        {
            //임의 생성
            baseGrowing.vector3 = new Vector3(RotateData + 0.00001f, 0f, 0f);
            baseGrowing._maxBendingSize = MaxRotateData;
        }
        else
        {
            //임의 생성
            baseGrowing.vector3 = new Vector3(0f, RotateData + 0.00001f, 0f);
            baseGrowing._maxBendingSize = MaxRotateData;
        }

        baseGrowing.SetStart();
    }

    public void TreeInput(string treenString)
    {
        Transform tmpTr = leafTaget;
        Debug.Log(treenString);

        for (int i = 0; i < 9; i++)
        {
            if (tmpTr.childCount == 2)
            {
                var tmpCher = int.Parse(treenString[i - 1].ToString());

                //Debug.Log(i + tmpCher);

                if (tmpCher == 1)
                {
                    tmpTr.GetChild(1).gameObject.SetActive(true);
                    tmpTr.GetChild(1).GetChild(0).gameObject.SetActive(true);
                }
                else if (tmpCher == 2)
                {
                    tmpTr.GetChild(1).gameObject.SetActive(true);
                    tmpTr.GetChild(1).GetChild(0).gameObject.SetActive(true);
                    tmpTr.GetChild(1).GetChild(1).gameObject.SetActive(true);
                }
            }

            if (tmpTr.childCount == 0)
                return;

            tmpTr = tmpTr.GetChild(0).transform;
        }
    }
}