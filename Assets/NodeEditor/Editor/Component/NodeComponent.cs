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
        }

        public Rect m_pRect;
        public Rect m_pRectEnter;
        public Rect m_pRectExit;

        private float _m_fZoom = 1;
        private Vector2 v2Delta = Vector2.zero;

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
            _m_strName = "节点";
            
            _m_pIn = new List<NodeComponent>(8);
            _m_pOut = new List<NodeComponent>(8);
        }
        public NodeComponent(bool root)
        {
            m_bRoot = root;
            m_pRect = new Rect(new Rect(200, 300, 160, 80));

            InitBezierPoint();

            _m_uuid = this.GetHashCode();
            _m_strName = m_bRoot ? "根节点" : "节点";

            _m_pIn = new List<NodeComponent>(8);
            _m_pOut = new List<NodeComponent>(8);
        }
        public NodeComponent(Vector2 v2Position, float fZoom) : this()
        {
            m_pRect = new Rect(v2Position.x, v2Position.y, 160, 80);
            InitBezierPoint();
            
            if (fZoom > 0)
            {
                _m_fZoom += fZoom;
                v2Delta /= fZoom;
                m_pRect.width /= fZoom;
                m_pRect.height /= fZoom;
                m_pRectEnter.width /= fZoom;
                m_pRectEnter.height /= fZoom;
                m_pRectExit.width /= fZoom;
                m_pRectExit.height /= fZoom;
                SetBezierPointPosition();
            }
            else if (fZoom < 0)
            {
                _m_fZoom += fZoom;
                v2Delta *= -fZoom;
                m_pRect.width *= -fZoom;
                m_pRect.height *= -fZoom;
                m_pRectEnter.width *= -fZoom;
                m_pRectEnter.height *= -fZoom;
                m_pRectExit.width *= -fZoom;
                m_pRectExit.height *= -fZoom;
                SetBezierPointPosition();
            }
        }

        public void InitBezierPoint()
        {
            float fSize = 24;
            m_pRectEnter = new Rect(m_pRect.x - fSize, m_pRect.y - 0.5f*fSize + 0.5f*m_pRect.height, fSize, fSize);
            m_pRectExit = new Rect(m_pRect.x + m_pRect.width, m_pRect.y - 0.5f*fSize + 0.5f*m_pRect.height, fSize, fSize);
        }
        public void SetBezierPointPosition()
        {
            m_pRectEnter = new Rect(m_pRect.x - m_pRectEnter.width, m_pRect.y - 0.5f*m_pRectEnter.height + 0.5f*m_pRect.height, m_pRectEnter.width, m_pRectEnter.height);
            m_pRectExit = new Rect(m_pRect.x + m_pRect.width, m_pRect.y - 0.5f*m_pRectExit.height + 0.5f*m_pRect.height, m_pRectExit.width, m_pRectExit.height);
        }
        public void SetPosition(Vector2 v2Position)
        {
            m_pRect.position = v2Position;
            SetBezierPointPosition();
        }

        public void SetRoot()
        {
            m_bRoot = true;
            _m_strName = "根节点";
        
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
        public void SetName(string strName)
        {
            _m_strName = strName;
        }
        public void Draw()
        {
            DrawBezier_OutNodes();
            DrawNode();
        }
    
        private void DrawNode()
        {
            if (m_bRoot)
            {
                GUI.Box(m_pRect, _m_strName, UIStyle.ms_pStyleRootNode);
            }
            else
            {
                GUI.Box(m_pRectEnter, "", UIStyle.ms_pStyleEnterPoint);
                if (m_bSelected)
                {
                    GUI.Box(m_pRect, _m_strName, UIStyle.ms_pStyleSelected);
                }
                else
                {
                    GUI.Box(m_pRect, _m_strName, UIStyle.ms_pStyleNormal);
                }
            }
            GUI.Box(m_pRectExit, "", UIStyle.ms_pStyleExitPoint);
        }
    
        private void DrawBezier_OutNodes()
        {
            if (_m_pOut.Count > 0)
            {
                for (int i = 0; i < _m_pOut.Count; ++i)
                {
                    Vector2 v2Start = this.m_pRectExit.center, v2End = _m_pOut[i].m_pRectEnter.center;
                    int iGradDirX = 1, iGradDirY = -1;
                    Handles.DrawBezier(v2Start, v2End, v2Start + iGradDirX*100*Vector2.right, v2End + iGradDirY*100*Vector2.right, Color.cyan, null, UIStyle.ms_fBezierLineWidth*_m_fZoom);
                    
                    if (m_bRoot)
                    {
                        GUI.Box(new Rect(v2Start + 0.5f*(v2End - v2Start) - 0.5f*UIStyle.ms_v2BranchTextSize - UIStyle.ms_pStyleBranchText.fontSize*Vector2.one, UIStyle.ms_v2BranchTextSize), i == 0 ? "主线" : ("支线" + i), UIStyle.ms_pStyleBranchText);
                    }
                }
            }
        }
    
        public void Drag(Vector2 v2Delta)
        {
            this.v2Delta += v2Delta;
            m_pRect.position += v2Delta;
            m_pRectEnter.position += v2Delta;
            m_pRectExit.position += v2Delta;
        }
        public void Zoom(float fZoom)
        {
            v2Delta *= fZoom;
            m_pRect.position *= fZoom;
            m_pRectEnter.position *= fZoom;
            m_pRectExit.position *= fZoom;
            m_pRect.width *= fZoom;
            m_pRect.height *= fZoom;
            m_pRectEnter.width *= fZoom;
            m_pRectEnter.height *= fZoom;
            m_pRectExit.width *= fZoom;
            m_pRectExit.height *= fZoom;
        }
        public void ZoomIn(float fZoom)
        {
            _m_fZoom += fZoom;
            Zoom(1/fZoom);
        }
        public void ZoomOut(float fZoom)
        {
            _m_fZoom -= fZoom;
            Zoom(fZoom);
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

            float fZoom = 1-_m_fZoom;
            float fWidth = m_pRect.width, fHeight = m_pRect.height, fPositionX = m_pRect.position.x, fPositionY = m_pRect.position.y;
            if (fZoom > 0)
            {
                fWidth = m_pRect.width / fZoom;
                fHeight = m_pRect.height / fZoom;
                fPositionX = m_pRect.position.x / fZoom;
                fPositionY = m_pRect.position.y / fZoom;
            }
            else if (fZoom < 0)
            {
                fWidth = m_pRect.width * -fZoom;
                fHeight = m_pRect.height * -fZoom;
                fPositionX = m_pRect.position.x * -fZoom;
                fPositionY = m_pRect.position.y * -fZoom;
            }

            sb.Append("Size:").Append(fWidth).Append(',').Append(fHeight).Append("\n");
            sb.Append("Position:").Append(fPositionX).Append(',').Append(fPositionY);
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
                        SetPosition(new Vector2(float.Parse(strPosition[0]), float.Parse(strPosition[1])));
                        InitBezierPoint();
                        break;
                }
            }
        }
    }
}
