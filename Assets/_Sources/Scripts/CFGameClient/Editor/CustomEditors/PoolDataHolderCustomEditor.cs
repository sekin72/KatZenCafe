using CerberusFramework.Managers.Pool;
using UnityEditor;
using UnityEngine;

namespace CerberusFramework.Utilities.CustomEditors
{
    [CustomEditor(typeof(PoolDataHolder))]
    public class PoolDataHolderCustomEditor : Editor
    {
        public PoolDataHolder PoolDataHolder;

        public void OnEnable()
        {
            PoolDataHolder = (PoolDataHolder)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Sort"))
            {
                PoolDataHolder.Sort();
                EditorUtility.SetDirty(PoolDataHolder);
            }
        }
    }
}