using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Class which is animating Tail Animator behaviour in UI 2D space
    /// </summary>
    public class FTail_AnimatorUI : FTail_Animator
    {
        public bool Lock2D = true;

        /// <summary>
        /// Setting extra correction variables for UI
        /// </summary>
        protected override void Reset()
        {
            UseAutoCorrectLookAxis = false;
            AxisCorrection = Vector3.right;
            AxisLookBack = Vector3.up;

            ExtraCorrectionOptions = false;
            ExtraFromDirection = Vector3.forward;
            ExtraToDirection = Vector3.right;

            WavingAxis = Vector3.forward;
        }

        /// <summary>
        /// For UI we must calculate it differently
        /// </summary>
        protected override Quaternion CalculateTargetRotation(Vector3 startLookPos, Vector3 currentPos, FTail_Point previousTailPoint = null, FTail_Point currentTailPoint = null, int lookDirectionFixIndex = 0)
        {
            if (Lock2D) return FLogicMethods.TopDownAnglePosition2D(currentPos, startLookPos);
            else
                return base.CalculateTargetRotation(startLookPos, currentPos, previousTailPoint, currentTailPoint, lookDirectionFixIndex);
        }
    }
}