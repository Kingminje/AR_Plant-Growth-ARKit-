using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.FTail;
using FIMSpace.Jiggling;

internal enum GrowState
{ IDLE, WORKING, MIDDLE, END }

public class MainGrowing : MonoBehaviour
{
    public FJiggling_Grow fjGrow;

    public FTail_Animator fTailAni;

    public GameObject[] tailBones;  // 밴딩시킬 끝부분 본

    public Quaternion _MaxRot = Quaternion.identity; // 밴딩시킬 최대 각

    public int _rotateXY = 0; // x축일지 y축일지 정함

    public Vector3 vector3; // 저장된 현재 밴딩 로테이션 값

    public int _maxBendingSize = 0; // 최대 밴딩 사이즈를 정하기 위한 값

    public int time_Choise;/*(time_minutes, time_hours, time_Day);*/

    private float time_Value;
    private float tmpTime = 0.0166666666f;

    [SerializeField]
    private GrowState State = GrowState.IDLE;

    [SerializeField]
    private bool mainOrLeaf;

    private void Awake()
    {
        Jiggling();
    }

    private void Start()
    {
        if (transform.tag == "leaf")
        {
            SetStart();
        }
    }

    public void TimeSet(int timeNum)
    {
        switch (timeNum)
        {
            case 0:
                time_Value = 0.000011574f;
                break;

            case 1:
                time_Value = 0.000277777777f;
                break;

            case 2:
                time_Value = 0.0166666666f;
                break;

            default:
                time_Value = 0.16666666f;
                break;
        }

        //var tmp = FindObjectsOfType<MainGrowing>();
        //foreach (var item in tmp)
        //{
        //    item.time_Value = this.time_Value;
        //}
    }

    private void Jiggling()
    {
        fTailAni = GetComponent<FTail_Animator>();
        fjGrow = GetComponent<FJiggling_Grow>();

        fjGrow.GrowShrinkSpeed = 100f;
        fjGrow.StartShrinking();
    }

    public void SetStart()
    {
        MainOrLeaf();

        StartSetting();
    }

    private void MainOrLeaf()
    {
        if (transform.tag == "leaf")
        {
            mainOrLeaf = false;
            StartCoroutine(ScaleCheckLeaf());
        }
        else
        {
            mainOrLeaf = true;
            StartCoroutine(ScaleCheckMain());
        }
    }

    private void StartSetting() // 기본 세팅 및 최소 크기로 줄여둠.
    {
        if (mainOrLeaf == true)
        {
            tailBones = new GameObject[5];
            //vector3 = new Vector3(1, 0, 0);
            //_maxBendingSize = 10;
            TailBoneSet();
            TailMaxRotSet(vector3, _maxBendingSize);
        }

        State = GrowState.WORKING;
        //Debug.Log("ss");
    }

    private void TailBoneSet()
    {
        GameObject tail = transform.gameObject;

        for (; ; ) // 제일 끝 부분 찾음.
        {
            if (tail.tag == "Finish")
            {
                tailBones[0] = tail;
                break;
            }
            else
            {
                tail = tail.transform.GetChild(0).gameObject;
            }
        }

        for (int i = 1; i < 5; i++) //순차적으로 배열 등록
        {
            GameObject go = tailBones[i - 1].transform.parent.gameObject;
            tailBones[i] = go;
        }
    }

    private void TailMaxRotSet(Vector3 currentRot, int max)
    {
        float rotxy = Mathf.Floor(max * 10f) * 0.1f;

        if (currentRot[0] == 0 && currentRot[1] == 0)
        {
            Debug.Log("벡터값 어디갔니");
            return;
        }
        else if (currentRot[0] != 0f)
        {
            _MaxRot.eulerAngles = new Vector3(max, 0f, 0f);
            fTailAni.GravityPower.y = rotxy * 0.25f; // 그라비티값
            TailCurrentRot(tailBones, currentRot); // 현재 로테이션 값이 0이 아니면 현재 값 만큼 밴딩
            _rotateXY = 0;
        }
        else if (currentRot[1] != 0f)
        {
            _MaxRot.eulerAngles = new Vector3(0f, max, 0f);
            fTailAni.GravityPower.x = rotxy * 0.25f;
            TailCurrentRot(tailBones, currentRot);
            _rotateXY = 1;
        }
    }

    private void TailCurrentRot(GameObject[] ga, Vector3 currentRot) //현재 값 만큼 밴딩
    {
        for (int i = 0; i < 5; i++)
        {
            ga[i].transform.localEulerAngles = currentRot;
        }
    }

    public IEnumerator Bending(GameObject ga) //정해진 값으로 밴딩
    {
        float timer = 0f;
        float speed = 0.01f;

        float xy = 0;

        while ((int)xy <= _maxBendingSize)
        {
            switch (_rotateXY)
            {
                case 0:

                    xy = Mathf.Round(ga.transform.localEulerAngles.x);
                    break;

                case 1:
                    xy = Mathf.Round(ga.transform.localEulerAngles.y);
                    break;
            }

            if ((int)xy == _maxBendingSize)
            {
                yield break;
            }

            timer = Mathf.Clamp(timer + Time.deltaTime * speed, 0.0f, 1.0f);
            speed += Time.deltaTime * 0.05f;

            Quaternion a = ga.transform.localRotation;

            ga.transform.localRotation = Quaternion.Lerp(a, _MaxRot, timer * 0.5f);

            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator ScaleCheckMain()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(1f);

            if (transform.localScale.x == fjGrow.checkSize && State == GrowState.WORKING)
            {
                GetComponentInParent<SkinnedMeshRenderer>().enabled = true;
                //fjGrow.GrowShrinkSpeed = time_Value; 테스트
                fjGrow.GrowShrinkSpeed = tmpTime;
                fjGrow.StartGrowing();
                State = GrowState.MIDDLE;
            }

            if (transform.localScale.x >= 0.5f && State == GrowState.MIDDLE)
            {
                for (int i = 0; i < 5; i++)
                {
                    StartCoroutine(Bending(tailBones[i]));
                }
                State = GrowState.END;
            }

            if (transform.localScale.x >= 1f && State == GrowState.END)
            {
                GetComponent<FTail_Animator>().enabled = true;

                yield break;
            }
        }
    }

    public IEnumerator ScaleCheckLeaf()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(1f);

            if (transform.localScale.x == fjGrow.checkSize && State == GrowState.WORKING)
            {
                GetComponentInParent<SkinnedMeshRenderer>().enabled = true;
                //fjGrow.GrowShrinkSpeed = time_Value; 테스트
                fjGrow.GrowShrinkSpeed = tmpTime;
                fjGrow.StartGrowing();
                State = GrowState.END;
            }

            if (transform.localScale.x >= 1f && State == GrowState.END)
            {
                GetComponent<FTail_Animator>().enabled = true;
                yield break;
            }
        }
    }
}

//fTailAni.GravityPower.y = rotxy * 0.25f;
//Debug.Log(Mathf.Round(_MaxRot.eulerAngles.x) + " - " + _MaxRot.eulerAngles.y + " - " + _MaxRot.eulerAngles.z);

//TailCurrentRot(tailBones, _MaxRot.eulerAngles);

/*
public void ScaleCheck() // Update에서 사이즈 체크
{
   if (transform.localScale.x == 0 && State == GrowState.WORKING)
   {
       GetComponentInParent<SkinnedMeshRenderer>().enabled = true;
       fjGrow.GrowShrinkSpeed = 0.05f;
       fjGrow.StartGrowing();
       State = GrowState.MIDDLE;
   }

   if (transform.localScale.x >= 0.5f && State == GrowState.MIDDLE && mainOrLeaf == true)
   {
       for (int i = 0; i < 5; i++)
       {
           StartCoroutine(Bending(tailBones[i]));
       }
       State = GrowState.END;
   }

   if (transform.localScale.x >= 1f && State == GrowState.END)
   {
       if (mainOrLeaf == true)
       {
           //TailCurrentRot(tailBones, _MaxRot.eulerAngles);
       }
       GetComponent<FTail_Animator>().enabled = true;
   }
}*/