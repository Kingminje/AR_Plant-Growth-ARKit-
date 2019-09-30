using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Class which is animating Tail Animator behaviour in Sprites 2D space
    /// </summary>
    public class FTail_Animator2D : FTail_AnimatorUI
    {
        protected override void Init()
        {
            UseAutoCorrectLookAxis = false;
            base.Init();
        }

        /// <summary>
        /// Setting correction options to be setted for 2D sprites space behaviour
        /// </summary>
        protected override void Reset()
        {
            AxisCorrection = -Vector3.right;
            AxisLookBack = Vector3.up;

            ExtraCorrectionOptions = true;
            ExtraFromDirection = Vector3.forward;
            ExtraToDirection = Vector3.right;

            WavingAxis = Vector3.forward;
        }

        protected override Quaternion CalculateTargetRotation(Vector3 startLookPos, Vector3 currentPos, FTail_Point previousTailPoint = null, FTail_Point currentTailPoint = null, int lookDirectionFixIndex = 0)
        {
            if (Lock2D) return FLogicMethods.TopDownAnglePosition2D(startLookPos, currentPos);
            else
                return base.CalculateTargetRotation(startLookPos, currentPos, previousTailPoint, currentTailPoint, lookDirectionFixIndex);
        }
    }
}