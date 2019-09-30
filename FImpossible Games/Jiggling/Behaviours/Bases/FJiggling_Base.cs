using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.Jiggling
{
    /// <summary>
    /// FM : 흔들기와 같은 애니메이션을 만드는 데 유용한 변수를 계산하는 메소드가있는 클래스
    /// </summary>
    public abstract class FJiggling_Base : MonoBehaviour
    {
        [Tooltip("얼마나 흔들리게 할것 인지 옵션 값")]
        public float JiggleTiltValue = 12f; // 흔들림 틸트 값

        [Tooltip("얼마나 빠르게 흔들리게 할것 인지 옵션 값")]
        public float JiggleFrequency = 24f; // 흔들리는 주파수

        [Tooltip("재생중인 애니메이션 느려지는 속도 옵션 값")]
        public float JiggleDecelerate = 1.75f; // 흔들림이 줄어 듭니다.

        [Tooltip("약간의 효과를 변경하는 옵션 값")]
        [Range(0.5f, 2f)]
        public float AdditionalSpeedValue = 1f; // 반복 속도 값 추가

        [Tooltip("조금의 효과를 변경하는 옵션값")]
        [Range(0.0f, 0.5f)]
        public float ConstantJiggleValue = 0f; // 일정한 흔들림 값

        /// <summary> 삼각 함수 시간 (더 많은 제어가 필요하기 때문에 Time.time을 사용하지 않음) </summary>
        protected float time;

        /// <summary> 애니메이션이 끝난 경우 </summary>
        protected bool animationFinished = false;

        /* 삼각 함수, 값, 변형에 대한 변수 */
        protected List<TrigonoParams> trigParams;

        /// <summary> 흔들림 애니메이션을 만드는 데 사용되는 삼각 함수의 수 </summary>
        [Tooltip("높은 무작위 레버 - 무작위로 더 많이 움직입니다.")]
        [Range(1, 3)]
        public int RandomLevel = 1;

        /// <summary> 애니메이션의 강도를 변경하는 변수 </summary>
        protected float targetPowerValue = 1.2f;

        protected float easedPowerProgress = 1f;
        protected float currentJigglePower = 0.15f;
        protected bool reJiggled = false;

        /// <summary> 초기화 메소드 제어 플래그 </summary>
		protected bool initialized = false;

        /// <summary>
        /// Start () 메소드를 기다리는 것보다 더 많은 제어를하기 위해 컴포넌트를 초기화하는 메소드. 프로그래머가 필요로하기 때문에, init은 시작 전후에 실행될 수있다.
        /// </summary>
        protected virtual void Init()
        {
            if (initialized) return;

            RandomizeVariables();

            if (ConstantJiggleValue > 0f)
            {
                easedPowerProgress = 0.1f;
                targetPowerValue = 0.1f;
            }

            initialized = true;
        }

        private void Awake()
        {
            Init();
        }

        private void OnValidate()
        {
            if (Application.isPlaying) RandomizeVariables();
        }

        /// <summary>
        /// 흔들림 계산 및 애니메이션 메서드 실행
        /// </summary>
        protected virtual void Update()
        {
            CalculateJiggle();
            if (animationFinished) enabled = false;
        }

        /// <summary>
        /// 애니메이션이 끝날 때 사용자 지정 작업을 넣을 수있는 공간
        /// </summary>
        protected virtual void OnAnimationFinish()
        {
        }

        /// <summary>
        /// 구성 요소 활성화 및 기본 변수 재설정으로 흔들림 애니메이션 시작
        /// </summary>
        public virtual void StartJiggle()
        {
            Init();
            enabled = true;

            if (animationFinished)
            {
                animationFinished = false;

                targetPowerValue = 1.15f;
                easedPowerProgress = 1f;
                currentJigglePower = 0.15f;

                RandomizeVariables();

                reJiggled = false;
            }
            else
            {
                easedPowerProgress = 1.1f;
                ReJiggle();
            }
        }

        /// <summary>
        /// 흔들림이 애니메이션 중에 다시 실행될 때
        /// </summary>
        protected virtual void ReJiggle()
        {
            reJiggled = true;
        }

        /// <summary>
        /// 애니메이션을 위한 삼각 함수 변수를 매번 다르게 재설정하기
        /// </summary>
        protected virtual void RandomizeVariables()
        {
            time = Random.Range(-Mathf.PI, Mathf.PI);

            trigParams = new List<TrigonoParams>();

            // 각각의 임의의 레벨은 2 개의 삼각 함수  (걸프 및 코사인)
            for (int i = 0; i < RandomLevel; i++)
            {
                for (int t = 0; t < 2; t++)
                {
                    TrigonoParams newParams = new TrigonoParams();
                    newParams.Randomize();
                    trigParams.Add(newParams);
                }
            }
        }

        /// <summary>
        /// 흔들 거리는 애니메이션을 시뮬레이트하기위한 삼각 함수 변수 계산
        /// </summary>
        protected virtual void CalculateTrigonometricVariables(float timeMultiplier = 1f)
        {
            time += Time.deltaTime * (JiggleFrequency * timeMultiplier);

            currentJigglePower = Mathf.Lerp(currentJigglePower, targetPowerValue, Time.deltaTime * 20f);

            for (int i = 0; i < RandomLevel; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    TrigonoParams parameters = trigParams[i + j];

                    float timeValue = time * parameters.RandomTimeMul + parameters.TimeOffset;
                    float multiplyValue = JiggleTiltValue * parameters.Multiplier * currentJigglePower / AdditionalSpeedValue;

                    if (j % 2 == 0)
                        parameters.Value = Mathf.Sin(timeValue) * multiplyValue;
                    else
                        parameters.Value = Mathf.Cos(timeValue) * multiplyValue;
                }
            }

            easedPowerProgress -= Time.deltaTime * JiggleDecelerate * AdditionalSpeedValue;
            easedPowerProgress = Mathf.Max(ConstantJiggleValue, easedPowerProgress);

            targetPowerValue = EaseInOutCubic(0f, 1f, easedPowerProgress);
        }

        /// <summary>
        /// 삼각 함수 값 계산 및 애니메이션 완료 여부 정의
        /// 계산 된 삼각 함수 값의 맞춤 사용을위한 장소
        /// </summary>
        protected virtual void CalculateJiggle()
        {
            CalculateTrigonometricVariables();

            if (easedPowerProgress <= 0f)
            {
                if (ConstantJiggleValue <= 0f) animationFinished = true;
            }
        }

        /// <summary>
        /// FM : 애니메이션의 흔들림 매개 변수를 애니메이트하기위한 삼각 함수 매개 변수를 포함하는 컨테이너 클래스
        /// </summary>
        [System.Serializable]
        public class TrigonoParams
        {
            public float Value;
            public float Multiplier;
            public float TimeOffset;
            public float RandomTimeMul;

            /// <summary>
            /// 변수에 임의의 값 가져 오기
            /// </summary>
            public void Randomize()
            {
                Value = 0f;
                Multiplier = Random.Range(0.85f, 1.15f);

                TimeOffset = Random.Range(-Mathf.PI, Mathf.PI);
                RandomTimeMul = Random.Range(0.8f, 1.2f);
            }
        }

        public static float EaseInOutCubic(float start, float end, float value)
        {
            value /= .5f;
            end -= start;

            if (value < 1) return end * 0.5f * value * value * value + start;

            value -= 2;

            return end * 0.5f * (value * value * value + 2) + start;
        }

#if UNITY_EDITOR

        public bool drawGizmo = true;

        protected virtual void OnDrawGizmos()
        {
            if (!drawGizmo) return;
            Gizmos.DrawIcon(transform.position, "FIMSpace/Jiggling/Jiggling Gizmo.png");
        }

#endif
    }
}