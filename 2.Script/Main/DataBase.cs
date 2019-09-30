using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataBase : MonoBehaviour
{
    [SerializeField]
    private float bonsLocaleScale = 0f;

    [SerializeField]
    private float branchLocaleRo = 0f;

    [SerializeField]
    private float branchRGB = 0;

    private int bonMaxRotation;

    private float bonCurrentRotation;

    private int xyVactor = 0;

    [Tooltip("첫번째 자리부터 본의 줄기 넘버 0번째 줄기의 값이 1이면 x값 양쪽 생성 2면 x,y값 전부 켜짐")]
    [SerializeField]
    private string branchTree = "00000000";

    public bool TestMod = false;

    public float _bonsLoaleScale
    {
        get { return bonsLocaleScale; }
        set { bonsLocaleScale = value; }
    }

    public float _branchLocaleScale
    {
        get { return branchLocaleRo; }
        set { branchLocaleRo = value; }
    }

    public float _BranchRGB
    {
        get { return branchRGB; }
        set { branchRGB = value; }
    }

    public string _branchTree
    {
        get { return branchTree; }
        set { branchTree = value; }
    }

    public int _bonMaxRotateValue
    {
        get { return bonMaxRotation; }
        set { bonMaxRotation = value; }
    }

    public float _bonCurrentRotateValue
    {
        get { return bonCurrentRotation; }
        set { bonCurrentRotation = value; }
    }

    public int _xyVectoerCheck
    {
        get { return xyVactor; }
    }

    public bool saveCheck = false; // 세이브 데이터가 있을 시 true 반환

    private bool SaveCheck()
    {
        if (PlayerPrefs.HasKey("BonScale"))
        {
            saveCheck = true;
            return true;
        }
        return false;
    }

    private void Awake()
    {
        this.gameObject.AddComponent<DonRemove>();

        if (TestMod)
        {
            PlayerPrefs.DeleteAll();
        }

        if (SaveCheck())
        {
            LoadProcessor();
        }
        else
        {
            ResetDataBase();
            SaveDataBase(0f, 0f, 5f, 204f);
        }
        SceneManager.LoadScene(1);

        //LoadProcessor();
    }

    public void ResetDataBase()
    {
        bonCurrentRotation = 0f;
        bonsLocaleScale = 0f;
        branchLocaleRo = 0f;
        branchRGB = 204f;

        string formatTree = "";

        for (int i = 0; i < 10; i++)
            if (i != 0 && i != 9)
                formatTree += Random.Range(0, 3);/*(0, 3).ToString();*/

        branchTree = formatTree;

        bonMaxRotation = Random.Range(-10, 11);

        var tmpVectorCheck = bonMaxRotation;

        if (Mathf.Abs(tmpVectorCheck % 2) == 0)
            xyVactor = 0;
        else
            xyVactor = 1;

        //쓰레기 값 들어가는거 방지
        PlayerPrefs.DeleteAll();

        // 한번만 돌기에 여기에 넣는다
        PlayerPrefs.SetInt("BonRotateVectoer", xyVactor);
        PlayerPrefs.SetString("BranchTree", branchTree);
        PlayerPrefs.SetInt("BonMaxRotate", bonMaxRotation);
    }

    public void LoadProcessor()
    {
        bonsLocaleScale = PlayerPrefs.GetFloat("BonScale");
        branchLocaleRo = PlayerPrefs.GetFloat("BonRotate");
        branchRGB = PlayerPrefs.GetFloat("BonRGBAScale_G");
        branchTree = PlayerPrefs.GetString("BranchTree");
        bonCurrentRotation = PlayerPrefs.GetFloat("BonCurrentRotate"); // 꽃의 현재 회전값 부름
        Debug.LogFormat("Load" + bonsLocaleScale + branchLocaleRo + branchRGB + "트리" + branchTree + bonCurrentRotation);
    }

    public void SaveProcessor(float bonscale, float branchRo, float bonRotate, float bonRGB = 204f)
    {
        bonsLocaleScale = bonscale;
        branchLocaleRo = branchRo;
        branchRGB = bonRGB;
        bonCurrentRotation = bonRotate;

        SaveDataBase(bonscale, branchRo, bonRotate, bonRGB);
    }

    /// <summary>
    /// 데이터 세이브
    /// </summary>
    /// <param name="bonscale">메인 본의 스케일 값 입력</param>
    /// <param name="branchRo">메인 본의 로테이션 값 입력</param>
    /// <param name="bonRGB">디폴트값 204f임 메트리얼값 RGB </param>
    /// <returns></returns>
    private bool SaveDataBase(float bonscale, float branchRo, float bonRotate, float bonRGB = 204f)
    {
        bonsLocaleScale = bonscale;
        branchLocaleRo = branchRo;
        branchRGB = bonRGB;
        bonCurrentRotation = bonRotate;

        PlayerPrefs.SetFloat("BonScale", bonsLocaleScale); // 꽃의 대 스케일
        PlayerPrefs.SetFloat("BonRotate", branchLocaleRo);// 꽃의 대 의 회전값 저장
        PlayerPrefs.SetFloat("BonRGBAScale_G", branchRGB); // 꽃의 대에  메트리얼 RPGA에 G값 저장
        PlayerPrefs.SetFloat("BonCurrentRotate", bonRotate); // 꽃의 현재 회전값 저장

        PlayerPrefs.Save();

        Debug.LogFormat("Save " + bonsLocaleScale + "/" + branchLocaleRo + " / " + branchRGB + " / " + bonCurrentRotation);

        //PlayerPrefs.SetString
        //PlayerPrefs.SetInt("Branch", ) // 예) 가지위치 + 1~2 1이면 왼쪽 오른쪽, 2면

        return true;
    }
}

//PlayerPrefs.SetFloat("BonBranchScale_0", 0f); // 꽃의 가지 스케일값 저장(0)

//PlayerPrefs.SetFloat("BonBranchScale_1", 0f); // 꽃의 가지 스케일값 저장(1)
//PlayerPrefs.SetFloat("BonBranchGraviteScale_0", 0f); // 꽃의 가지 끝에 나뭇잎의 그라비티 스케일(0)
//PlayerPrefs.SetFloat("BonBranchGraviteScale_1", 0f);// 꽃의 가지 끝에 나뭇잎의 그라비티 스케일(1)
//PlayerPrefs.SetFloat("BonBranchRGBAScale_G_0", 204f); //가지의 메트리얼 색상값 저장
//PlayerPrefs.SetFloat("BonBranchRGBAScale_G_1", 204f); //가지의 메트리얼 색상값 저장

//string[] _setData = new string[4];

//_setData[0] = bonscale.ToString();
//_setData[1] = branch.ToString();
//_setData[2] = branchScales[0].ToString();
//_setData[3] = branchScales[1].ToString();