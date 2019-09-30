using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FTail
{
    [CustomEditor(typeof(FTail_AnimatorBase))]
    [CanEditMultipleObjects]
    /// <summary>
    /// FM: Editor class component to enchance controll over component from inspector window
    /// </summary>
    public class FTailAnimator_Editor_Base : Editor
    {
        protected static bool drawGizmoSwitcher = true;

        protected static bool drawDefaultInspector = false;
        protected static bool drawTailBones = false;
        protected static bool drawTuningParams = false;
        protected static bool drawPhysicalParams = false;
        protected static bool drawFromTo = false;
        protected static bool drawAutoFixOption = true;
        protected Texture2D breakLineTail = null;

        protected HOEditorUndoManager undoManager;

        protected SerializedProperty sp_posSpeeds;
        protected SerializedProperty sp_rotSpeeds;
        protected SerializedProperty sp_useAutoCorr;

        protected SerializedProperty sp_addRefs;
        protected SerializedProperty sp_stretch;
        protected SerializedProperty sp_fullCorrect;
        protected SerializedProperty sp_rollBones;
        protected SerializedProperty sp_axisCorr;
        protected SerializedProperty sp_axisBack;
        protected SerializedProperty sp_extraCorr;
        protected SerializedProperty sp_fromdir;
        protected SerializedProperty sp_todir;
        protected SerializedProperty sp_animate;
        protected SerializedProperty sp_refr;
        protected SerializedProperty sp_smoothdelta;
        protected SerializedProperty sp_queue;
        protected SerializedProperty sp_rootp;
        protected SerializedProperty sp_autoone;
        protected SerializedProperty sp_useCollision;
        protected SerializedProperty sp_gravity;
        protected SerializedProperty sp_colType;
        protected SerializedProperty sp_colScale;
        protected SerializedProperty sp_colScaleMul;
        protected SerializedProperty sp_colBoxDim;
        protected SerializedProperty sp_colDiffFact;
        protected SerializedProperty sp_colWithOther;
        protected SerializedProperty sp_colIgnored;
        protected SerializedProperty sp_colSameLayer;
        protected SerializedProperty sp_colCustomLayer;
        protected SerializedProperty sp_colAddRigs;



        protected virtual void OnEnable()
        {
            undoManager = new HOEditorUndoManager(target as FTail_AnimatorBase, "Undo Manager");

            sp_posSpeeds = serializedObject.FindProperty("PositionSpeed");
            sp_rotSpeeds = serializedObject.FindProperty("RotationSpeed");
            sp_useAutoCorr = serializedObject.FindProperty("UseAutoCorrectLookAxis");

            sp_addRefs = serializedObject.FindProperty("AddTailReferences");
            sp_stretch = serializedObject.FindProperty("StretchMultiplier");
            sp_fullCorrect = serializedObject.FindProperty("FullCorrection");
            sp_rollBones = serializedObject.FindProperty("RolledBones");
            sp_axisCorr = serializedObject.FindProperty("AxisCorrection");
            sp_axisBack = serializedObject.FindProperty("AxisLookBack");
            sp_extraCorr = serializedObject.FindProperty("ExtraCorrectionOptions");
            sp_fromdir = serializedObject.FindProperty("ExtraFromDirection");
            sp_todir = serializedObject.FindProperty("ExtraToDirection");
            sp_animate = serializedObject.FindProperty("AnimateCorrections");
            sp_refr = serializedObject.FindProperty("RefreshHelpers");
            sp_smoothdelta = serializedObject.FindProperty("SmoothDeltaTime");
            sp_queue = serializedObject.FindProperty("QueueToLastUpdate");
            sp_rootp = serializedObject.FindProperty("RootToParent");
            sp_autoone = serializedObject.FindProperty("AutoGetWithOne");
            sp_useCollision = serializedObject.FindProperty("UseCollision");
            sp_gravity = serializedObject.FindProperty("GravityPower");
            sp_colType = serializedObject.FindProperty("CollidersType");
            sp_colScale = serializedObject.FindProperty("CollidersScale");
            sp_colScaleMul = serializedObject.FindProperty("CollidersScaleMul");
            sp_colBoxDim = serializedObject.FindProperty("BoxesDimensionsMul");
            sp_colDiffFact = serializedObject.FindProperty("DifferenceScaleFactor");
            sp_colWithOther = serializedObject.FindProperty("CollideWithOtherTails");

            sp_colIgnored = serializedObject.FindProperty("IgnoredColliders");
            sp_colSameLayer = serializedObject.FindProperty("CollidersSameLayer");
            sp_colCustomLayer = serializedObject.FindProperty("CollidersLayer");
            sp_colAddRigs = serializedObject.FindProperty("CollidersAddRigidbody");
        }

        public override void OnInspectorGUI()
        {
            // Update component from last changes
            serializedObject.Update();

            FTail_AnimatorBase tailComp = (FTail_AnimatorBase)target;

            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            EditorGUILayout.BeginHorizontal();
            drawDefaultInspector = GUILayout.Toggle(drawDefaultInspector, "Default inspector");

            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 80;
            drawGizmoSwitcher = GUILayout.Toggle(drawGizmoSwitcher, "View Gizmo Switch");

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            DrawingStack(tailComp);

            // Apply changed parameters variables
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawingStack(FTail_AnimatorBase tail)
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

                DrawTuningParameters(tail);

                EditorGUILayout.EndVertical();

                if (drawGizmoSwitcher) DrawBottomTailBreakLine();

                if (GUI.changed) tail.OnValidate();

                undoManager.CheckDirty();
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected void DrawTailList(FTail_AnimatorBase tail)
        {
            GUILayout.BeginHorizontal(FEditor_Styles.BlueBackground);
            // Long text as tooltip to save space in inspectors
            EditorGUILayout.LabelField(new GUIContent("ENTER HERE FOR INFO TOOLTIP (no in playmode)", "Put under 'Tail Bones' first bone of tail - component will use children transform to get rest tail bones, or left empty, then tail structure will be created starting from this transform, also you can put here for example 3 bones so only this transforms will be animated"));
            GUILayout.EndHorizontal();

            // Extra info for Tail bones array viewer
            string extraInfo = "";
            bool red = false;


            if (tail.TailTransforms == null) tail.TailTransforms = new System.Collections.Generic.List<Transform>();

            if (tail.TailTransforms.Count > 0)
            {
                if (tail.TailTransforms[0] == null)
                {
                    extraInfo = " - NULL BONE!";
                    red = true;
                }
                else
                if (tail.TailTransforms.Count == 1)
                {
                    if (tail.AutoGetWithOne)
                    {
                        if (drawTailBones)
                            extraInfo = "  (1 - Auto Get)";
                        else
                            extraInfo = "  (1 - Auto Child Transforms)";
                    }
                    else
                    {
                        extraInfo = "  (Only one bone)";
                    }
                }
                else
                {
                    bool nullDetected = false;
                    for (int i = 1; i < tail.TailTransforms.Count; i++)
                    {
                        if (tail.TailTransforms[i] == null)
                        {
                            nullDetected = true;
                            break;
                        }
                    }

                    if (nullDetected)
                    {
                        extraInfo = "   (SOME NULLS!)";
                        red = true;
                    }
                }
            }
            else
            {
                if (drawTailBones)
                    extraInfo = "  (No Bone - Auto Get)";
                else
                    extraInfo = "  (No Bone - Auto Get)";
            }

            if (extraInfo == "") extraInfo = " (" + tail.TailTransforms.Count + ")";


            if (red)
                GUILayout.BeginVertical(FEditor_Styles.RedBackground);
            else
                GUILayout.BeginVertical(FEditor_Styles.GrayBackground);


            GUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;


            drawTailBones = EditorGUILayout.Foldout(drawTailBones, "Tail Bones" + extraInfo, true);

            if (tail.TailTransforms.Count == 1)
                EditorGUILayout.PropertyField(sp_autoone, new GUIContent("", "When you want to use auto get when you assigning one bone inside inspector window (Not working with waving - then you have to rotate parent transform in your own way to get same effect)"), new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(18) });

            if (drawTailBones)
            {
                DrawAddTailButtons(tail);

                GUILayout.EndHorizontal();

                EditorGUI.indentLevel++;

                for (int i = 0; i < tail.TailTransforms.Count; i++)
                {
                    tail.TailTransforms[i] = (Transform)EditorGUILayout.ObjectField("Tail Bone [" + i + "]", tail.TailTransforms[i], typeof(Transform), true);
                }

                EditorGUI.indentLevel--;
            }
            else
            {
                if (tail.TailTransforms.Count == 0) DrawAddTailButtons(tail);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(FEditor_Styles.GrayBackground);

            EditorGUIUtility.labelWidth = 155;
            EditorGUILayout.PropertyField(sp_addRefs);
            EditorGUILayout.PropertyField(sp_rootp);
            EditorGUIUtility.labelWidth = 0;
            GUILayout.EndVertical();

            GUILayout.Space(1f);

            EditorGUI.indentLevel--;

            EditorGUIUtility.labelWidth = 0;
        }

        protected void DrawAddTailButtons(FTail_AnimatorBase tail)
        {
            // V1.2.2
            if (GUILayout.Button("Auto", new GUILayoutOption[2] { GUILayout.MaxWidth(48), GUILayout.MaxHeight(14) }))
            {
                if (tail.TailTransforms.Count <= 1)
                {
                    tail.AutoGetTailTransforms(true);
                }
                else
                {
                    if (tail.TailTransforms[0] == null)
                        tail.AutoGetTailTransforms(true);
                    else
                    {
                        bool isnull = false;

                        for (int i = 0; i < tail.TailTransforms.Count; i++)
                            if ( tail.TailTransforms[i] == null)
                            {
                                isnull = true;
                                break;
                            }

                        if (isnull)
                        {
                            for (int i = 1; i < tail.TailTransforms.Count; i++)
                            {
                                if (tail.TailTransforms[i - 1].childCount == 0) break;
                                tail.TailTransforms[i] = tail.TailTransforms[i - 1].GetChild(0);
                            }
                        }
                        else
                        {
                            Transform first = tail.TailTransforms[0];
                            tail.TailTransforms.Clear();
                            tail.TailTransforms.Add(first);
                            tail.AutoGetTailTransforms(true);
                        }
                    }
                }
            }

            if (GUILayout.Button("+", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
            {
                if (tail.TailTransforms.Count == 0) drawTailBones = true;

                tail.TailTransforms.Add(null);
            }

            if (GUILayout.Button("-", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
            {
                if (tail.TailTransforms.Count > 0) tail.TailTransforms.RemoveAt(tail.TailTransforms.Count - 1);
            }
        }

        protected virtual void DrawSpeedSliders(FTail_AnimatorBase tail)
        {
            GUILayout.BeginHorizontal(FEditor_Styles.LBlueBackground);
            EditorGUILayout.HelpBox("Elasticity Behaviour Parameters", MessageType.None);
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);

            EditorGUILayout.Slider(sp_posSpeeds, 0f, 60f);
            EditorGUILayout.Slider(sp_rotSpeeds, 0f, 60f);

            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(sp_queue);

            GUILayout.Space(1f);

            EditorGUIUtility.labelWidth = 0;
        }

        protected virtual void DrawTuningParameters(FTail_AnimatorBase tail)
        {
            EditorGUIUtility.labelWidth = 130;

            EditorGUILayout.BeginVertical(FEditor_Styles.GreenBackground);

            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal(FEditor_Styles.GreenBackground);
            drawTuningParams = EditorGUILayout.Foldout(drawTuningParams, "Tuning Parameters", true);

            if (drawAutoFixOption)
            {
                GUILayout.FlexibleSpace();
                EditorGUIUtility.labelWidth = 80;
                EditorGUILayout.PropertyField(sp_useAutoCorr, new GUIContent("Automatic", ""));
            }

            GUILayout.EndHorizontal();

            if (drawTuningParams)
            {
                GUILayout.Space(8f);
                DrawTuningParametersGUI(tail);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = 0;
        }



        protected virtual void DrawTuningParametersGUI(FTail_AnimatorBase tail)
        {
            EditorGUIUtility.labelWidth = 140;
            EditorGUILayout.PropertyField(sp_stretch);
            EditorGUILayout.PropertyField(sp_refr);
            GUILayout.Space(3f);

            if (drawAutoFixOption)
            {
                EditorGUIUtility.labelWidth = 170;
                //EditorGUILayout.LabelField(new GUIContent("Full Correction - previously 'Auto go through all bones'"));
                EditorGUILayout.PropertyField(sp_fullCorrect, new GUIContent(new GUIContent("Full Correction", "If automatic orientation fix should be calculated for each bones separately (previously named 'Auto go through all bones')")));
            }

            if (tail.FullCorrection)
            {
                if (!tail.RolledBones)
                    EditorGUILayout.PropertyField(sp_animate, new GUIContent("Animate Corrections", "When you want corrections to match animation in realtime"));

                EditorGUILayout.PropertyField(sp_rollBones, new GUIContent("Rolled Bones?", "Use this option when your model is rolling strangely when waving"));

                if (tail.UpdateClock != Basics.EFUpdateClock.FixedUpdate)
                {
                    EditorGUILayout.PropertyField(sp_smoothdelta);
                }
            }

            if (!tail.RolledBones)
            {
                EditorGUIUtility.labelWidth = 140;
                EditorGUILayout.PropertyField(sp_axisCorr, new GUIContent("Axis Correction", "[Advanced] Bones wrong rotations axis corrector"));
                EditorGUILayout.PropertyField(sp_axisBack, new GUIContent("Axis LookBack", "[Advanced] Look rotation transform direction reference"));
                GUILayout.Space(8f);

                EditorGUI.indentLevel++;

                GUILayout.BeginHorizontal(FEditor_Styles.BlueBackground);
                drawFromTo = EditorGUILayout.Foldout(drawFromTo, new GUIContent("More Advanced Parameters", "Click on toggle to enable using this options"), true);

                GUILayout.FlexibleSpace();
                bool preExtr = tail.ExtraCorrectionOptions;
                EditorGUILayout.PropertyField(sp_extraCorr, new GUIContent(""), GUILayout.MaxWidth(45f));
                if (preExtr != tail.ExtraCorrectionOptions) if (tail.ExtraCorrectionOptions) drawFromTo = true; else drawFromTo = false;

                GUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(FEditor_Styles.LBlueBackground);

                if (drawFromTo)
                {
                    EditorGUIUtility.labelWidth = 117;
                    EditorGUILayout.PropertyField(sp_fromdir, new GUIContent("From Axis", "From rotation transforming. Extra repair parameters for rotating tail in unusual axes space."));
                    EditorGUILayout.PropertyField(sp_todir, new GUIContent("To Axis", ""));
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }

            EditorGUIUtility.labelWidth = 0;
        }

        protected virtual void DrawPhysicalOptionsTab(FTail_AnimatorBase tail)
        {
            EditorGUIUtility.labelWidth = 130;

            EditorGUILayout.BeginVertical(FEditor_Styles.LBlueBackground);

            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal(FEditor_Styles.LBlueBackground);
            drawPhysicalParams = EditorGUILayout.Foldout(drawPhysicalParams, "Physical (Experimental)", true);
            GUILayout.EndHorizontal();

            if (drawPhysicalParams)
            {
                GUILayout.Space(8f);
                DrawPhysicalParametersGUI(tail);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = 0;
        }


        protected virtual void DrawPhysicalParametersGUI(FTail_AnimatorBase tail)
        {
            EditorGUIUtility.labelWidth = 140;

            if (tail.UseCollision) EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);

            EditorGUILayout.PropertyField(sp_useCollision);

            if (!Application.isPlaying)
            {
                if (tail.UseCollision)
                {
                    EditorGUILayout.HelpBox("Collision support is experimental and not working fully correct yet. When entering playmode colliders will be generated as in editor preview", MessageType.Info);

                    EditorGUI.indentLevel++;
                    GUILayout.Space(2f);
                    EditorGUILayout.PropertyField(sp_colType);
                    EditorGUIUtility.labelWidth = 190;
                    EditorGUILayout.PropertyField(sp_colWithOther);
                    EditorGUIUtility.labelWidth = 140;
                    EditorGUILayout.PropertyField(sp_colScaleMul, new GUIContent("Scale Multiplier"));
                    EditorGUILayout.PropertyField(sp_colScale, new GUIContent("Scale Curve"));
                    if (tail.CollidersType == FTail_AnimatorBase.FTailColliders.Boxes) EditorGUILayout.PropertyField(sp_colBoxDim, new GUIContent("Dimensions Mul."));
                    EditorGUILayout.PropertyField(sp_colDiffFact, new GUIContent("Auto Curve"));
                    EditorGUILayout.PropertyField(sp_colAddRigs, new GUIContent("Add Rigidbodies", "If you add rigidbodies to each tail segment's collider, collision will work on everything but it will be less optimal, you don't have to add here rigidbodies but then you must have not kinematic rigidbodies on objects segments can collide"));
                    GUILayout.Space(2f);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                if (tail.UseCollision)
                {
                    if (!Application.isPlaying)
                        EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);

                    EditorGUI.indentLevel++;
                    GUILayout.Space(2f);
                    EditorGUIUtility.labelWidth = 190;
                    EditorGUILayout.PropertyField(sp_colWithOther);
                    EditorGUIUtility.labelWidth = 140;
                    GUILayout.Space(2f);
                    EditorGUI.indentLevel--;
                }
            }

            if (tail.UseCollision)
            {
                GUILayout.Space(2f);
                EditorGUI.indentLevel++;
                if (!Application.isPlaying)
                {
                    EditorGUIUtility.labelWidth = 180;
                    EditorGUILayout.PropertyField(sp_colSameLayer);

                    if (!tail.CollidersSameLayer)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUIUtility.labelWidth = 140;
                        EditorGUILayout.PropertyField(sp_colCustomLayer);
                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUIUtility.labelWidth = 0;
                EditorGUILayout.PropertyField(sp_colIgnored, true);
                EditorGUI.indentLevel--;
                GUILayout.Space(2f);

                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(5f);

            EditorGUIUtility.labelWidth = 140;
            EditorGUILayout.PropertyField(sp_gravity);

            EditorGUIUtility.labelWidth = 0;
        }



        protected void DrawBottomTailBreakLine()
        {
            if (breakLineTail == null) breakLineTail = Resources.Load("FTail_BreakLineTail", typeof(Texture2D)) as Texture2D;
            Rect rect = GUILayoutUtility.GetRect(128f, breakLineTail.height * 1f);
            GUILayout.BeginHorizontal();
            GUI.DrawTexture(rect, breakLineTail, ScaleMode.StretchToFill, true, 1f);
            FTail_AnimatorBase tail = (FTail_AnimatorBase)target;
            tail.drawGizmos = GUILayout.Toggle(tail.drawGizmos, new GUIContent("", "Toggle to switch drawing gizmos"));
            GUILayout.EndHorizontal();
        }
    }
}
