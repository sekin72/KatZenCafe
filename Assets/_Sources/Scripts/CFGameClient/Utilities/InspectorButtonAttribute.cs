using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace CFGameClient.Utilities
{
    public class InspectorButtonAttribute : PropertyAttribute
    {
        public string MethodName;
        public string Name;
        public bool UseAsValue;

        public InspectorButtonAttribute(string methodName, bool useAsValue = false, string customName = null)
        {
            MethodName = methodName;
            UseAsValue = useAsValue;

            Name = string.IsNullOrEmpty(customName) ? methodName : customName;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    public class InspectorButtonDrawer : PropertyDrawer
    {
        private float _defaultHeight;
        private FieldInfo _fieldInfo;

        private InspectorButtonAttribute _inspectorButtonAttribute;
        private MethodInfo _methodInfo;
        private Object _targetObject;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_methodInfo == null)
            {
                _inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
                _targetObject = property.serializedObject.targetObject;
                _methodInfo = property.serializedObject.targetObject.GetType().GetMethod(
                    _inspectorButtonAttribute.MethodName,
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (_methodInfo == null)
                    Debug.LogError(
                        $"The method \"{_targetObject.GetType().FullName}.{_inspectorButtonAttribute.MethodName}\" COULD NOT BE FOUND!");
                _fieldInfo = _targetObject.GetType().GetField(property.name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                return;
            }

            if (_inspectorButtonAttribute.UseAsValue)
            {
                EditorGUI.PropertyField(position, property, label, true);
                position.y += _defaultHeight + EditorGUIUtility.standardVerticalSpacing;
                position.height = _defaultHeight * 1.5f;

                if (_inspectorButtonAttribute.MethodName.Equals("InspectorButton_ChangeToLevelWithType"))
                    position.y += 40;
                if (GUI.Button(position,
                    $"{_inspectorButtonAttribute.Name}({property.name} = {_fieldInfo.GetValue(_targetObject)})"))
                    _methodInfo.Invoke(_targetObject, new[]
                    {
                        _fieldInfo.GetValue(_targetObject)
                    });
            }
            else
            {
                position.height = _defaultHeight * 1.5f;
                if (GUI.Button(position, _inspectorButtonAttribute.Name)) _methodInfo.Invoke(_targetObject, null);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_methodInfo == null) _inspectorButtonAttribute = (InspectorButtonAttribute)attribute;

            if (_inspectorButtonAttribute.MethodName.Equals("InspectorButton_ChangeToLevelWithType"))
                return (_defaultHeight = base.GetPropertyHeight(property, label)) +
                    (_inspectorButtonAttribute.UseAsValue
                        ? (_defaultHeight * 1.5f) + EditorGUIUtility.standardVerticalSpacing + 40
                        : _defaultHeight * 0.5f);

            return (_defaultHeight = base.GetPropertyHeight(property, label)) + (_inspectorButtonAttribute.UseAsValue
                ? (_defaultHeight * 1.5f) + EditorGUIUtility.standardVerticalSpacing
                : _defaultHeight * 0.5f);
        }
    }

#endif
}