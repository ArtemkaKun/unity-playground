using ArtemkaKun.Scripts.Tools;
using UnityEditor;
using UnityEngine;

namespace ArtemkaKun.Scripts.Editor
{
    /// ATTENTION - script's author - Patryk Galach - https://www.patrykgalach.com/2020/01/20/readonly-attribute-in-unity-editor/
    /// <summary>
    /// This class contain custom drawer for ReadOnly attribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(EditorReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Unity method for drawing GUI in Editor
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="property">Property.</param>
        /// <param name="label">Label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Saving previous GUI enabled value
            var previousGUIState = GUI.enabled;
            // Disabling edit for property
            GUI.enabled = false;
            // Drawing Property
            EditorGUI.PropertyField(position, property, label);
            // Setting old GUI enabled value
            GUI.enabled = previousGUIState;
        }
    }
}