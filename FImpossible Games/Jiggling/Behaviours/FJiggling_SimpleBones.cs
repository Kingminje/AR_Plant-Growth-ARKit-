using UnityEngine;

namespace FIMSpace.Jiggling
{
    /// <summary>
    /// FM: Animating transform's rotation and scale to make it look kinda like jelly
    /// </summary>
    public class FJiggling_SimpleBones : FJiggling_Simple
    {
        public bool NoRotationKeyframes = false;
        private Quaternion initialKeyRotation;

        protected override void Init()
        {
            if (initialized) return;

            base.Init();
            initialKeyRotation = TransformToAnimate.localRotation;
        }

        protected override void Update()
        {
            // Erasing all actions made in Update() 
        }

        protected virtual void LateUpdate()
        {
            if (NoRotationKeyframes) transform.localRotation = initialKeyRotation;

            // Every beginning of late update rotations are the same as in animation played by Animator component
            initRotation = transform.localRotation;

            // Doing update calculations in LateUpdate() to override Animator's work
            base.Update();
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// FM: Editor class for Jiggle Bones component to check animation from editor level (in playmode)
    /// </summary>
    [UnityEditor.CustomEditor(typeof(FJiggling_SimpleBones))]
    public class FJiggling_SimpleBonesEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            FJiggling_SimpleBones targetScript = (FJiggling_SimpleBones)target;
            DrawDefaultInspector();

            GUILayout.Space(10f);

            if (!Application.isPlaying) GUI.color = FColorMethods.ChangeColorAlpha(GUI.color, 0.45f);
            if (GUILayout.Button("Jiggle It")) if (Application.isPlaying) targetScript.StartJiggle(); else Debug.Log("You must be in playmode to run this method!");
        }
    }
#endif
}