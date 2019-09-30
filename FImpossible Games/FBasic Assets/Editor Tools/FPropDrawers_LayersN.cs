#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace FIMSpace.FEditor
{
    public class FPropDrawers_LayersNAttribute : PropertyAttribute
    {
    }

    [CustomPropertyDrawer(typeof(FPropDrawers_LayersNAttribute))]
    public class FPropDrawers_LayersN : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
}


#endif
