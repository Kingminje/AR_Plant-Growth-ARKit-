using UnityEngine;

namespace FIMSpace.Jiggling
{
    public class FJiggling_Demo_Limb : FJiggling_Simple
    {
        /// <summary>
        /// 회전 키 프레임 없음
        /// </summary>
        public bool NoRotationKeyframes = false;

        /// <summary>
        /// 최기 키 회전
        /// </summary>
        private Quaternion initialKeyRotation;

        protected override void Init()
        {
            if (initialized) return;

            base.Init();
            initialKeyRotation = TransformToAnimate.localRotation;
        }

        protected override void Update()
        {
        }

        protected virtual void LateUpdate()
        {
            if (NoRotationKeyframes) transform.localRotation = initialKeyRotation;

            // 후기 업데이트 교체의 모든 시작은 Animator 구성 요소가 재생하는 애니메이션과 동일합니다.
            initRotation = transform.localRotation;

            base.Update();
            //if (animationFinished) enabled = true;
        }

        private void OnMouseDown()
        {
            StartJiggle();
        }
    }
}