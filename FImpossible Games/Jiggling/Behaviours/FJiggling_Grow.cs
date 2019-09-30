using UnityEngine;

namespace FIMSpace.Jiggling
{
    /// <summary>
    /// FM: Class which is using FJiggle_Simple to animate tilting but also scalling transform up / down
    /// </summary>
    public class FJiggling_Grow : FJiggling_Simple
    {
        /// <summary> Multiplies deltaTime </summary>
        public float GrowShrinkSpeed = 1f;

        public float checkSize;

        protected float growProgress = 1f;
        protected bool shrinking = false;

        public void GowSizeCheck(float size)
        {
            checkSize = size;
        }

        public void ToggleGrowShrink()
        {
            if (shrinking) StartGrowing();
            else
                StartShrinking();
        }

        public virtual void StartGrowing()
        {
            shrinking = false;
            StartJiggle();
        }

        public virtual void StartShrinking()
        {
            shrinking = true;
            StartJiggle();
        }

        protected override void ReJiggle()
        {
            // Dont do anything
        }

        protected override void CalculateJiggle()
        {
            base.CalculateJiggle();

            float sign = 1f;
            if (shrinking) sign = -1f;

            growProgress = Mathf.Clamp(growProgress + Time.deltaTime * sign * GrowShrinkSpeed, checkSize, 1f);

            Transform t = TransformToAnimate;
            t.transform.localScale = t.transform.localScale * growProgress;

            if (shrinking)
            {
                easedPowerProgress = Mathf.Lerp(0.7f, 1f, growProgress);

                if (growProgress == 0) animationFinished = true;
            }
            else // When object is growing
            {
                easedPowerProgress = Mathf.Lerp(1f, 0.15f, growProgress);

                if (growProgress == 1) animationFinished = true;
            }

            if (animationFinished) OnAnimationFinish();
        }

        protected override void OnAnimationFinish()
        {
            if (!shrinking) if (growProgress == 1) ResetInitialPosRot();
        }

        protected void ResetInitialPosRot()
        {
            base.OnAnimationFinish();
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// FM: Editor class for Grow component to check animation from editor level (in playmode)
    /// </summary>
    [UnityEditor.CustomEditor(typeof(FJiggling_Grow))]
    public class FJiggling_GrowEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            FJiggling_Grow targetScript = (FJiggling_Grow)target;
            DrawDefaultInspector();

            GUILayout.Space(10f);

            if (!Application.isPlaying) GUI.color = FColorMethods.ChangeColorAlpha(GUI.color, 0.45f);
            if (GUILayout.Button("Animate Growing")) if (Application.isPlaying) targetScript.StartGrowing(); else Debug.Log("You must be in playmode to run this method!");
            if (GUILayout.Button("Animate Shrink")) if (Application.isPlaying) targetScript.StartShrinking(); else Debug.Log("You must be in playmode to run this method!");
        }
    }

#endif
}