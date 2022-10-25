using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace NodeEditor.Tools
{
    [AttributeUsage(AttributeTargets.All)]
    public class ScriptFieldAttribute : Attribute
    {

    }

    public class ScriptField
    {
        public System.Object m_pInstance;
        public FieldInfo m_pFieldInfo;
        public SerializedPropertyType m_pType;

        public ScriptField(System.Object pInstance, FieldInfo pFieldInfo, SerializedPropertyType pType)
        {
            m_pInstance = pInstance;
            m_pFieldInfo = pFieldInfo;
            m_pType = pType;
        }
    
        public object GetValue()
        {
            return m_pFieldInfo.GetValue(m_pInstance);
        }
        public void SetValue(object pVal)
        {
            m_pFieldInfo.SetValue(m_pInstance, pVal);
        }
    
        public static bool GetFieldType(FieldInfo pInfo, out SerializedPropertyType pType)
        {
            pType = SerializedPropertyType.Generic;

            switch (pInfo.FieldType)
            {
                case Type t when t == typeof(int):
                    pType = SerializedPropertyType.Integer;
                    return true;
                case Type t when t == typeof(float):
                    pType = SerializedPropertyType.Float;
                    return true;
                case Type t when t == typeof(bool):
                    pType = SerializedPropertyType.Boolean;
                    return true;
                case Type t when t == typeof(string):
                    pType = SerializedPropertyType.String;
                    return true;
                case Type t when t == typeof(Vector2):
                    pType = SerializedPropertyType.Vector2;
                    return true;
                case Type t when t == typeof(Vector3):
                    pType = SerializedPropertyType.Vector3;
                    return true;
                case Type t when t == typeof(Vector4):
                    pType = SerializedPropertyType.Vector4;
                    return true;
                case Type t when t == typeof(Quaternion):
                    pType = SerializedPropertyType.Quaternion;
                    return true;
                case Type t when t == typeof(Rect):
                    pType = SerializedPropertyType.Rect;
                    return true;
                case Type t when t == typeof(Color):
                    pType = SerializedPropertyType.Color;
                    return true;
                case Type t when t == typeof(AnimationCurve):
                    pType = SerializedPropertyType.AnimationCurve;
                    return true;
                case Type t when t == typeof(Bounds):
                    pType = SerializedPropertyType.Bounds;
                    return true;
                case Type t when t == typeof(Gradient):
                    pType = SerializedPropertyType.Gradient;
                    return true;
                case Type t when t == typeof(Vector2Int):
                    pType = SerializedPropertyType.Vector2Int;
                    return true;
                case Type t when t == typeof(Vector3Int):
                    pType = SerializedPropertyType.Vector3Int;
                    return true;
                case Type t when t == typeof(RectInt):
                    pType = SerializedPropertyType.RectInt;
                    return true;
                case Type t when t == typeof(BoundsInt):
                    pType = SerializedPropertyType.BoundsInt;
                    return true;
                case Type t when t == typeof(Enum):
                    pType = SerializedPropertyType.Enum;
                    return true;
                default:
                    if (pInfo.FieldType.IsEnum)
                    {
                        pType = SerializedPropertyType.Enum;
                        return true;
                    }
                    return false;
            }
        }
    }

    public class FieldDrawer
    {
        public static ScriptField[] Capture(System.Object pObject)
        {
            List<ScriptField> listFields = new List<ScriptField>(8);
            FieldInfo[] arrInfos = pObject.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo pInfo in arrInfos)
            {
                object[] arrAttrs = pInfo.GetCustomAttributes(typeof(ScriptFieldAttribute), true);
                if (arrAttrs.Length > 0)
                {
                    if (ScriptField.GetFieldType(pInfo, out SerializedPropertyType pType))
                    {
                        listFields.Add(new ScriptField(pObject, pInfo, pType));
                    }
                }
            }
            return listFields.ToArray();
        }

        public static void Draw(ScriptField[] pFields)
        {
            GUILayoutOption[] arrOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            EditorGUILayout.BeginVertical(arrOptions);
            foreach (ScriptField pField in pFields)
            {
                EditorGUILayout.BeginHorizontal(arrOptions);
                switch (pField.m_pType)
                {
                    case SerializedPropertyType.Integer:
                        pField.SetValue(EditorGUILayout.IntField(pField.m_pFieldInfo.Name, (int)pField.GetValue()));
                        break;
                    case SerializedPropertyType.Boolean:
                        pField.SetValue(EditorGUILayout.Toggle(pField.m_pFieldInfo.Name, (bool)pField.GetValue()));
                        break;
                    case SerializedPropertyType.Float:
                        pField.SetValue(EditorGUILayout.FloatField(pField.m_pFieldInfo.Name, (float)pField.GetValue()));
                        break;
                    case SerializedPropertyType.String:
                        pField.SetValue(EditorGUILayout.TextField(pField.m_pFieldInfo.Name, (string)pField.GetValue()));
                        break;
                    case SerializedPropertyType.Vector2:
                        pField.SetValue(EditorGUILayout.Vector2Field(pField.m_pFieldInfo.Name, (Vector2)pField.GetValue()));
                        break;
                    case SerializedPropertyType.Vector3:
                        pField.SetValue(EditorGUILayout.Vector3Field(pField.m_pFieldInfo.Name, (Vector3)pField.GetValue()));
                        break;
                    case SerializedPropertyType.Vector4:
                        pField.SetValue(EditorGUILayout.Vector4Field(pField.m_pFieldInfo.Name, (Vector4)pField.GetValue()));
                        break;
                    case SerializedPropertyType.Rect:
                        pField.SetValue(EditorGUILayout.RectField(pField.m_pFieldInfo.Name, (Rect)pField.GetValue()));
                        break;
                    case SerializedPropertyType.ArraySize:
                        break;
                    case SerializedPropertyType.Character:
                        break;
                    case SerializedPropertyType.AnimationCurve:
                        pField.SetValue(EditorGUILayout.CurveField(pField.m_pFieldInfo.Name, (AnimationCurve)pField.GetValue()));
                        break;
                    case SerializedPropertyType.Bounds:
                        pField.SetValue(EditorGUILayout.BoundsField(pField.m_pFieldInfo.Name, (Bounds)pField.GetValue()));
                        break;
                    case SerializedPropertyType.Gradient:
                        break;
                    case SerializedPropertyType.Quaternion:
                        break;
                    case SerializedPropertyType.Enum:
                        pField.SetValue(EditorGUILayout.EnumPopup(pField.m_pFieldInfo.Name, (Enum)pField.GetValue()));
                        break;
                    default:
                        break;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
    }
}
