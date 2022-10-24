using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using NodeEditor.Data;
using NodeEditor.Config;

namespace NodeEditor.Component
{
    public class NodeComponent : DataBase
    {
        private int _m_uuid;
        public int UUID { get => _m_uuid; }

        private string _m_strName;
        public string Name
        {
            get => _m_strName;
            set => _m_strName = value;
        }

        public Rect m_pRect;

        private bool m_bRoot = false;
        public bool Root
        {
            get => m_bRoot;
        }

        private bool m_bSelected;
        public bool Selected
        {
            get => m_bSelected;
            set => m_bSelected = value;
        }

        private List<NodeComponent> _m_pIn;
        private List<NodeComponent> _m_pOut;

        public MonoScript m_pScript;
        public DataBase m_pData;
        public void SetDataSource(DataBase pData)
        {
            m_pData = pData;
            if (m_pData != null)
            {
                m_pData.ID = ID;
            }
        }

        public NodeComponent()
        {
            _m_uuid = this.GetHashCode();
            _m_strName = "Node";

            _m_pIn = new List<NodeComponent>(8);
            _m_pOut = new List<NodeComponent>(8);
        }
        public NodeComponent(bool root)
        {
            m_bRoot = root;
            m_pRect = new Rect(new Rect(200, 250, 160, 80));

            _m_uuid = this.GetHashCode();
            _m_strName = m_bRoot ? "Root" : "Node";

            _m_pIn = new List<NodeComponent>(8);
            _m_pOut = new List<NodeComponent>(8);
        }
        public NodeComponent(Vector2 v2Position) : this()
        {
            m_pRect = new Rect(v2Position.x, v2Position.y, 160, 80);
        }

        public void SetRoot()
        {
            m_bRoot = true;
            _m_strName = "Root";
            
            for (int i = 0; i < _m_pIn.Count; ++i)
            {
                Disconnect(_m_pIn[i], this);
            }
        }

        public void SetID(int id)
        {
            ID = id;
            if (m_pData != null)
            {
                m_pData.ID = ID;
            }
        }

        public void Draw()
        {
            DrawNode();
            DrawBezier_OutNodes();
        }
    
        private void DrawNode()
        {
            if (m_bRoot)
            {
                GUI.Box(m_pRect, _m_strName, UIStyle.ms_pStyleRootNode);
            }
            else
            {
                if (m_bSelected)
                {
                    GUI.Box(m_pRect, _m_strName, UIStyle.ms_pStyleSelected);
                }
                else
                {
                    GUI.Box(m_pRect, _m_strName, UIStyle.ms_pStyleNormal);
                }
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
            if (this.m_bRoot || _m_pIn.Contains(pNode))
            {
                return;
            }
            _m_pIn.Add(pNode);
        }
        private void AddOutNode(NodeComponent pNode)
        {
            if (pNode.m_bRoot || _m_pOut.Contains(pNode))
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
        public void Connect(NodeComponent pIn, NodeComponent pOut)
        {
            pIn.AddOutNode(pOut);
            pOut.AddInNode(pIn);
        }
        public void Connect(NodeComponent pNode)
        {
            Connect(this, pNode);
        }
        public void Disconnect(NodeComponent pIn, NodeComponent pOut)
        {
            pIn.RemoveOutNode(pOut);
            pOut.RemoveInNode(pIn);
        }
        public void Disconnect(NodeComponent pNode)
        {
            Disconnect(this, pNode);
            Disconnect(pNode, this);
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
    
        public List<NodeComponent> GetInNodes()
        {
            return _m_pIn;
        }
        public List<NodeComponent> GetOutNodes()
        {
            return _m_pOut;
        }
    
        public override object Serializer()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Name:").Append(_m_strName).Append("\n");
            sb.Append("Size:").Append(m_pRect.width).Append(',').Append(m_pRect.height).Append("\n");
            sb.Append("Position:").Append(m_pRect.position.x).Append(',').Append(m_pRect.position.y);
            return sb;
        }
        public override void Deserializer(object obj)
        {
            StringBuilder sb = (StringBuilder)obj;
            string[] strLines = sb.ToString().Split('\n');
            foreach (string strLine in strLines)
            {
                string[] strValues = strLine.Split(':');
                if (strValues.Length != 2)
                {
                    continue;
                }
                string strKey = strValues[0];
                string strValue = strValues[1];
                switch (strKey)
                {
                    case "Name":
                        _m_strName = strValue;
                        break;
                    case "Size":
                        string[] strSize = strValue.Split(',');
                        m_pRect.width = float.Parse(strSize[0]);
                        m_pRect.height = float.Parse(strSize[1]);
                        break;
                    case "Position":
                        string[] strPosition = strValue.Split(',');
                        m_pRect.position = new Vector2(float.Parse(strPosition[0]), float.Parse(strPosition[1]));
                        break;
                }
            }
        }
    }
}
