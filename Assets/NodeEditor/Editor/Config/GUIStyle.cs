using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NodeEditor.Config
{
    public class UIStyle
    {
        public readonly static GUIStyle ms_pStyleNormal = new GUIStyle() {
            normal = new GUIStyleState() {
                background = EditorGUIUtility.Load(PathConfig.ms_ResourcePath + "node.png") as Texture2D,
            },
            alignment = TextAnchor.MiddleCenter,
            fontSize = 15,
            fontStyle = FontStyle.Normal
        };

        public readonly static GUIStyle ms_pStyleSelected = new GUIStyle() {
            normal = new GUIStyleState() {
                background = EditorGUIUtility.Load(PathConfig.ms_ResourcePath + "node_select.png") as Texture2D,
            },
            alignment = TextAnchor.MiddleCenter,
            fontSize = 15,
            fontStyle = FontStyle.BoldAndItalic
        };

        public readonly static GUIStyle ms_pStyleConnectPoint = new GUIStyle() {
            normal = new GUIStyleState() {
                background = EditorGUIUtility.Load(PathConfig.ms_ResourcePath + "point_normal.png") as Texture2D,
            }
        };

        public readonly static GUIStyle ms_pStyleRootNode = new GUIStyle() {
            normal = new GUIStyleState() {
                background = EditorGUIUtility.Load(PathConfig.ms_ResourcePath + "node_root.jpeg") as Texture2D,
            },
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
            fontStyle = FontStyle.Bold
        };

        public readonly static GUIStyle ms_pStyleEnterPoint = new GUIStyle() {
            normal = new GUIStyleState() {
                background = EditorGUIUtility.Load(PathConfig.ms_ResourcePath + "plus.png") as Texture2D,
            }
        };
        public readonly static GUIStyle ms_pStyleExitPoint = new GUIStyle() {
            normal = new GUIStyleState() {
                background = EditorGUIUtility.Load(PathConfig.ms_ResourcePath + "minus.png") as Texture2D,
            }
        };

        public static float ms_fBezierLineWidth = 6f;
    }
}
