using System.Collections;
using UnityEngine;

namespace FIMSpace.Basics
{
    /// <summary>
    /// FM : 패널의 버튼을 움직이게하는 클래스
    /// </summary>
    public class FBasic_Shared_PanelAnimations : MonoBehaviour
    {
        [Tooltip("버튼이 내려가는 순간의 애니메이션 재생 시간")]
        public float AnimationTime = 1f;

        [Tooltip("다른 기능의 탄력성을 약간 변경하는 기능을 완화하기위한 부가 가치")]
        public float EaseExtraValue = 0.85f;

        private Transform buttonTransform;
        private Vector3 buttonInitPosition;

        protected virtual void Start()
        {
            buttonTransform = transform.Find("Button");
            buttonInitPosition = buttonTransform.localPosition;
        }

        public virtual void Click()
        {
            StopAllCoroutines();
            StartCoroutine(ClickAniamtion());
        }

        /// <summary>
        /// 때때로 전체 시간 동안 업데이트가 실행되기 때문에 courutine이 Update ()보다 낫습니다.
        /// 그러나 courutine은 필요할 때만 사용할 수 있습니다.
        /// 빈 Update ()를 사용하는 약 1000 가지 동작이 CPU에서 과부하를 일으킬 수 있음을 확인합니다.
        /// </summary>
        private IEnumerator ClickAniamtion()
        {
            buttonTransform.localPosition = buttonInitPosition;
            float time = 0f;

            while (time < AnimationTime * 0.6f)
            {
                time += Time.deltaTime;

                float progress = time / AnimationTime;

                buttonTransform.localPosition = Vector3.LerpUnclamped(buttonInitPosition, buttonInitPosition - Vector3.up * 0.05f, FEasing.EaseOutElastic(0f, 1f, progress, EaseExtraValue));

                yield return null;
            }

            time = 0f;

            Vector3 currentPos = buttonTransform.localPosition;

            while (time < AnimationTime / 4f)
            {
                time += Time.deltaTime;

                float progress = time / (AnimationTime / 4f);

                buttonTransform.localPosition = Vector3.LerpUnclamped(currentPos, buttonInitPosition, FEasing.EaseInOutCubic(0f, 1f, progress));

                yield return null;
            }

            yield break;
        }
    }
}