using FIMSpace.Basics;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: 꼬리 같은 프로 시저 애니메이션을위한 기본 스크립트
    /// </summary>
    public abstract class FTail_AnimatorBase : MonoBehaviour
    {
        //비어있는 경우 자동 감지
        [Header("[ Auto detection if left empty ]", order = 0)]
        [Space(-8f)]
        [Header("[ or put here first bone ]", order = 2)]
        /// <summary> 뼈 목록 </summary>
        public List<Transform> TailTransforms;

        [Tooltip("체인의 첫 번째 뼈 대신 부모에게 핀 제어 회전 모션을 적용하려는 경우")]
        public bool RootToParent = false;

        [Tooltip("인스펙터 창 안쪽에 하나의 본을 지정할 때 자동 가져 오기를 사용하려는 경우")]
        public bool AutoGetWithOne = true;

        /// <summary> 꼬리에 대한 유령 애니메이션을 나타내는 편집 점에 대해 보이지 않는 목록 </summary>
        protected List<FTail_Point> proceduralPoints;

        [Header("[ Tail behaviour params ]")]
        [Tooltip("위치 속도는 꼬리 부분이 얼마나 빨리 목표 위치로 돌아갈지를 정의합니다. 애니메이션이 더 거친 느낌을줍니다.")]
        [Range(5f, 60f)]
        public float PositionSpeed = 35f;

        [Tooltip("회전 속도는 얼마나 빨리 꼬리 부분이 목표 회전으로 돌아갈지를 정의합니다. 애니메이션이 더 낮 으면 게으른 느낌을줍니다. ")]
        [Range(5f, 60f)]
        public float RotationSpeed = 20f;

        protected List<Transform> editorGizmoTailList;

        //[Header("[ Tuning modificators ]")]
        [Tooltip("꼬리 애니메이션이 올바르게 보이게하기 위해 일부 미세 조정 설정을 자동으로 변경합니다.")]
        public bool UseAutoCorrectLookAxis = true;

        public bool FullCorrection = false;

        [Tooltip("물결 치면 모델이 이상하게 굴러 갈 때이 옵션을 사용하십시오.")]
        public bool RolledBones = false;

        public bool AnimateCorrections = false;

        public float StretchMultiplier = 1f;

        [Tooltip("뼈 틀린 회전 축 교정기")]
        [Space(8f)]
        public Vector3 AxisCorrection = new Vector3(0f, 0f, 1f);

        public Vector3 AxisLookBack = new Vector3(0f, 1f, 0f);

        [HideInInspector]
        public bool ExtraCorrectionOptions = false;

        public Vector3 ExtraFromDirection = new Vector3(0f, 0f, 1f);
        public Vector3 ExtraToDirection = new Vector3(0f, 0f, 1f);

        [Tooltip("이 옵션은 모든 꼬리 세그먼트에 TailReference 구성 요소를 추가하므로 꼬리의 세그먼트 변환에서이 구성 요소에 액세스 할 수 있습니다.")]
        public bool AddTailReferences = false;

        // V1.2
        [Tooltip("자체 애니메이션과 함께 객체 위에 컴포넌트를 사용하려면 업데이트 클럭을 LateUpdate로 설정하십시오.")]
        public EFUpdateClock UpdateClock = EFUpdateClock.Update;

        [Tooltip("Update 또는 LateUpdate를 사용할 때 부드러운 델타 시간을 사용할 수 있으며, 프레임 속도가 안정적이지 않을 때 일부 절단을 제거 할 수 있습니다")]
        public bool SmoothDeltaTime = true;

        // V1.2.2
        [Tooltip("예를 들어, 모델이 초기 T-Pose ( 'Animate Corrections'를 사용할 때)와는 다른 애니메이션 중에 포즈를 취할 때 사용하려면")]
        public bool RefreshHelpers = false;

        // V1.2.3
        [Tooltip("다른 컴포넌트를 사용하여 뼈대 계층에 영향을 줄 때이 컴포넌트가 다른 컴포넌트의 변경 사항을 따르길 원할 때 유용합니다.")]
        public bool QueueToLastUpdate = true;

        // V1.2.6
        [Tooltip("[실험] 몇 가지 간단한 계산을 사용하여 콜라에 꼬리 벤드 만들기")]
        public bool UseCollision = false;

        public bool CollideWithOtherTails = false;
        public FTailColliders CollidersType = FTailColliders.Spheres;

        public enum FTailColliders { Boxes, Spheres }

        public AnimationCurve CollidersScale = AnimationCurve.Linear(0, 1, 1, 1);
        public float CollidersScaleMul = 6.5f;
        public Vector3 BoxesDimensionsMul = Vector3.one;
        public List<Collider> IgnoredColliders;
        public bool CollidersSameLayer = true;

        [Tooltip("각 꼬리 세그먼트의 충돌 자에 강체를 추가하면 충돌은 모든 것에 작용하지만 덜 최적이됩니다. 여기에 강체를 추가 할 필요는 없지만 객체 세그먼트에 운동 학적 강체가 충돌하지 않아야합니다")]
        public bool CollidersAddRigidbody = true;

#if UNITY_EDITOR

        [FIMSpace.FEditor.FPropDrawers_LayersN]
#endif
        public LayerMask CollidersLayer = 0;

        [Range(0f, 1f)]
        public float DifferenceScaleFactor = 1f;

        // V1.2.6
        [Tooltip("테일 애니메이션에 비해 글로벌 추가 힘을 시뮬레이트하고 싶다면 중력과 똑같지는 않지만 단순한 계산으로 이것을 모방하려고합니다")]
        public Vector2 GravityPower = Vector3.zero;

        /// <summary> 초기화 메소드 제어 플래그 </summary>
		protected bool initialized;

        public bool IsInitialized { get; protected set; }

        /// <summary> 애니메이션 중에 꼬리 부분 사이에 올바른 배치를위한 꼬리 변환 사이의 초기 거리를 기억하십시오.</summary>
        protected List<float> distances;

        /// <summary> 첫 꼬리 변형의 부모 변환 </summary>
        protected Transform rootTransform;

        /// <summary> Init ()에서 감지되고 꼬리 구성을 허용하는 변수는 뼈의 회전 방향이 설정되는 방식과는 독립적으로 회전을 추적합니다 </summary>
        protected List<Vector3> tailLookDirections;

        protected List<Vector3> lookBackDirections;
        protected List<Quaternion> lookBackOffsets;
        protected List<Quaternion> animatedCorrections;

        protected Vector3 firstBoneInitialRotation = Vector3.zero;
        protected Quaternion firstBoneInitialRotationQ = Quaternion.identity;

        protected bool preAutoCorrect = false;

        protected List<Vector3> collisionOffsets;
        protected List<float> collisionFlags;
        protected List<Collision> collisionContacts;

        protected virtual void Reset()
        {
            IsInitialized = false;
        }

        /// <summary>
        /// Start () 메소드를 기다리는 것보다 더 많은 제어를하기 위해 컴포넌트를 초기화하는 메소드. 프로그래머가 필요로하기 때문에, init은 시작 전후에 실행될 수있다.
        /// </summary>
        protected virtual void Init()
        {
            if (initialized) return;

            string name = transform.name;
            if (transform.parent) name = transform.parent.name;

            ConfigureBonesTransforms();

            CoputeHelperVariables();

            PrepareTailPoints();

            if (QueueToLastUpdate) QueueComponentToLastUpdate();

            if (TailTransforms.Count == 1)
            {
                // 경고 : 체인에서 뼈 하나만 사용되었다고 감지하면 자동으로 설정합니다.
                RootToParent = true;

                if (TailTransforms[0].parent == null) Debug.LogError("부모 참조 변환이없는 단일 본에 테일 애니메이터를 사용하고 싶습니다!");
            }

            if (UseCollision) AddColliders();

            initialized = true;
            IsInitialized = true;
        }

        /// <summary>
        /// 도우미 및 해결사 변수 계산
        /// </summary>
        protected virtual void CoputeHelperVariables()
        {
            // Extra fixing variables
            tailLookDirections = new List<Vector3>();
            lookBackDirections = new List<Vector3>();
            lookBackOffsets = new List<Quaternion>();
            animatedCorrections = new List<Quaternion>();

            // 애니 메이팅을위한 몇 가지 추가 수정 본 구조에 대한 사전 계산 꼬리 모양 지시
            if (TailTransforms.Count > 0)
            {
                firstBoneInitialRotation = TailTransforms[0].localRotation.eulerAngles;
                firstBoneInitialRotationQ = TailTransforms[0].localRotation;

                if (AddTailReferences)
                {
                    for (int i = 0; i < TailTransforms.Count; i++)
                    {
                        if (TailTransforms[i] == transform) continue;
                        if (!TailTransforms[i].GetComponent<FTail_Reference>()) TailTransforms[i].gameObject.AddComponent<FTail_Reference>().TailReference = this;
                    }
                }

                for (int i = 0; i < TailTransforms.Count; i++)
                {
                    lookBackDirections.Add(TailTransforms[i].localRotation * Vector3.forward);
                    lookBackOffsets.Add(TailTransforms[i].localRotation);
                    animatedCorrections.Add(TailTransforms[i].localRotation);
                }

                for (int i = 0; i < TailTransforms.Count - 2; i++)
                {
                    tailLookDirections.Add(-
                        (
                        TailTransforms[i].InverseTransformPoint(TailTransforms[i + 1].position)
                        -
                        TailTransforms[i].InverseTransformPoint(TailTransforms[i].position)
                        )
                        .normalized);
                }

                if (TailTransforms.Count == 1)
                {
                    Vector3 rootLook = -(TailTransforms[0].parent.InverseTransformPoint(TailTransforms[0].position) - TailTransforms[0].parent.InverseTransformPoint(TailTransforms[0].parent.position)).normalized;
                    tailLookDirections.Add(rootLook);
                }
                else
                if (TailTransforms.Count == 2)
                {
                    Vector3 preLook = -(TailTransforms[0].InverseTransformPoint(TailTransforms[1].position) - TailTransforms[0].InverseTransformPoint(TailTransforms[0].position)).normalized;
                    tailLookDirections.Add(preLook);
                }
                else
                    tailLookDirections.Add(tailLookDirections[tailLookDirections.Count - 1]);
            }

            rootTransform = TailTransforms[0].parent;

            // 애니메이션의 올바른 위치를 계산하기 위해 뼈 사이의 초기 거리 기억하기
            distances = new List<float>();
            if (TailTransforms[0].parent == null) distances.Add(0f); else distances.Add(Vector3.Distance(TailTransforms[0].position, TailTransforms[0].parent.position));

            for (int i = 1; i < TailTransforms.Count; i++)
                distances.Add(Vector3.Distance(TailTransforms[i].position, TailTransforms[i - 1].position));
        }

        /// <summary>
        /// 인스펙터에서 정의되지 않은 경우 자동으로 꼬리 변환을 수집합니다.
        /// 이것은 또한 재정의하고 더 많은 것을 구성하기위한 장소입니다.
        /// </summary>
        protected virtual void ConfigureBonesTransforms()
        {
            AutoGetTailTransforms();
        }

        /// <summary>
        /// 꼬리 구조를 자동으로 정의하기위한 자식 뼈 가져 오기
        /// </summary>
        public void AutoGetTailTransforms(bool editor = false)
        {
            if (TailTransforms == null) TailTransforms = new List<Transform>();

            bool can = true;
            if (!AutoGetWithOne && !editor) can = false;
            if (!can) return;

            if (TailTransforms.Count < 2)
            {
                Transform lastParent = transform;

                bool boneDefined = true;

                // (V1.1) Start parent
                if (TailTransforms.Count == 0)
                {
                    boneDefined = false;
                    lastParent = transform;
                }
                else lastParent = TailTransforms[0];

                Transform rootTransform = lastParent;

                // 내가 while () 루프를 두려워하기 때문에 100 번 반복 : O는 누군가가 필요로한다면 100 또는 1000으로 제한합니다.
                for (int i = TailTransforms.Count; i < 100; i++)
                {
                    if (boneDefined)
                        if (lastParent == rootTransform)
                        {
                            if (lastParent.childCount == 0) break;
                            lastParent = lastParent.GetChild(0);
                            continue;
                        }

                    TailTransforms.Add(lastParent);

                    if (lastParent.childCount > 0) lastParent = lastParent.GetChild(0); else break;
                }
            }
        }

        /// <summary>
        /// 모든 꼬리 변환을 자유롭게 이동할 수 있도록 연결 해제
        /// </summary>
        protected virtual void PrepareTailPoints()
        {
            proceduralPoints = new List<FTail_Point>();

            for (int i = 0; i < TailTransforms.Count; i++)
            {
                FTail_Point p = new FTail_Point();
                p.index = i;
                p.Position = TailTransforms[i].position;
                p.Rotation = TailTransforms[i].rotation;
                p.InitialPosition = TailTransforms[i].localPosition;
                p.InitialRotation = TailTransforms[i].localRotation;

                proceduralPoints.Add(p);
            }
        }

        /// <summary>
        /// 올바른 작업을 위해 구성 요소 초기화
        /// </summary>
        protected void Start()
        {
            Init();
        }

        /// <summary>
        /// 파생 클래스에 대한 모션 계산 메서드 순서 지정을위한 주요 메서드
        /// </summary>
        public virtual void CalculateOffsets()
        {
            MotionCalculations();
        }

        /// <summary>
        /// 주어진 변형 목록에 대한 꼬리 모양의 움직임 애니메이션 논리 계산하기
        /// </summary>
        protected virtual void MotionCalculations()
        {
            if (UseCollision)
            {
                if (collisionOffsets == null) AddColliders();
            }

            if (preAutoCorrect != UseAutoCorrectLookAxis)
            {
                ApplyAutoCorrection();
                preAutoCorrect = UseAutoCorrectLookAxis;
            }

            if (AnimateCorrections)
                for (int i = 0; i < TailTransforms.Count; i++)
                    animatedCorrections[i] = TailTransforms[i].localRotation;

            // 애니메이션 변수 계산하기
            float posDelta;
            float rotDelta;

            if (UpdateClock == EFUpdateClock.FixedUpdate)
            {
                posDelta = Time.fixedDeltaTime * PositionSpeed;
                rotDelta = Time.fixedDeltaTime * RotationSpeed;
            }
            else
            {
                if (SmoothDeltaTime)
                {
                    posDelta = Time.smoothDeltaTime * PositionSpeed;
                    rotDelta = Time.smoothDeltaTime * RotationSpeed;
                }
                else
                {
                    posDelta = Time.deltaTime * PositionSpeed;
                    rotDelta = Time.deltaTime * RotationSpeed;
                }
            }

            if (!RootToParent)
            {
                proceduralPoints[0].Position = TailTransforms[0].position;
            }
            else
            {
                // Supporting root parent motion
                FTail_Point currentTailPoint = proceduralPoints[0];
                Vector3 startLookPosition = TailTransforms[0].parent.position;
                Vector3 translationVector;

                translationVector = TailTransforms[0].parent.TransformDirection(tailLookDirections[0]);

                Vector3 targetPosition = TailTransforms[0].parent.transform.position + (translationVector * -1f * (distances[0] * StretchMultiplier));

                FTail_Point temporaryPoint = new FTail_Point
                {
                    index = 0,
                    Position = TailTransforms[0].parent.position,
                    Rotation = TailTransforms[0].parent.rotation
                };

                Quaternion targetLookRotation = CalculateTargetRotation(startLookPosition, currentTailPoint.Position, temporaryPoint, currentTailPoint, -1);

                proceduralPoints[0].Position = Vector3.Lerp(currentTailPoint.Position, targetPosition, posDelta);
                proceduralPoints[0].Rotation = Quaternion.Lerp(currentTailPoint.Rotation, targetLookRotation, rotDelta);
            }

            for (int i = 1; i < proceduralPoints.Count; i++)
            {
                FTail_Point previousTailPoint = proceduralPoints[i - 1];
                FTail_Point currentTailPoint = proceduralPoints[i];

                Vector3 startLookPosition = previousTailPoint.Position;

                Vector3 translationVector;

                if (FullCorrection)
                    translationVector = previousTailPoint.TransformDirection(tailLookDirections[i - 1]);
                else
                    translationVector = previousTailPoint.TransformDirection(AxisCorrection);

                Vector3 targetPosition = previousTailPoint.Position + (translationVector * -1f * (distances[i] * StretchMultiplier));

                Quaternion targetLookRotation = CalculateTargetRotation(startLookPosition, currentTailPoint.Position, previousTailPoint, currentTailPoint, i - 1);

                proceduralPoints[i].Position = Vector3.Lerp(currentTailPoint.Position, targetPosition, posDelta);
                proceduralPoints[i].Rotation = Quaternion.Lerp(currentTailPoint.Rotation, targetLookRotation, rotDelta);

                if (UseCollision)
                {
                    if (collisionFlags[i] > 0f)
                        collisionFlags[i] -= Time.deltaTime * 4f;
                    else
                        collisionOffsets[i] = Vector3.zero;
                }
            }

            if (UseCollision) for (int i = 1; i < collisionContacts.Count; i++) UseCollisionContact(i);
            //if (UseCollision) for (int i = collisionContacts.Count-1; i >= 1; i--) UseCollisionContact(i);
        }

        /// <summary>
        /// 꼬리말을 설정하면 절차 포인트 애니메이션에서 세계의 위치와 회전을 변환합니다.
        /// </summary>
        protected virtual void SetTailTransformsFromPoints()
        {
            for (int i = 0; i < TailTransforms.Count; i++)
            {
                TailTransforms[i].position = proceduralPoints[i].Position;
                TailTransforms[i].rotation = proceduralPoints[i].Rotation;
            }
        }

        // V1.1 and V1.1.1/2
        /// <summary>
        /// 꼬리 변환 위치 및 회전 설정 한 꼬리 세그먼트의 목표 회전을 계산합니다.
        /// 2D 회전과 같은 일부 예외 계산을 위해 재정의합니다.
        /// </summary>
        protected virtual Quaternion CalculateTargetRotation(Vector3 startLookPos, Vector3 currentPos, FTail_Point previousTailPoint = null, FTail_Point currentTailPoint = null, int lookDirectionFixIndex = 0)
        {
            Quaternion targetRotation;

            //V1.2.5
            int fixDirForw = lookDirectionFixIndex + 1;
            if (lookDirectionFixIndex == -1)
            {
                fixDirForw = 0;
                lookDirectionFixIndex = 0;
            }

            if (FullCorrection)
            {
                targetRotation = Quaternion.identity;

                bool rotationCollision = false;

                if (UseCollision)
                    if (collisionFlags[fixDirForw] > 0f)
                        if (collisionOffsets[fixDirForw] != Vector3.zero)
                        {
                            rotationCollision = true;
                        }

                if (!rotationCollision)
                {
                    if (RolledBones)
                        targetRotation = Quaternion.LookRotation(startLookPos - currentPos, previousTailPoint.TransformDirection(-lookBackDirections[fixDirForw] * 0.99f));
                    else
                        targetRotation = Quaternion.LookRotation(startLookPos - currentPos, previousTailPoint.TransformDirection(AxisLookBack));
                }
                else
                {
                    //Quaternion target = Quaternion.LookRotation(collisionOffsets[fixDirForw], previousTailPoint.TransformDirection(AxisLookBack));
                    //// Quaternion target = Quaternion.LookRotation(collisionOffsets[fixDirForw], previousTailPoint.TransformDirection(AxisLookBack)) * Quaternion.FromToRotation(tailLookDirections[lookDirectionFixIndex], ExtraToDirection);
                    //targetRotation *= Quaternion.Slerp(Quaternion.identity, target, collisionFlags[fixDirForw]);

                    Vector3 tailDirection = (startLookPos - currentPos).normalized;
                    Vector3 upwards;

                    if (RolledBones) upwards = previousTailPoint.TransformDirection(-lookBackDirections[fixDirForw] * 0.99f); else upwards = previousTailPoint.TransformDirection(AxisLookBack);

                    Vector3 smoothedDirection = Vector3.Slerp(tailDirection, (tailDirection + collisionOffsets[currentTailPoint.index]).normalized, collisionFlags[fixDirForw]);
                    targetRotation = Quaternion.LookRotation(smoothedDirection, upwards);
                }

                if (GravityPower != Vector2.zero)
                {
                    float mul = 10 / (fixDirForw * 2.5f + 1);
                    targetRotation *= Quaternion.Euler(GravityPower.y * mul, GravityPower.x * mul, 0f);
                }

                targetRotation *= Quaternion.FromToRotation(tailLookDirections[lookDirectionFixIndex], ExtraToDirection);

                if (AnimateCorrections)
                    targetRotation *= animatedCorrections[fixDirForw];
                else
                    targetRotation *= lookBackOffsets[fixDirForw];
            }
            else
            {
                targetRotation = Quaternion.identity;

                bool rotationCollision = false;
                if (UseCollision) if (collisionFlags[fixDirForw] > 0f) if (collisionOffsets[fixDirForw] != Vector3.zero)
                        {
                            #region Experiments

                            //Quaternion target = Quaternion.LookRotation(collisionOffsets[fixDirForw], previousTailPoint.TransformDirection(AxisLookBack));
                            ////Quaternion target = Quaternion.LookRotation(collisionOffsets[fixDirForw], previousTailPoint.TransformDirection(AxisLookBack)) * Quaternion.FromToRotation(tailLookDirections[lookDirectionFixIndex], ExtraToDirection);
                            //targetRotation *= Quaternion.Slerp(Quaternion.identity, target, collisionFlags[fixDirForw]);

                            //Quaternion target = Quaternion.LookRotation(collisionOffsets[fixDirForw], previousTailPoint.TransformDirection(AxisLookBack));
                            //targetRotation *= Quaternion.Slerp(Quaternion.identity, target, collisionFlags[fixDirForw]);

                            //Vector3 tailDirection = (startLookPos - currentPos).normalized;
                            //Vector3 smoothedDirection = Vector3.Slerp(tailDirection, (tailDirection + collisionOffsets[fixDirForw]).normalized, collisionFlags[fixDirForw]);
                            //targetRotation = Quaternion.LookRotation(smoothedDirection, previousTailPoint.TransformDirection(AxisLookBack * Mathf.Sign(FVectorMethods.VectorSum(AxisCorrection))));

                            //Vector3 smoothedDirection = Vector3.Slerp(tailDirection, (tailDirection + collisionOffsets[fixDirForw]).normalized, collisionFlags[fixDirForw]);

                            //Vector3 upwards = previousTailPoint.TransformDirection(AxisLookBack * Mathf.Sign(FVectorMethods.VectorSum(AxisCorrection)));
                            //Vector3 tailDirection = (startLookPos - currentPos).normalized;
                            //targetRotation = Quaternion.LookRotation(tailDirection, upwards);
                            //targetRotation *= Quaternion.Slerp( Quaternion.identity, Quaternion.LookRotation(collisionOffsets[currentTailPoint.index], upwards), collisionFlags[fixDirForw]);

                            #endregion Experiments

                            Vector3 tailDirection = (startLookPos - currentPos).normalized;
                            Vector3 smoothedDirection = Vector3.Slerp(tailDirection, (tailDirection + collisionOffsets[currentTailPoint.index]).normalized, collisionFlags[fixDirForw]);
                            targetRotation = Quaternion.LookRotation(smoothedDirection, previousTailPoint.TransformDirection(AxisLookBack));
                            rotationCollision = true;
                        }

                if (!rotationCollision)
                    targetRotation = Quaternion.LookRotation(startLookPos - currentPos, previousTailPoint.TransformDirection(AxisLookBack * Mathf.Sign(FVectorMethods.VectorSum(AxisCorrection))));

                if (GravityPower != Vector2.zero)
                {
                    float mul = 10 / (fixDirForw * 2.5f + 1);
                    targetRotation *= Quaternion.Euler(GravityPower.y * mul, GravityPower.x * mul, 0f);
                }

                if (ExtraCorrectionOptions)
                    targetRotation *= Quaternion.FromToRotation(ExtraFromDirection, ExtraToDirection);
            }

            return targetRotation;
        }

        /// <summary>
        /// 객체가 파손되었을 때 연결이 끊어진 변환을 모두 제거하십시오.
        /// </summary>
        protected virtual void OnDestroy()
        {
        }

        // V.1.1.1
        /// <summary>
        /// 꼬리 모양 축 자동 보정
        /// </summary>
        protected void ApplyAutoCorrection()
        {
            ExtraCorrectionOptions = true;
            AxisCorrection = tailLookDirections[0];
            ExtraFromDirection = tailLookDirections[0];
        }

        /// <summary>
        /// 단순하지만 효과적이며 프레임에서 마지막으로 실행될 구성 요소를 푸시합니다.
        /// </summary>
        public void QueueComponentToLastUpdate()
        {
            enabled = false;
            enabled = true;
        }

        /// <summary>
        /// 속성이 변경 될 때마다 파생 클래스의 내용을 새로 고치기 위해 예약 된 메서드입니다.
        /// </summary>
        public virtual void OnValidate()
        {
        }

        // V1.2.0
        /// <summary>
        /// 꼬리뼈를 애니메이션화하는 도우미 클래스
        /// </summary>
        protected class FTail_Point
        {
            public int index = -1;
            public Vector3 Position = Vector3.zero;
            public Quaternion Rotation = Quaternion.identity;

            public Vector3 InitialPosition = Vector3.zero;
            public Quaternion InitialRotation = Quaternion.identity;

            public Vector3 TransformDirection(Vector3 dir)
            {
                return Rotation * dir;
            }
        }

        #region V1.2.6 Colliders Support

        /// <summary>
        /// 제공된 설정으로 꼬리에 콜리 더 생성하기
        /// </summary>
        private void AddColliders()
        {
            collisionOffsets = new List<Vector3>();
            collisionFlags = new List<float>();
            collisionContacts = new List<Collision>();

            collisionOffsets.Add(Vector3.zero);
            collisionFlags.Add(0f);
            collisionContacts.Add(null);

            for (int i = 1; i < TailTransforms.Count; i++)
            {
                collisionOffsets.Add(Vector3.zero);
                collisionContacts.Add(null);
                collisionFlags.Add(0f);
                if (CollidersSameLayer) TailTransforms[i].gameObject.layer = gameObject.layer; else TailTransforms[i].gameObject.layer = CollidersLayer;
            }

            if (CollidersType == FTailColliders.Boxes)
            {
                for (int i = 1; i < TailTransforms.Count; i++)
                {
                    BoxCollider b = TailTransforms[i].gameObject.AddComponent<BoxCollider>();
                    FTail_CollisionHelper tcol = TailTransforms[i].gameObject.AddComponent<FTail_CollisionHelper>().Init(CollidersAddRigidbody);
                    tcol.index = i;
                    tcol.ParentTail = this;
                    b.size = GetColliderBoxSizeFor(TailTransforms, i);
                }
            }
            else
            {
                for (int i = 1; i < TailTransforms.Count; i++)
                {
                    SphereCollider b = TailTransforms[i].gameObject.AddComponent<SphereCollider>();
                    FTail_CollisionHelper tcol = TailTransforms[i].gameObject.AddComponent<FTail_CollisionHelper>().Init(CollidersAddRigidbody);
                    tcol.index = i;
                    tcol.ParentTail = this;
                    b.radius = GetColliderSphereRadiusFor(TailTransforms, i);
                }
            }
        }

        //V1.2.6
        /// <summary>
        /// 꼬리 부분에 의해 제공된 Collision 데이터로 꼬리에 충돌 자 생성
        /// </summary>
        internal void CollisionDetection(int index, Collision collision)
        {
            collisionContacts[index] = collision;
        }

        /// <summary>
        /// 충돌 종료
        /// </summary>
        internal void ExitCollision(int index)
        {
            collisionContacts[index] = null;
        }

        public bool CollisionLookBack = true;

        /// <summary>
        /// 업데이트 방법을 사용할 때 저장된 충돌 접점을 적절한 순간에 사용하십시오.
        /// </summary>
        protected void UseCollisionContact(int index)
        {
            if (collisionContacts[index] == null) return;

            Collision collision = collisionContacts[index];
            Vector3 desiredDirection;

            desiredDirection = Vector3.Reflect((proceduralPoints[index - 1].Position - proceduralPoints[index].Position).normalized, collision.contacts[0].normal);

            #region Experiments

            //Quaternion segmentForwQ = Quaternion.LookRotation(proceduralPoints[index - 1].Position - proceduralPoints[index].Position);// * Quaternion.FromToRotation(tailLookDirections[index - 1], ExtraToDirection);
            //desiredDirection = Vector3.Reflect(segmentForwQ.eulerAngles.normalized, collision.contacts[0].normal);

            //Quaternion segmentForwQ = Quaternion.LookRotation(proceduralPoints[index].Position - transform.position);// * Quaternion.FromToRotation(tailLookDirections[index - 1], ExtraToDirection);
            //Quaternion segmentForwQ = proceduralPoints[index].Rotation * Quaternion.FromToRotation(tailLookDirections[index - 1], ExtraToDirection);

            //desiredDirection = Vector3.ProjectOnPlane(segmentForward, collision.contacts[0].normal);
            //desiredDirection = Vector3.Lerp(Vector3.Reflect(segmentForward, collision.contacts[0].normal), desiredDirection, collisionFlagsSlow[index]);

            //Vector3 segmentForward = segmentForwQ.eulerAngles.normalized;

            //desiredDirection = Vector3.Project(segmentForward, collision.contacts[0].normal);
            //Plane collisionPlane = new Plane(collision.contacts[0].normal, collision.contacts[0].point);

            //Vector3 norm = collision.contacts[0].normal;
            //Vector3 dir = segmentForward;
            //Vector3.OrthoNormalize(ref norm, ref dir);

            // desiredDirection = Vector3.ProjectOnPlane((TailTransforms[index].position - TailTransforms[index - 1].position).normalized, collision.contacts[0].normal);
            // Quaternion startDiffQuat = Quaternion.Inverse(proceduralPoints[index].InitialRotation * Quaternion.Inverse( TailTransforms[index].localRotation));

            //if (CollisionLookBack)
            //{
            //    Vector3 backCompensation = segmentForward + (Quaternion.Inverse(TailTransforms[index - 1].localRotation) * proceduralPoints[index - 1].InitialRotation) * Vector3.forward;
            //    desiredDirection -= backCompensation;

            //Quaternion backCompensation = Quaternion.Inverse(segmentForwQ) * (Quaternion.Inverse(TailTransforms[index - 1].localRotation) * proceduralPoints[index - 1].InitialRotation);
            //desiredDirection -= backCompensation * Vector3.forward;
            //}

            //desiredDirection = (transform.rotation * firstBoneInitialRotationQ ) * desiredDirection;
            //desiredDirection = (transform.rotation * firstBoneInitialRotationQ) * desiredDirection;

            #endregion Experiments

            //Debug.DrawRay(proceduralPoints[index].Position, desiredDirection, Color.yellow);

            collisionOffsets[index] = desiredDirection; //(Quaternion.Inverse(transform.rotation) * firstBoneInitialRotationQ) * desiredDirection;
            collisionFlags[index] = Mathf.Min(1f, collisionFlags[index] + Time.deltaTime * 8);
        }

        /// <summary>
        /// 올바른 순간에 저장된 충돌 접점을 사용하여 꼬리에 충돌자를 자동으로 계산합니다.
        /// </summary>
        protected Vector3 GetColliderBoxSizeFor(List<Transform> transforms, int i)
        {
            float refDistance = 1f;
            if (transforms.Count > 1) refDistance = Vector3.Distance(transforms[1].position, transforms[0].position);

            float singleScale = Mathf.Lerp(refDistance, Vector3.Distance(transforms[i - 1].position, transforms[i].position) * 0.5f, DifferenceScaleFactor);
            float step = 1f / (float)(transforms.Count - 1);

            Vector3 newScale = Vector3.one * singleScale * CollidersScaleMul * CollidersScale.Evaluate(step * (float)i);
            newScale.x *= BoxesDimensionsMul.x;
            newScale.y *= BoxesDimensionsMul.y;
            newScale.z *= BoxesDimensionsMul.z;

            return newScale;
        }

        /// <summary>
        /// 꼬리에 colliders에 대한 자동 스케일 계산
        /// </summary>
        protected float GetColliderSphereRadiusFor(List<Transform> transforms, int i)
        {
            float refDistance = 1f;
            if (transforms.Count > 1) refDistance = Vector3.Distance(transforms[1].position, transforms[0].position);

            float singleScale = Mathf.Lerp(refDistance, Vector3.Distance(transforms[i - 1].position, transforms[i].position) * 0.5f, DifferenceScaleFactor);

            float step = 1f / (float)(transforms.Count - 1);

            return 0.5f * singleScale * CollidersScaleMul * CollidersScale.Evaluate(step * (float)i);
        }

        #endregion V1.2.6 Colliders Support

        // V1.2.0
#if UNITY_EDITOR

        // 거지를 원하지 않으면 false로 설정하십시오.
        public static bool drawMainGizmo = true;

        public bool drawGizmos = false;

        /// <summary>
        /// 체인을 아직 정의하지 않은 경우 편집기 모드에서 변형 목록 가져 오기
        /// </summary>
        protected List<Transform> GetEditorGizmoTailList()
        {
            editorGizmoTailList = new List<Transform>();

            if (TailTransforms != null && TailTransforms.Count > 1)
            {
                editorGizmoTailList = TailTransforms;
            }
            else
            {
                Transform lastParent = transform;
                bool boneDefined = true;

                if (TailTransforms == null || TailTransforms.Count == 0)
                {
                    boneDefined = false;
                    lastParent = transform;
                }
                else lastParent = TailTransforms[0];

                Transform rootTransform = lastParent;

                for (int i = editorGizmoTailList.Count; i < 100; i++)
                {
                    if (boneDefined)
                        if (lastParent == rootTransform)
                        {
                            if (lastParent == null) break;
                            if (lastParent.childCount == 0) break;
                            lastParent = lastParent.GetChild(0);
                            continue;
                        }

                    editorGizmoTailList.Add(lastParent);

                    if (lastParent.childCount > 0) lastParent = lastParent.GetChild(0); else break;
                }
            }

            return editorGizmoTailList;
        }

        protected virtual void OnDrawGizmos()
        {
            // V1.2.6
            if (!Application.isPlaying)
                if (UseCollision)
                {
                    GetEditorGizmoTailList();

                    Color preCol = Gizmos.color;
                    Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.7f);

                    switch (CollidersType)
                    {
                        case FTailColliders.Boxes:

                            for (int i = 1; i < editorGizmoTailList.Count; i++)
                            {
                                if (editorGizmoTailList[i] == null) continue;
                                Gizmos.matrix = Matrix4x4.TRS(editorGizmoTailList[i].position, editorGizmoTailList[i].rotation, editorGizmoTailList[i].lossyScale);
                                Gizmos.DrawWireCube(Vector3.zero, GetColliderBoxSizeFor(editorGizmoTailList, i));
                            }

                            Gizmos.matrix = Matrix4x4.identity;

                            break;

                        case FTailColliders.Spheres:
                            for (int i = 1; i < editorGizmoTailList.Count; i++)
                            {
                                if (editorGizmoTailList[i] == null) continue;
                                Gizmos.matrix = Matrix4x4.TRS(editorGizmoTailList[i].position, editorGizmoTailList[i].rotation, editorGizmoTailList[i].lossyScale);
                                Gizmos.DrawWireSphere(Vector3.zero, GetColliderSphereRadiusFor(editorGizmoTailList, i));
                            }

                            break;
                    }

                    Gizmos.color = preCol;
                }

            if (!drawMainGizmo) return;

            Gizmos.DrawIcon(transform.position, "FIMSpace/FTail/SPR_TailAnimatorGizmoIcon.png", true);
        }

#endif
    }
}