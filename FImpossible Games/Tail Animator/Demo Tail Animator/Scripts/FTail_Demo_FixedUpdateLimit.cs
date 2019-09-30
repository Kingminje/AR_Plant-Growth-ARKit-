using FIMSpace.Basics;
using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Using FTail_MovementSinus calculations to animate tail with simple movement in fixed update
    /// We disconnecting cape in demo, because it's attached to skeleton and root transform is
    /// moving in FixedUpdate, this making stuff problematic
    /// </summary>
    public class FTail_Demo_FixedUpdateLimit : FTail_Animator
    {
        [Header("Limitation optional variables for X rotation axis")]
        public bool UseXLimitation = true;
        public float WorldXDontGoUnder = -65f;
        public float PushTime = 0.25f;
        public float PushPower = 15f;

        protected Vector3 TrueTailRotationOffset = Vector3.zero;
        protected float pushTimer = 0f;

        protected override void Reset()
        {
            DisconnectTransforms = true;
        }

        protected override void Init()
        {
            base.Init();

            TrueTailRotationOffset = TailRotationOffset;
            UpdateClock = EFUpdateClock.FixedUpdate;
        }

        internal override void Update()
        {
            if (UpdateClock == EFUpdateClock.FixedUpdate) return;
            CalculateOffsets();
        }

        internal override void FixedUpdate()
        {
            if (UpdateClock != EFUpdateClock.FixedUpdate) return;
            CalculateOffsets();
        }

        /// <summary>
        /// Adding sinus wave rotation with limiting option for first bone before other calculations
        /// </summary>
        public override void CalculateOffsets()
        {
            // Just calculating animation variables
            float delta;

            if (UpdateClock == EFUpdateClock.FixedUpdate)
                delta = Time.fixedDeltaTime;
            else
                delta = Time.deltaTime;

            if (UseWaving)
            {
                waveTime += delta * (2 * WavingSpeed);

                // It turned out to be problematic
                // When using clamp setting rotation was flipping other axes
                // So we push rotation in x axis (most common to this problem)
                if (UseXLimitation)
                {
                    Vector3 worldRotation = TailTransforms[0].rotation.eulerAngles;
                    float wrappedWorldAngle = LimitAngle360(worldRotation.x);

                    if (wrappedWorldAngle < WorldXDontGoUnder) pushTimer = PushTime;

                    if (pushTimer > 0f)
                    {
                        TrueTailRotationOffset = Vector3.Lerp(TrueTailRotationOffset, Vector3.zero, delta * PushPower);
                        pushTimer -= delta;
                    }
                    else
                    {
                        TrueTailRotationOffset = Vector3.Lerp(TrueTailRotationOffset, TailRotationOffset, delta * PushPower * 0.7f);
                    }
                }

                Vector3 rot = firstBoneInitialRotation + TrueTailRotationOffset;

                float sinVal = Mathf.Sin(waveTime) * (30f * WavingRange);
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

            if (preAutoCorrect != UseAutoCorrectLookAxis)
            {
                ApplyAutoCorrection();
                preAutoCorrect = UseAutoCorrectLookAxis;
            }

            proceduralPoints[0].Position = TailTransforms[0].position;

            // Just calculating animation variables
            float posDelta;
            float rotDelta;

            if (UpdateClock == EFUpdateClock.FixedUpdate)
            {
                posDelta = Time.fixedDeltaTime * PositionSpeed;
                rotDelta = Time.fixedDeltaTime * RotationSpeed;
            }
            else
            {
                posDelta = Time.deltaTime * PositionSpeed;
                rotDelta = Time.deltaTime * RotationSpeed;
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
                proceduralPoints[i].Position = Vector3.Lerp(currentTailPoint.Position, targetPosition, posDelta);

                Quaternion targetLookRotation = CalculateTargetRotation(startLookPosition, currentTailPoint.Position, previousTailPoint, currentTailPoint, i - 1);
                proceduralPoints[i].Rotation = Quaternion.Lerp(currentTailPoint.Rotation, targetLookRotation, rotDelta);
            }

            SetTailTransformsFromPoints();
        }

        /// <summary>
        /// Regulates angle values returned from rotation.eulerAngles to be same as in inspector
        /// </summary>
        private static float LimitAngle360(float angle)
        {
            angle %= 360;
            if (angle > 180) return angle - 360;
            return angle;
        }
    }
}
