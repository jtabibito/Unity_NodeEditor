using UnityEngine;
using UnityEditor;
using NodeEditor.Component;
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
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Node Name");
                _m_pNode.Name = EditorGUILayout.TextField(_m_pNode.Name);
                EditorGUILayout.EndHorizontal();

                var pScript = EditorGUILayout.ObjectField("Data", _m_pNode.m_pScript, typeof(MonoScript), false) as MonoScript;
                if (pScript != _m_pNode.m_pScript)
                {
                    _m_pNode.m_pScript = pScript;
                    _m_pNode.m_pData = Activator.CreateInstance(_m_pNode.m_pScript.GetClass());
                }

                if (_m_pNode.m_pData != null)
                {
                    _m_arrFields = FieldDrawer.Capture((object)_m_pNode.m_pData);
                    FieldDrawer.Draw(_m_arrFields);
                }
            }
        }

    }
}
