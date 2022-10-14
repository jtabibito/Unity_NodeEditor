using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditor.Data;

namespace NodeEditor.Component
{
    public class NodeComponent
    {
        private int _m_uuid;
        private string _m_strName;
        public string Name
        {
            get => _m_strName;
            set => _m_strName = value;
        }

        public Rect m_pRect;

        private bool m_bSelected;
        public bool Selected
        {
            get => m_bSelected;
            set => m_bSelected = value;
        }

        private List<NodeComponent> _m_pIn;
        private List<NodeComponent> _m_pOut;

        private GUIStyle _m_pStyleNormal;
        private GUIStyle _m_pStyleSelected;

        public MonoScript m_pScript;
        public object m_pData;

        public NodeComponent(int uuid)
        {
            _m_uuid = uuid;
            _m_strName = string.Concat("Node", _m_uuid);

            _m_pStyleNormal = new GUIStyle();
            _m_pStyleNormal.normal.background = EditorGUIUtility.Load("Assets/Resources/node.png") as Texture2D;
            _m_pStyleNormal.alignment = TextAnchor.MiddleCenter;
            _m_pStyleNormal.fontSize = 15;
            _m_pStyleNormal.fontStyle = FontStyle.Normal;

            _m_pStyleSelected = new GUIStyle();
            _m_pStyleSelected.normal.background = EditorGUIUtility.Load("Assets/Resources/node_select.png") as Texture2D;
            _m_pStyleSelected.alignment = TextAnchor.MiddleCenter;
            _m_pStyleSelected.fontSize = 15;
            _m_pStyleSelected.fontStyle = FontStyle.BoldAndItalic;

            _m_pIn = new List<NodeComponent>(8);
            _m_pOut = new List<NodeComponent>(8);
        }
        public NodeComponent(int uuid, Vector2 v2Position) : this(uuid)
        {
            m_pRect = new Rect(v2Position.x, v2Position.y, 160, 80);
        }

        public void Draw()
        {
            DrawNode();
            DrawBezier_OutNodes();
        }
    
        private void DrawNode()
        {
            if (m_bSelected)
            {
                GUI.Box(m_pRect, _m_strName, _m_pStyleSelected);
            }
            else
            {
                GUI.Box(m_pRect, _m_strName, _m_pStyleNormal);
            }
        }
    
        private void DrawBezier_OutNodes()
        {
            if (_m_pOut.Count > 0)
            {
                foreach (NodeComponent pNode in _m_pOut)
                {
                    Vector2 v2Start = this.m_pRect.center, v2End = pNode.m_pRect.center;
                    int iGradDirX = 1, iGradDirY = -1;
                    if (pNode.m_pRect.xMax < m_pRect.xMin)
                    {
                        v2Start.x -= this.m_pRect.width*0.5f;
                        v2End.x += pNode.m_pRect.width*0.5f;
                        iGradDirX = -1;
                    }
                    else if (pNode.m_pRect.xMin > m_pRect.xMax)
                    {
                        v2Start.x += this.m_pRect.width*0.5f;
                        v2End.x -= pNode.m_pRect.width*0.5f;
                        iGradDirX = 1;
                    }
                    if (pNode.m_pRect.yMax < m_pRect.yMin)
                    {
                        v2Start.y -= this.m_pRect.height*0.5f;
                        v2End.y += pNode.m_pRect.height*0.5f;
                        iGradDirY = -1*iGradDirX;
                    }
                    else if (pNode.m_pRect.yMin > m_pRect.yMax)
                    {
                        v2Start.y += this.m_pRect.height*0.5f;
                        v2End.y -= pNode.m_pRect.height*0.5f;
                        iGradDirY = -1*iGradDirX;
                    }
                    Handles.DrawBezier(v2Start, v2End, v2Start + iGradDirX*100*Vector2.right, v2End + iGradDirY*100*Vector2.right, Color.cyan, null, 8);
                }
            }
        }
    
        public void Drag(Vector2 v2Delta)
        {
            m_pRect.position += v2Delta;
        }
        public bool InRange(Vector2 v2Position)
        {
            return m_pRect.Contains(v2Position);
        }

        private void AddInNode(NodeComponent pNode)
        {
            if (_m_pIn.Contains(pNode))
            {
                return;
            }
            _m_pIn.Add(pNode);
        }
        private void AddOutNode(NodeComponent pNode)
        {
            if (_m_pOut.Contains(pNode))
            {
                return;
            }
            _m_pOut.Add(pNode);
        }
        private void RemoveInNode(NodeComponent pNode)
        {
            _m_pIn.Remove(pNode);
        }
        private void RemoveOutNode(NodeComponent pNode)
        {
            _m_pOut.Remove(pNode);
        }
        public void Connect(NodeComponent pNode)
        {
            this.AddOutNode(pNode);
            pNode.AddInNode(this);
        }
        public void Disconnect(NodeComponent pNode)
        {
            this.RemoveInNode(pNode);
            this.RemoveOutNode(pNode);
            pNode.RemoveInNode(this);
            pNode.RemoveOutNode(this);
        }
        public void DisconnectAll()
        {
            foreach (NodeComponent pNode in _m_pIn)
            {
                pNode.RemoveOutNode(this);
            }
            foreach (NodeComponent pNode in _m_pOut)
            {
                pNode.RemoveInNode(this);
            }
            _m_pIn.Clear();
            _m_pOut.Clear();
        }
    }
}
