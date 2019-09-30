using FIMSpace.Basics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: 꼬리 움직임에 정현파 흔들기를 추가하는 기능이있는 파생 된 클래스입니다.
    /// </summary>
    public class FTail_Animator : FTail_AnimatorBase
    {
        public bool UseWaving = true;

        [Tooltip("애니메이션 흔들기에 변형 추가하기")]
        public bool CosinusAdd = false;

        public float WavingSpeed = 3f;
        public float WavingRange = 0.8f;
        public Vector3 WavingAxis = new Vector3(1f, 0.0f, 0f);

        public Vector3 TailRotationOffset = Vector3.zero;

        [Tooltip("모델 (첫 번째 뼈 제외)의 전체 뼈 체인 연결 끊기 - 모션이보다 자유롭고 계층 구조와 관련된 몇 가지 요소와 독립적입니다. 필요할 때만 수행하는 것이 좋습니다.")]
        public bool DisconnectTransforms = false;

        /// <summary> 다른 모든 연결이 끊긴 뼈를 다른 물건과 함께 담아 모든 것을 깨끗하게 유지합니다.</summary>
        protected static Transform disconnectedContainer;

        /// <summary> 이 변환 안에서 뼈 또는 뼈 모방이 될 것입니다.</summary>
        protected Transform localDisconnectedContainer;

        /// <summary> 삼각 함수 시간 변수 </summary>
        protected float waveTime;

        protected float cosTime;

        private int RefreshCounter = 0;

        protected override void Init()
        {
            base.Init();

            waveTime = Random.Range(-Mathf.PI, Mathf.PI);
            cosTime = Random.Range(-Mathf.PI, Mathf.PI);
        }

        protected virtual void WavingCalculations()
        {
            if (!RootToParent)
            {
                if (UseWaving)
                {
                    waveTime += Time.deltaTime * (2 * WavingSpeed);

                    Vector3 rot = firstBoneInitialRotation + TailRotationOffset;

                    float sinVal = Mathf.Sin(waveTime) * (30f * WavingRange);

                    if (CosinusAdd)
                    {
                        cosTime += Time.deltaTime * (2.535f * WavingSpeed);
                        sinVal += Mathf.Cos(cosTime) * (27f * WavingRange);
                    }

                    rot += sinVal * WavingAxis;

                    if (rootTransform)
                        proceduralPoints[0].Rotation = rootTransform.rotation * Quaternion.Euler(rot);
                    else
                        proceduralPoints[0].Rotation = TailTransforms[0].transform.rotation * Quaternion.Euler(rot);
                }
                else
                {
                    if (rootTransform)
                        proceduralPoints[0].Rotation = rootTransform.rotation * Quaternion.Euler(firstBoneInitialRotation);
                    else
                        proceduralPoints[0].Rotation = TailTransforms[0].transform.rotation * Quaternion.Euler(firstBoneInitialRotation);
                }
            }
        }

        /// <summary>
        /// 다른 계산 전에 첫 번째 뼈에 대한 부비동 파 회전 추가
        /// </summary>
        public override void CalculateOffsets()
        {
            // 모션 계산을하기 전에 여기서는 깔끔한 것이 아니라, 끝나야합니다.
            if (RefreshHelpers)
            {
                RefreshCounter--;
                if (RefreshCounter < -5)
                {
                    RefreshHelpers = false;
                    CoputeHelperVariables();
                    RefreshCounter = 0;
                }
            }

            WavingCalculations();

            base.CalculateOffsets();

            SetTailTransformsFromPoints();
        }

        /// <summary>
        /// 연결 해제 기능 지원
        /// </summary>
        protected override void ConfigureBonesTransforms()
        {
            base.ConfigureBonesTransforms();

            if (DisconnectTransforms)
                for (int i = 1; i < TailTransforms.Count; i++)
                {
                    TailTransforms[i].SetParent(GetDisconnectedContainer(), true);
                }
        }

        /// <summary>
        /// 연결 해제 된 뼈에 대한 로컬 컨테이너 변형을 반환합니다.
        /// </summary>
        protected Transform GetDisconnectedContainer()
        {
            if (disconnectedContainer == null)
                disconnectedContainer = new GameObject("[Tail Animator Container]").transform;

            if (localDisconnectedContainer == null)
            {
                localDisconnectedContainer = new GameObject("Tail Container [" + name + "]").transform;
                localDisconnectedContainer.SetParent(disconnectedContainer, true);
            }

            return localDisconnectedContainer;
        }

        internal virtual void Update()
        {
            if (UpdateClock != EFUpdateClock.Update) return;
            if (!initialized) return;
            CalculateOffsets();
        }

        internal virtual void LateUpdate()
        {
            if (UpdateClock != EFUpdateClock.LateUpdate) return;
            if (!initialized) return;
            if (Time.deltaTime <= 0) return;

            CalculateOffsets();
        }

        internal virtual void FixedUpdate()
        {
            if (UpdateClock != EFUpdateClock.FixedUpdate) return;
            if (!initialized) return;
            CalculateOffsets();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (localDisconnectedContainer) Destroy(localDisconnectedContainer.gameObject);
        }

        // V1.2.0
#if UNITY_EDITOR

        /// <summary>
        /// 모든 꼬리 변형을 정의하지 않고 기즈모에 대한 꼬리 목록 체인 업데이트 (tailBones 목록에 추가되지 않은 경우 자동 수집)
        /// </summary>
        public override void OnValidate()
        {
            GetEditorGizmoTailList();
        }

        /// <summary>
        /// 장면 창에 기즈모를 그리기
        /// </summary>
        override protected void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (!drawGizmos) return;

            if (editorGizmoTailList != null)
            {
                if (TailTransforms == null || TailTransforms.Count == 0)
                {
                    // 세그먼트 아이콘을 그리지 않습니다 (아이콘 기즈모 아이콘과 겹치지 않습니다)
                }
                else if (editorGizmoTailList.Count > 0) Gizmos.DrawIcon(editorGizmoTailList[0].position, "FIMSpace/FTail/SPR_TailAnimatorGizmoSegment.png");

                for (int i = 1; i < editorGizmoTailList.Count; i++)
                {
                    if (editorGizmoTailList[i] == null) continue;
                    Gizmos.DrawIcon(editorGizmoTailList[i].position, "FIMSpace/FTail/SPR_TailAnimatorGizmoSegment.png");
                }
            }
        }

#endif
    }
}