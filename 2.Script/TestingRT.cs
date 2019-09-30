using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingRT : MonoBehaviour
{
    public SkinnedMeshRenderer nowskin;

    private void Start()
    {
        StartCoroutine(ChangeColorCo(20f));
    }

    private void Update()
    {
        //ChangeColor(1f);
    }

    public void ChangeColor(float changeC)
    {
        var mat = transform.GetComponent<SkinnedMeshRenderer>().material;

        Color main = mat.color;

        Color change = new Color(main.r, changeC / 255f, main.b, main.a);

        mat.color = Color.Lerp(main, change, Time.deltaTime * 0.5f);
    }

    public IEnumerator ChangeColorCo(float changeC)
    {
        float timer = 0f;
        float speed = 0.01f;

        var mat = transform.GetComponent<SkinnedMeshRenderer>().material;

        float changNum = Mathf.Floor((changeC / 255f) * 100f) * 0.01f;
        float matGNum = mat.color.g;

        Debug.Log(changNum);

        while (matGNum >= changNum)
        {
            matGNum = Mathf.Floor(mat.color.g * 100f) * 0.01f;

            if (matGNum == changNum)
            {
                Debug.Log("eee");
                yield break;
            }

            Color main = mat.color;

            Color change = new Color(main.r, changeC / 255f, main.b, main.a);

            Debug.Log(matGNum);

            timer = Mathf.Clamp(timer + Time.deltaTime * speed, 0.0f, 1.0f);
            speed += Time.deltaTime * 0.05f;

            mat.color = Color.Lerp(main, change, timer);

            yield return new WaitForFixedUpdate();
        }
    }
}

/*
  public IEnumerator ROT()
  {
      float speed = 0f;
      float timer = 0f;
      for (; ; )
      {
          if (transform.eulerAngles.x == 50f)
          {
              Debug.Log(1f);
              yield break;
          }
          Quaternion rotation = Quaternion.identity;
          rotation.eulerAngles = new Vector3(90f, 0f, 0f);

          Quaternion a = transform.localRotation;

          timer = Mathf.Clamp(timer + Time.deltaTime * speed, 0.0f, 1.0f);

          speed += Time.deltaTime * 0.05f;
          transform.localRotation = Quaternion.Lerp(a, rotation, timer * 0.5f);
          yield return new WaitForEndOfFrame();
      }
  }*/