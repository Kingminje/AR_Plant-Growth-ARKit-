using FIMSpace.Basics;
using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FTail
{
    [CustomEditor(typeof(FTail_Animator))]
    [CanEditMultipleObjects]
    /// <summary>
    /// FM: Editor class component to enchance controll over component from inspector window
    /// </summary>
    public class FTailAnimator_Editor : FTailAnimator_Editor_Base
    {
        protected bool drawWavingParams = false;

        protected SerializedProperty sp_upClock;
        protected SerializedProperty sp_discTransf;

        protected SerializedProperty sp_useWav;
        protected SerializedProperty sp_cosAd;
        protected SerializedProperty sp_wavSp;
        protected SerializedProperty sp_wavRa;
        protected SerializedProperty sp_wavAx;
        protected SerializedProperty sp_tailRotOff;

        protected override void OnEnable()
        {
            base.OnEnable();

            sp_upClock = serializedObject.FindProperty("UpdateClock");
            sp_discTransf = serializedObject.FindProperty("DisconnectTransforms");

            sp_useWav = serializedObject.FindProperty("UseWaving");
            sp_cosAd = serializedObject.FindProperty("CosinusAdd");
            sp_wavSp = serializedObject.FindProperty("WavingSpeed");
            sp_wavRa = serializedObject.FindProperty("WavingRange");
            sp_wavAx = serializedObject.FindProperty("WavingAxis");
            sp_tailRotOff = serializedObject.FindProperty("TailRotationOffset");
        }

        protected override void DrawingStack(FTail_AnimatorBase tail)
        {
            if (drawDefaultInspector)
            {
                GUILayout.Space(5f);
                DrawDefaultInspector();
            }
            else
            {
                undoManager.CheckUndo();
                serializedObject.Update();

                GUILayout.Space(3f);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                DrawTailList(tail);
                DrawSpeedSliders(tail);

                DrawWavingOptions((FTail_Animator)tail);

                DrawTuningParameters(tail);

                DrawPhysicalOptionsTab(tail);

                EditorGUILayout.EndVertical();

                if (drawGizmoSwitcher) DrawBottomTailBreakLine();

                if (GUI.changed) tail.OnValidate();

                undoManager.CheckDirty();
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override void DrawSpeedSliders(FTail_AnimatorBase tail)
        {
            GUILayout.BeginVertical(FEditor_Styles.LNavy);
            EditorGUILayout.HelpBox("Elasticity Behaviour Parameters", MessageType.None);

            EditorGUILayout.Slider(sp_posSpeeds, 0f, 60f);
            EditorGUILayout.Slider(sp_rotSpeeds, 0f, 60f);

            GUILayout.EndVertical();

            // V1.2
            FTail_Animator tailSimple = tail as FTail_Animator;
            FTail_AnimatorBlending tailBlending = tail as FTail_AnimatorBlending;
            if (!tailBlending) if (tailSimple != null)
                {
                    EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);

                    GUILayout.BeginHorizontal(FEditor_Styles.LBlueBackground);
                    EditorGUILayout.HelpBox("Use late update order for animated objects", MessageType.Info);
                    GUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(sp_upClock, new GUIContent("Update Order"));
                    EditorGUIUtility.labelWidth = 147;
                    EditorGUILayout.PropertyField(sp_discTransf);
                    EditorGUILayout.PropertyField(sp_queue);
                    EditorGUIUtility.labelWidth = 0;

                    EditorGUILayout.EndVertical();
                }

            GUILayout.Space(1f);

            EditorGUIUtility.labelWidth = 0;
        }

        protected virtual void DrawWavingOptions(FTail_Animator tail)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginVertical(FEditor_Styles.LBlueBackground);

            GUILayout.BeginHorizontal(FEditor_Styles.LBlueBackground);
            drawWavingParams = EditorGUILayout.Foldout(drawWavingParams, "Waving Options", true);

            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 90;

            if (!tail.RootToParent)
                EditorGUILayout.PropertyField(sp_useWav);

            GUILayout.EndHorizontal();

            if (!tail.RootToParent)
                if (drawWavingParams)
                {
                    EditorGUIUtility.labelWidth = 165;

                    GUILayout.Space(5f);
                    EditorGUILayout.PropertyField(sp_cosAd);
                    GUILayout.Space(5f);

                    EditorGUILayout.PropertyField(sp_wavSp);
                    EditorGUILayout.PropertyField(sp_wavRa);
                    GUILayout.Space(5f);

                    EditorGUILayout.PropertyField(sp_wavAx);

                    EditorGUILayout.PropertyField(sp_tailRotOff);
                }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = 0;
        }
    }
}