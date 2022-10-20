using UnityEngine;
using UnityEditor;
using NodeEditor.Component;
using NodeEditor.Data;
using NodeEditor.Tools;

namespace NodeEditor.Window
{
    public class NodeInspectorWindow : EditorWindow
    {
        private NodeComponent _m_pNode;
        private ScriptField[] _m_arrFields;

        public static NodeInspectorWindow OpenNodeInspector(object pObject)
        {
            NodeInspectorWindow pWindow = GetWindow<NodeInspectorWindow>();
            pWindow._m_pNode = pObject as NodeComponent;
            pWindow.Show();
            return pWindow;
        }

        public void RefreshData(object pObject)
        {
            _m_pNode = pObject as NodeComponent;
        }

        public void OnGUI()
        {
            if (_m_pNode != null)
            {
                _m_pNode.Name = EditorGUILayout.TextField("Node Name", _m_pNode.Name, GUILayout.ExpandWidth(true));
                _m_pNode.SetID(EditorGUILayout.IntField("Node ID", _m_pNode.ID));

                var pScript = EditorGUILayout.ObjectField("Data", _m_pNode.m_pScript, typeof(MonoScript), false) as MonoScript;
                if (pScript != _m_pNode.m_pScript)
                {
                    _m_pNode.m_pScript = pScript;
                    if (_m_pNode.m_pScript != null)
                    {
                        _m_pNode.SetDataSource(Activator.CreateInstance(_m_pNode.m_pScript.GetClass()) as DataBase);
                    }
                }

                if (_m_pNode.m_pData != null)
                {
                    _m_arrFields = FieldDrawer.Capture(_m_pNode.m_pData);
                    FieldDrawer.Draw(_m_arrFields);
                }
                else
                {
                    EditorGUI.HelpBox(EditorGUILayout.GetControlRect(), "Data must inherit from " + typeof(DataBase), MessageType.Warning);
                }
            }
        }

    }
}
