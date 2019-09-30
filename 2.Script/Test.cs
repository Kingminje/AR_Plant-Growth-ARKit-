using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FIMSpace.FTail;
using FIMSpace.Jiggling;

public class Test : MonoBehaviour
{
    public FJiggling_Grow fjGrow;

    public FTail_Animator fTailAni;

    public GameObject[] bones;

    public Quaternion rotation = Quaternion.identity;

    public bool bendingstart = false;

    public int _rotateXY = 0;

    public Vector3 vector3;
    public float _maxBendingSize = 0f;

    private void Start()
    {
        StartSetting();

        StartCoroutine(StartGrow());
    }

    private void StartSetting()
    {
        fTailAni = GetComponent<FTail_Animator>();
        fjGrow = GetComponent<FJiggling_Grow>();
        fjGrow.GrowShrinkSpeed = 100f;
        fjGrow.StartShrinking();

        bones = new GameObject[5];

        vector3 = new Vector3(5f, 0f, 0f);

        _maxBendingSize = 10f;

        TailBoneSetting();
        SettingRot();
    }

    public void Update()
    {
        //float x = Mathf.Floor(transform.eulerAngles.x * 100f) * 0.01f;

        if (transform.localScale.y >= 0.5f && bendingstart == true)
        {
            bendingstart = false;
            for (int i = 0; i < 5; i++)
            {
                StartCoroutine(Bending(bones[i]));
            }
        }
    }

    public IEnumerator StartGrow() // 성장 시작
    {
        yield return new WaitForSeconds(1f);

        GetComponentInParent<SkinnedMeshRenderer>().enabled = true;

        fjGrow.GrowShrinkSpeed = 0.5f;

        fjGrow.StartGrowing();

        StartCoroutine(TailAniON());
        bendingstart = true;
        yield break;
    }

    public IEnumerator TailAniON() // 테일 애니메이션 on
    {
        while (transform.localScale.y <= 1f)
        {
            if (transform.localScale.y >= 1f)
            {
                //transform.parent.gameObject.AddComponent<Equipmentizer>();
                GetComponent<FTail_Animator>().enabled = true;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void TailBoneSetting()// 밴딩 시킬 본들 세팅, 꼬리 끝 부터 시작
    {
        GameObject tail = transform.gameObject;

        for (int i = 0; i < 11; i++)
        {
            if (tail.tag == "Finish")
            {
                bones[0] = tail;
                break;
            }
            else
            {
                tail = tail.transform.GetChild(0).gameObject;
            }
        }

        for (int i = 1; i < 5; i++)
        {
            GameObject go = bones[i - 1].transform.parent.gameObject;
            bones[i] = go;
        }

        TailRotSetting(bones, vector3);
    }

    public void TailRotSetting(GameObject[] ga, Vector3 rotV) // 시작 밴딩 로테이션 세팅.
    {
        for (int i = 0; i < 5; i++)
        {
            ga[i].transform.localEulerAngles = rotV;
        }
    }

    public IEnumerator Bending(GameObject ga) //가지가 휘는 작업
    {
        float timer = 0f;
        float speed = 0.1f;

        for (; ; )
        {
            switch (_rotateXY)
            {
                case 0:
                    if (ga.transform.eulerAngles.x == _maxBendingSize)
                    {
                        Debug.LogError("dddd");
                        yield break;
                    }
                    break;

                case 1:
                    if (ga.transform.eulerAngles.y == _maxBendingSize)
                    {
                        Debug.LogError("dddd");
                        yield break;
                    }
                    break;
            }

            timer = Mathf.Clamp(timer + Time.deltaTime * speed, 0.0f, 1.0f);
            speed += Time.deltaTime * 0.05f;

            Quaternion a = ga.transform.localRotation;

            ga.transform.localRotation = Quaternion.Lerp(a, rotation, timer * 0.5f);
            yield return new WaitForFixedUpdate();
        }
    }

    public void SettingRot()// 휘는 방향 세팅.
    {
        float rotxy = Mathf.Floor(_maxBendingSize * 10f) * 0.1f;

        if (vector3[0] == 0 && vector3[1] == 0)
        {
            Debug.Log("벡터값 어디갔니");
        }
        else if (vector3[0] != 0f)
        {
            rotation.eulerAngles = new Vector3(rotxy, 0f, 0f);

            //fTailAni.GravityPower.y = rotxy * 0.25f;

            _rotateXY = 0;
        }
        else if (vector3[1] != 0f)
        {
            rotation.eulerAngles = new Vector3(0f, rotxy, 0f);

            //fTailAni.GravityPower.x = rotxy * -0.25f;

            _rotateXY = 1;
        }
    }

    /*
    public void SetBoneTr() //미사용.
    {
        boneRt = new Transform[10];
        boneRt[0] = GetComponent<Transform>();

        for (int i = 0; i < 9; i++)
        {
            int a = i + 1;

            GameObject cgo = boneRt[i].transform.Find("BoneF-LOD[" + a + "]").gameObject;

            boneRt[a] = cgo.transform;
        }
    }*/

    /*    public void DBCheck()
    {
        DataBase dataBase = FindObjectOfType<DataBase>();

        float scale = dataBase._bonsLoaleScale;
        fjGrow.checkSize = scale;

        _maxBendingSize = dataBase._bonMaxRotateValue;
        _currentRot = dataBase._bonCurrentRotateValue;
        // %
        _rotateXY = Mathf.Abs(dataBase._bonMaxRotateValue);

        Debug.Log(scale + " - " + _currentRot + " - " + _maxBendingSize);
    }*/
}

//public void ChangeRt()
//{
//    float a = Mathf.Floor(transform.localScale.z * 10f) * 0.1f;

//    Quaternion a = boneRt[1].transform.localRotation;
//    boneRt[1].transform.localRotation = Quaternion.Lerp(a, rotation, Time.deltaTime);

//    for (int i = 1; i < 10; i++)
//    {
//        Quaternion a = boneRt[i].transform.rotation;

//        Quaternion rotation = Quaternion.identity;
//        rotation.eulerAngles = new Vector3(2f, 0f, 0f);

//        boneRt[i].transform.rotation = Quaternion.Lerp(a, a * rotation, Time.deltaTime);

//        //boneRt[i].transform.rotation *= rotation;
//    }

//    bones = new GameObject[5];

//    bones[0] = GameObject.FindGameObjectWithTag("Finish");
//    for (int i = 1; i < 5; i++)
//    {
//        GameObject go = bones[i - 1].transform.parent.gameObject;
//        bones[i] = go;

//        Quaternion a = bones[i - 1].transform.localRotation;
//        bones[i - 1].transform.localRotation = Quaternion.Lerp(a, rotation, Time.deltaTime);
//    }
//}