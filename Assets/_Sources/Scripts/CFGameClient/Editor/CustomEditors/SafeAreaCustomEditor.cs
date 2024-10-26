using CerberusFramework.UI.Components;
using UnityEditor;
using UnityEngine;

namespace CerberusFramework.Utilities.CustomEditors
{
    [CustomEditor(typeof(SafeArea))]
    public class SafeAreaCustomEditor : Editor
    {
        public SafeArea SafeArea;

        public void OnEnable()
        {
            SafeArea = (SafeArea)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update Safe Area"))
            {
                SafeArea.UpdateSafeArea();
                EditorUtility.SetDirty(SafeArea);
            }
        }
    }
}
