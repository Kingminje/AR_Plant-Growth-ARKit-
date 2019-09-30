using UnityEngine;

namespace FIMSpace.Jiggling
{
    /// <summary>
    /// FM : 변형의 회전 및 크기를 애니메이션하여 젤리처럼 보입니다.
    /// </summary>
    public class FJiggling_Simple : FJiggling_Base
    {
        //[Header("Left empty - uses component's transform")]
        public Transform TransformToAnimate;

        [Header("For more custom animations")]
        public Vector3 ScaleAxesMultiplier = Vector3.one;

        public Vector3 RotationAxesMultiplier = new Vector3(1f, 0f, 1f);

        /* 변환의 초기 상태 기억하기 */
        public Quaternion initRotation;
        public Vector3 initScale;

        /* 애니메이팅 중에 두 번째로 jiggling이 실행되면 object와 같은 효과를내는 변수가 다시 hitted됩니다. */

        [Space(5f)]
        [Tooltip("애니메이션 작업 중 다른 시간에 jiggling이 트리거 될 때 기울이기 영향이 얼마나 커야합니까?")]
        public float ReJigglePower = 2.25f;

        protected float reJiggleProgress = 0f;
        protected float reJiggleValue = 0f;
        protected float reJiggleRandomOffset;

        protected override void Init()
        {
            if (initialized) return;

            base.Init();

            if (!TransformToAnimate) TransformToAnimate = transform;

            initRotation = TransformToAnimate.localRotation;
            initScale = TransformToAnimate.localScale;

            if (ConstantJiggleValue <= 0f) enabled = false;
        }

        protected virtual void Reset()
        {
            TransformToAnimate = transform;
        }

        public override void StartJiggle()
        {
            base.StartJiggle();

            easedPowerProgress = 1f;
            reJiggleRandomOffset = Random.Range(-0.2f, 0.2f);
        }

        protected override void ReJiggle()
        {
            base.ReJiggle();
            reJiggleProgress = 1f;
        }

        /// <summary>
        /// 기본 클래스에서 계산 된 변수를 사용하여 애니메이션 변환
        /// </summary>
        protected override void CalculateJiggle()
        {
            base.CalculateJiggle();

            Transform t = TransformToAnimate;

            float val1 = 0f;
            float val2 = 0f;

            for (int i = 0; i < RandomLevel * 2; i++)
            {
                if (i % 2 == 0)
                    val1 += trigParams[i].Value;
                else
                    val2 += trigParams[i].Value;
            }

            val1 /= RandomLevel;
            val2 /= RandomLevel;

            if (reJiggled)
            {
                if (reJiggleProgress > 0f)
                {
                    float targetValue = Mathf.Sin(time * 1.45f * trigParams[1].RandomTimeMul + reJiggleRandomOffset + trigParams[0].TimeOffset + trigParams[1].TimeOffset) * JiggleTiltValue * trigParams[0].Multiplier * reJiggleProgress * ReJigglePower;
                    reJiggleValue = Mathf.Lerp(reJiggleValue, targetValue, Time.deltaTime * 12f);
                    reJiggleProgress -= Time.deltaTime * JiggleDecelerate * 1.75f;
                }
                else reJiggleProgress = 0f;
            }

            // Additional variation to rotating
            float add1 = 0f;
            float add2 = 0;
            if (RandomLevel > 1)
            {
                add1 = trigParams[3].Value;
                add2 = trigParams[2].Value;
            }

            t.transform.localRotation = initRotation * Quaternion.Euler((val1 + reJiggleValue + add1) * RotationAxesMultiplier.x, (-val2 + reJiggleValue - add1) * RotationAxesMultiplier.y, (val2 + reJiggleValue + add2) * RotationAxesMultiplier.z);

            t.transform.localScale = initScale + new Vector3(trigParams[0].Value * ScaleAxesMultiplier.x, (((trigParams[0].Value + trigParams[1].Value) / 2f)) * ScaleAxesMultiplier.y, trigParams[0].Value * ScaleAxesMultiplier.z) * 0.01f;

            if (animationFinished) OnAnimationFinish();
        }

        protected override void OnAnimationFinish()
        {
            TransformToAnimate.localRotation = initRotation;
            TransformToAnimate.localScale = initScale;
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// FM : Editor 레벨에서 애니메이션을 확인하는 Jiggle Simple 컴포넌트의 에디터 클래스 (playmode에서)
    /// </summary>
    [UnityEditor.CustomEditor(typeof(FJiggling_Simple))]
    public class FJiggling_SimpleEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            FJiggling_Simple targetScript = (FJiggling_Simple)target;
            DrawDefaultInspector();

            GUILayout.Space(10f);

            if (!Application.isPlaying) GUI.color = FColorMethods.ChangeColorAlpha(GUI.color, 0.45f);
            if (GUILayout.Button("Jiggle It")) if (Application.isPlaying) targetScript.StartJiggle(); else Debug.Log("You must be in playmode to run this method!");
        }
    }

#endif
}