using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkControl : MonoBehaviour
{
    protected Animator animator;

    public bool ikActive = false;
    public Transform TagetTr;
    public Transform lookObj = null;

    private Vector3 maxsize = new Vector3(0f, 5f, 0f);

    public float startSize;
    public float stopSize;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public bool FlowerIKSet(float scale)
    {
        if (scale > startSize && ikActive == false)
        {
            TagetTr.localPosition += Vector3.Lerp(TagetTr.localPosition, maxsize, Time.deltaTime * 15f) /*new Vector3(1f, 0f, 0f)*/;

            if (scale > stopSize)
            {
                ikActive = true;
            }
            return true;
        }

        return false;
    }

    private void OnAnimatorIK(/*int layerIndex*/)
    {
        if (animator)
        {
            // IK가 활성화 된 경우 위치 및 회전을 목표에 직접 설정합니다.
            if (ikActive)
            {
                // 보이는 목표 위치가 지정되면 설정합니다.
                if (lookObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                // 오른쪽 목표 위치와 회전이 설정되어 있으면이를 설정합니다.
                if (TagetTr != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, TagetTr.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, TagetTr.rotation);
                }
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }
}