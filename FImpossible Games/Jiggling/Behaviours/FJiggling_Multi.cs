using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FIMSpace.Jiggling
{
    /// <summary>
    /// FM: Animating multiple transforms rotation and scale to make it look kinda like jelly type animation
    /// </summary>
    public class FJiggling_Multi : FJiggling_Base
    {
        [HideInInspector]
        public List<FJiggling_Element> ToJiggle;

        [Header("Multi Variables")]
        [Tooltip("Calculating individual randomization variables for each element of chain, less optimal but can provide more interesting effects (it's cheap in cpu anyway)")]
        public bool SeparatedCalculations = true;

        public bool BonesNotAnimatedByAnimator = true;

        [HideInInspector]
        public bool ShowIndividualOptions = false;


        [Header("For more custom animations")]
        public Vector3 ScaleAxesMultiplier = Vector3.one;
        public Vector3 RotationAxesMultiplier = Vector3.one;


        /* When jiggling is executed second time during animating, this variables making effect like object is hitted again */
        [Space(5f)]
        [Tooltip("How big should be tilt impact when jiggling is triggered another time during animating")]
        public float ReJigglePower = 2.25f;
        protected float reJiggleProgress = 0f;
        protected float reJiggleValue = 0f;
        protected float reJiggleRandomOffset;


        protected override void Init()
        {
            if (initialized) return;

            for (int i = 0; i < ToJiggle.Count; i++)
            {
                if (ToJiggle[i].Transform == null)
                {
                    continue;
                }

                ToJiggle[i].InitPos = ToJiggle[i].Transform.localPosition;
                ToJiggle[i].InitRot = ToJiggle[i].Transform.localRotation;
                ToJiggle[i].InitScale = ToJiggle[i].Transform.localScale;
            }

            base.Init();
        }


        protected override void Update() { /* Don't do anything in Update() we will use LateUpdate() to support also animated models jiggling animation */ }


        /// <summary>
        /// Executing right methods for group of jiggle game objects
        /// </summary>
        private void LateUpdate()
        {
            CalculateJiggle();

            if (reJiggled)
            {
                if (reJiggleProgress > 0f)
                {
                    float targetValue = Mathf.Sin(time * 1.45f * trigParams[1].RandomTimeMul + reJiggleRandomOffset + trigParams[0].TimeOffset + trigParams[1].TimeOffset) * JiggleTiltValue * trigParams[0].Multiplier * reJiggleProgress * ReJigglePower;
                    reJiggleValue = Mathf.Lerp(reJiggleValue, targetValue, Time.deltaTime * 12f);
                    reJiggleProgress -= Time.deltaTime * JiggleDecelerate * 1.75f;
                }
                else reJiggleProgress = 0f;
            }

            if (BonesNotAnimatedByAnimator)
            {
                if (!SeparatedCalculations)
                    for (int i = 0; i < ToJiggle.Count; i++)
                    {
                        ToJiggle[i].Transform.localScale = ToJiggle[i].InitScale;
                        ToJiggle[i].Transform.localPosition = ToJiggle[i].InitPos;
                        ToJiggle[i].Transform.localRotation = ToJiggle[i].InitRot;
                        CalculateJigglingFor(i);
                    }
                else
                    for (int i = 0; i < ToJiggle.Count; i++)
                    {
                        ToJiggle[i].Transform.localScale = ToJiggle[i].InitScale;
                        ToJiggle[i].Transform.localPosition = ToJiggle[i].InitPos;
                        ToJiggle[i].Transform.localRotation = ToJiggle[i].InitRot;
                        CalculateTrigonoFor(i);
                        CalculateJigglingFor(i);
                    }
            }
            else
            {
                if (!SeparatedCalculations)
                    for (int i = 0; i < ToJiggle.Count; i++)
                    {
                        ToJiggle[i].Transform.localScale = ToJiggle[i].InitScale;
                        CalculateJigglingFor(i);
                    }
                else
                    for (int i = 0; i < ToJiggle.Count; i++)
                    {
                        ToJiggle[i].Transform.localScale = ToJiggle[i].InitScale;
                        CalculateTrigonoFor(i);
                        CalculateJigglingFor(i);
                    }
            }

            if (animationFinished)
            {
                OnAnimationFinish();
                enabled = false;
            }
        }


        /// <summary>
        /// Calculating motion for each jiggling element in chain
        /// </summary>
        private void CalculateJigglingFor(int index)
        {
            FJiggling_Element jigglE = ToJiggle[index];
            Transform t = jigglE.Transform;

            float val1 = 0f; float val2 = 0f;

            for (int i = 0; i < RandomLevel * 2; i++)
            {
                if (i % 2 == 0) val1 += jigglE.trigParams[i].Value; else val2 += jigglE.trigParams[i].Value;
            }

            val1 /= RandomLevel; val2 /= RandomLevel;

            // Additional variation to rotating
            float add1 = 0f; float add2 = 0;
            if (RandomLevel > 1)
            {
                add1 = jigglE.trigParams[3].Value;
                add2 = jigglE.trigParams[2].Value;
            }

            t.transform.localRotation *= Quaternion.Euler((val1 + reJiggleValue + add1) * jigglE.RotationAxesMul.x * RotationAxesMultiplier.x, (-val2 + reJiggleValue - add1) * jigglE.RotationAxesMul.y * RotationAxesMultiplier.y, (val2 + reJiggleValue + add2) * jigglE.RotationAxesMul.z * RotationAxesMultiplier.z);
            t.transform.localScale += new Vector3(trigParams[0].Value * jigglE.ScaleAxesMul.x * ScaleAxesMultiplier.x, (((trigParams[0].Value + trigParams[1].Value) / 2f)) * jigglE.ScaleAxesMul.y * ScaleAxesMultiplier.y, trigParams[0].Value * jigglE.ScaleAxesMul.z * ScaleAxesMultiplier.z) * 0.01f;
        }


        protected void CalculateTrigonoFor(int i)
        {
            for (int r = 0; r < RandomLevel; r++)
            {
                for (int j = 0; j < 2; j++)
                {
                    TrigonoParams parameters = ToJiggle[i].trigParams[r + j];

                    float timeValue = time * parameters.RandomTimeMul + parameters.TimeOffset;
                    float multiplyValue = JiggleTiltValue * parameters.Multiplier * currentJigglePower / AdditionalSpeedValue;

                    if (j % 2 == 0) parameters.Value = Mathf.Sin(timeValue) * multiplyValue; else parameters.Value = Mathf.Cos(timeValue) * multiplyValue;
                }
            }
        }

        /// <summary>
        /// Supporting separated trigonometric functions randomization if right toggle enabled
        /// </summary>
        protected override void RandomizeVariables()
        {
            if (!SeparatedCalculations)
            {
                base.RandomizeVariables();
                for (int j = 0; j < ToJiggle.Count; j++) ToJiggle[j].trigParams = trigParams;
            }
            else
            {
                base.RandomizeVariables();

                for (int j = 0; j < ToJiggle.Count; j++)
                {
                    ToJiggle[j].trigParams = new List<TrigonoParams>();

                    for (int i = 0; i < RandomLevel; i++)
                    {
                        for (int t = 0; t < 2; t++)
                        {
                            TrigonoParams newParams = new TrigonoParams();
                            newParams.Randomize();
                            ToJiggle[j].trigParams.Add(newParams);
                        }
                    }
                }
            }
        }


        #region List Support Methods

        public void AddNewElement(FJiggling_Element el)
        {
            bool canAdd = false;

            if (ToJiggle == null) ToJiggle = new List<FJiggling_Element>();

            if (el.Transform == null)
            {
                canAdd = true;
            }
            else
                if (!ToJiggle.Contains(el))
                canAdd = true;

            if (canAdd)
            {
                ToJiggle.Add(el);
            }
        }

        public void RemoveElement(FJiggling_Element el)
        {
            ToJiggle.Remove(el);
        }

        public void RemoveElement(int index)
        {
            ToJiggle.RemoveAt(index);
        }

        public void ClearElements()
        {
            ToJiggle.Clear();
        }

        public bool ContainsElement(Transform t)
        {
            for (int i = 0; i < ToJiggle.Count; i++)
                if (ToJiggle[i].Transform == t)
                    return true;

            return false;
        }

        #endregion

        [System.Serializable]
        public class FJiggling_Element
        {
            public FJiggling_Element(Transform target)
            {
                Transform = target;
            }

            public Transform Transform;

            public Vector3 RotationAxesMul = Vector3.one;
            public Vector3 ScaleAxesMul = Vector3.one;

            public Vector3 InitPos;
            public Quaternion InitRot;
            public Vector3 InitScale;

            public List<TrigonoParams> trigParams;
        }


#if UNITY_EDITOR

        protected override void OnDrawGizmos()
        {
            if (!drawGizmo) return;

            for (int i = 0; i < ToJiggle.Count; i++)
            {
                if (ToJiggle[i].Transform != null)
                    Gizmos.DrawIcon(ToJiggle[i].Transform.position, "FIMSpace/Jiggling/Jiggling Gizmo.png");
            }
        }

#endif

    }



#if UNITY_EDITOR
    /// <summary>
    /// FM: Editor class component to enchance controll over component from inspector window
    /// </summary>
    [UnityEditor.CustomEditor(typeof(FJiggling_Multi))]
    public class FJiggling_MultiEditor : UnityEditor.Editor
    {
        private static bool showTransforms = false;

        //private SerializedProperty sp_sep;

        //private void OnEnable()
        //{
        //    sp_sep = serializedObject.FindProperty("SeparatedCalculations");
        //}

        public override void OnInspectorGUI()
        {
            FJiggling_Multi targetScript = (FJiggling_Multi)target;

            DrawDefaultInspector();

            GUILayout.Space(5);

            GUILayout.BeginVertical(EditorStyles.helpBox);

            Color preCol = GUI.color;
            GUI.color = new Color(0.5f, 1f, 0.5f, 0.9f);

            var drop = GUILayoutUtility.GetRect(0f, 22f, new GUILayoutOption[1] { GUILayout.ExpandWidth(true) });
            GUI.Box(drop, "Drag & Drop your GameObjects here", new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter, fixedHeight = 22 });
            var dropEvent = Event.current;

            GUILayout.Space(3);

            GUILayout.BeginHorizontal();

            EditorGUIUtility.labelWidth = 166;
            targetScript.ShowIndividualOptions = EditorGUILayout.Toggle(new GUIContent(" Show Individual Options", "If single stimulated transforms don't have animated tracks from individual tracks like rotation or scale, also with this you can chcange intensity of effect for each element separately"), targetScript.ShowIndividualOptions);
            EditorGUIUtility.labelWidth = 0;
            EditorGUI.indentLevel++;

            if (ActiveEditorTracker.sharedTracker.isLocked) GUI.color = new Color(0.44f, 0.44f, 0.44f, 0.8f); else GUI.color = preCol;
            if (GUILayout.Button(new GUIContent("Lock Inspector", "Locking Inspector Window to help Drag & Drop operations"), new GUILayoutOption[2] { GUILayout.Width(106), GUILayout.Height(16) })) ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;

            GUI.color = preCol;

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            switch (dropEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop.Contains(dropEvent.mousePosition)) break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (dropEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            GameObject draggedObject = dragged as GameObject;

                            if (draggedObject)
                            {
                                targetScript.AddNewElement(new FJiggling_Multi.FJiggling_Element(draggedObject.transform));
                                EditorUtility.SetDirty(target);
                            }
                        }

                    }

                    Event.current.Use();
                    break;
            }

            if (targetScript.ToJiggle == null) targetScript.ToJiggle = new List<FJiggling_Multi.FJiggling_Element>();

            GUILayout.BeginHorizontal();
            showTransforms = EditorGUILayout.Foldout(showTransforms, "To Stimulate (" + targetScript.ToJiggle.Count + ")", true);

            if (GUILayout.Button("All", new GUILayoutOption[2] { GUILayout.MaxWidth(48), GUILayout.MaxHeight(14) }))
            {
                targetScript.ToJiggle.Clear();

                foreach (Transform tr in FTransformMethods.FindComponentsInAllChildren<Transform>(targetScript.transform))
                {
                    targetScript.AddNewElement(new FJiggling_Multi.FJiggling_Element(tr));
                }

                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("+", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
            {
                targetScript.AddNewElement(new FJiggling_Multi.FJiggling_Element(null));
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("-", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
                if (targetScript.ToJiggle.Count > 0)
                {
                    targetScript.RemoveElement(targetScript.ToJiggle.Count - 1);
                    EditorUtility.SetDirty(target);
                }

            if (GUILayout.Button("C", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
            {
                targetScript.ClearElements();
                EditorUtility.SetDirty(target);
            }

            GUILayout.EndHorizontal();
            GUI.color = preCol;

            if (showTransforms)
            {
                GUILayout.Space(3);

                if (!targetScript.ShowIndividualOptions)
                {
                    for (int i = 0; i < targetScript.ToJiggle.Count; i++)
                    {
                        GUILayout.BeginHorizontal();

                        string name;
                        if (!targetScript.ToJiggle[i].Transform)
                        {
                            name = "Assign Object";
                            GUI.color = new Color(0.9f, 0.4f, 0.4f, 0.9f);
                        }
                        else
                        {
                            name = targetScript.ToJiggle[i].Transform.name;
                            if (name.Length > 12) name = targetScript.ToJiggle[i].Transform.name.Substring(0, 7) + "...";
                        }

                        targetScript.ToJiggle[i].Transform = (Transform)EditorGUILayout.ObjectField("  [" + i + "] " + name, targetScript.ToJiggle[i].Transform, typeof(Transform), true);

                        GUI.color = preCol;
                        if (GUILayout.Button("X", new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(14) }))
                        {
                            targetScript.ToJiggle.RemoveAt(i);
                            EditorUtility.SetDirty(target);
                        }

                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    for (int i = 0; i < targetScript.ToJiggle.Count; i++)
                    {
                        GUILayout.BeginHorizontal();

                        string name;
                        if (!targetScript.ToJiggle[i].Transform)
                        {
                            name = "Assign Object";
                            GUI.color = new Color(0.9f, 0.4f, 0.4f, 0.9f);
                        }
                        else
                        {
                            name = targetScript.ToJiggle[i].Transform.name;
                            if (name.Length > 12) name = targetScript.ToJiggle[i].Transform.name.Substring(0, 7) + "...";
                        }

                        targetScript.ToJiggle[i].Transform = (Transform)EditorGUILayout.ObjectField("  [" + i + "] " + name, targetScript.ToJiggle[i].Transform, typeof(Transform), true);
                        GUI.color = preCol;

                        if (GUILayout.Button("X", new GUILayoutOption[2] { GUILayout.Width(20), GUILayout.Height(14) }))
                        {
                            targetScript.ToJiggle.RemoveAt(i);
                            EditorUtility.SetDirty(target);
                        }

                        GUILayout.EndHorizontal();

                        targetScript.ToJiggle[i].RotationAxesMul = EditorGUILayout.Vector3Field(new GUIContent("     Rot. Mul.", "Individual Rotation Axes Multiplier"), targetScript.ToJiggle[i].RotationAxesMul);
                        targetScript.ToJiggle[i].ScaleAxesMul = EditorGUILayout.Vector3Field(new GUIContent("     Scale Mul.", "Individual Rotation Axes Multiplier"), targetScript.ToJiggle[i].ScaleAxesMul);

                        GUILayout.Space(7);
                    }
                }
            }

            EditorGUI.indentLevel--;
            GUILayout.EndVertical();

            GUILayout.Space(5f);

            if (!Application.isPlaying) GUI.color = FColorMethods.ChangeColorAlpha(GUI.color, 0.45f); else GUI.color = preCol;
            if (GUILayout.Button("Jiggle It")) if (Application.isPlaying) targetScript.StartJiggle(); else Debug.Log("You must be in playmode to run this method!");
        }
    }
#endif


}