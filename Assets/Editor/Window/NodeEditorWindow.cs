using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditor.Input;
using NodeEditor.Config;
using NodeEditor.Component;

namespace NodeEditor.Window
{
    public class NodeEditorWindow : EditorWindow
    {
        [MenuItem("EditorTools/Node Editor")]
        private static void OpenNodeEditor()
        {
            NodeEditorWindow window = (NodeEditorWindow)EditorWindow.GetWindow(typeof(NodeEditorWindow));
            window.Show();
        }

        protected InputEvent _m_pInputEvent;
        
        protected GridStyle _m_pGridStyle1;
        protected GridStyle _m_pGridStyle2;
        protected Vector2 _m_v2GridOffset;

        private List<NodeComponent> _m_listNodes;

        private NodeInspectorWindow _m_pInspectorWindow;

        public NodeEditorWindow()
        {
            _m_pInputEvent = new InputEvent();

            _m_pGridStyle1 = new GridStyle();
            _m_pGridStyle1.m_fSpace = 20;
            _m_pGridStyle1.m_fOpacity = 0.2f;
            _m_pGridStyle1.m_pColor = Color.gray;

            _m_pGridStyle2 = new GridStyle();
            _m_pGridStyle2.m_fSpace = 100;
            _m_pGridStyle2.m_fOpacity = 0.4f;
            _m_pGridStyle2.m_pColor = Color.gray;

            _m_listNodes = new List<NodeComponent>(8);
        }
        public void OnDestroy()
        {
            if (_m_pInspectorWindow != null)
            {
                _m_pInspectorWindow.Close();
            }
        }

        public void OnGUI()
        {
            // Draws
            DrawGrids(_m_pGridStyle1);
            DrawGrids(_m_pGridStyle2);
            DrawNodes();

            // Events
            _m_pInputEvent.current = Event.current;
            GUIEvents();

            if (GUI.changed)
            {
                Repaint();
            }
        }
    
        public void DrawGrids(GridStyle pStyle)
        {
            int widthDivs = Mathf.CeilToInt(position.width / pStyle.m_fSpace);
            int heightDivs = Mathf.CeilToInt(position.height / pStyle.m_fSpace);

            Handles.BeginGUI();
            Handles.color = new Color(pStyle.m_pColor.r, pStyle.m_pColor.g, pStyle.m_pColor.b, pStyle.m_fOpacity);

            float newOffsetX = _m_v2GridOffset.x % pStyle.m_fSpace;
            float newOffsetY = _m_v2GridOffset.y % pStyle.m_fSpace;

            for (int i = 0; i < widthDivs; ++i)
            {
                Handles.DrawLine(new Vector3(pStyle.m_fSpace*i + newOffsetX, -pStyle.m_fSpace, 0), new Vector3(pStyle.m_fSpace*i + newOffsetX, position.height, 0));
            }
            for (int j = 0; j < heightDivs; ++j)
            {
                Handles.DrawLine(new Vector3(-pStyle.m_fSpace, pStyle.m_fSpace*j + newOffsetY, 0), new Vector3(position.width, pStyle.m_fSpace*j + newOffsetY, 0));
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }
        public void DrawNodes()
        {
            foreach (NodeComponent node in _m_listNodes)
            {
                node.Draw();
            }
        }
    
        private void GUIEvents()
        {
            switch (_m_pInputEvent.current.type)
            {
                case EventType.MouseDown:
                    if (_m_pInputEvent.current.button == 0)
                    {
                        Event_MouseLeftDown(_m_pInputEvent);
                    }
                    else if (_m_pInputEvent.current.button == 1)
                    {
                        Event_MouseRightDown(_m_pInputEvent);
                    }
                    break;
                case EventType.MouseDrag:
                    if (_m_pInputEvent.current.button == 0)
                    {
                        Event_MouseLeftDrag(_m_pInputEvent);
                    }
                    break;
                case EventType.MouseUp:
                    if (_m_pInputEvent.current.button == 0)
                    {
                        Event_MouseLeftUp(_m_pInputEvent);
                    }
                    break;
                default:
                    break;
            }
        }
    
        private void Event_MouseLeftDown(InputEvent pEvent)
        {
            if (pEvent.m_pState.InState(InputState.OP_CONNECT_NODE | InputState.OP_DISCONNECT_NODE))
            {
                NodeComponent pSelectNode2 = null;
                foreach (NodeComponent pNode in _m_listNodes)
                {
                    if (pNode.InRange(pEvent.current.mousePosition))
                    {
                        pNode.Selected = true;
                        pSelectNode2 = pNode;
                        break;
                    }
                }
                if (pSelectNode2 != null && pEvent.MatchNode(pNode => pSelectNode2 == pNode) == null)
                {
                    pEvent.AddNode(pSelectNode2);
                }
                return;
            }

            NodeComponent pSelectNode = null;
            foreach (NodeComponent pNode in _m_listNodes)
            {
                if (pNode.InRange(pEvent.current.mousePosition))
                {
                    pNode.Selected = true;
                    pSelectNode = pNode;
                    break;
                }
            }
            if (pSelectNode != null)
            {
                var pMatchNode = pEvent.MatchNode(pNode => pNode == pSelectNode);
                if (pMatchNode == null)
                {
                    pEvent.ForEachNodes(pNode => {
                        pNode.Selected = false;
                    });
                    pEvent.ClearNodes();
                    pEvent.AddNode(pSelectNode);
                }
                else
                {

                }
            }
            else
            {
                pEvent.ForEachNodes((NodeComponent pNode) => {
                    pNode.Selected = false;
                });
                pEvent.ClearNodes();
            }

            GUI.changed = true;
        }
    
        private void Event_MouseRightDown(InputEvent pEvent)
        {
            if (pEvent.NodeCount == 1)
            {
                if (pEvent.MatchNode(pNode => pNode.InRange(pEvent.current.mousePosition)) != null)
                {
                    GUI_ContextMenu_OnNode(pEvent);
                }
                else
                {
                    pEvent.ForEachNodes((NodeComponent pNode) => {
                        pNode.Selected = false;
                    });
                    pEvent.ClearNodes();
                    
                    GUI_ContextMenu_EmptyPlace(pEvent);

                    GUI.changed = true;
                }
            }
            else if (pEvent.NodeCount == 0)
            {
                pEvent.m_pState.ClearState(InputState.OP_CONNECT_NODE | InputState.OP_DISCONNECT_NODE);
                GUI_ContextMenu_EmptyPlace(pEvent);
            }
            else
            {

            }
        }

        private void Event_MouseLeftDrag(InputEvent pEvent)
        {
            NodeComponent pNode = null;
            if (pEvent.NodeCount > 0)
            {
                pNode = pEvent.MatchNode(pNode => {
                    return pNode.InRange(pEvent.current.mousePosition);
                });
            }
            if (pNode != null)
            {
                pEvent.ForEachNodes(pNode => {
                    pNode.Drag(pEvent.current.delta);
                });
            }
            else
            {
                // pEvent.ForEachNodes(pNode => {
                //     pNode.Selected = false;
                // });
                // pEvent.ClearNodes();
                _m_v2GridOffset += pEvent.current.delta;
                for (int i = _m_listNodes.Count-1; i >= 0; i--)
                {
                    _m_listNodes[i].Drag(pEvent.current.delta);

                    // GUI.changed = true;
                    // pEvent.current.Use();
                    // break;
                }
            }

            GUI.changed = true;
            pEvent.current.Use();
        }

        private void Event_MouseLeftUp(InputEvent pEvent)
        {
            if (pEvent.NodeCount == 2)
            {
                if (pEvent.m_pState.InState(InputState.OP_CONNECT_NODE))
                {
                    pEvent.GetNode(0).Connect(pEvent.GetNode(1));
                    pEvent.ForEachNodes((NodeComponent pNode) => {
                        pNode.Selected = false;
                    });
                    pEvent.ClearNodes();

                    pEvent.m_pState.ClearState(InputState.OP_CONNECT_NODE);
                }
                else if (pEvent.m_pState.InState(InputState.OP_DISCONNECT_NODE))
                {
                    pEvent.GetNode(0).Disconnect(pEvent.GetNode(1));
                    pEvent.ForEachNodes((NodeComponent pNode) => {
                        pNode.Selected = false;
                    });
                    pEvent.ClearNodes();

                    pEvent.m_pState.ClearState(InputState.OP_DISCONNECT_NODE);
                }
                else
                {

                }
            }
            else if (pEvent.NodeCount == 1)
            {
                if (_m_pInspectorWindow != null)
                {
                    _m_pInspectorWindow.RefreshData(pEvent.GetNode(0));
                }
            }

            GUI.changed = true;
            pEvent.current.Use();
        }
    
        public void MenuMouseRight_AddNode(object args)
        {
            _m_listNodes.Add(new NodeComponent(_m_listNodes.Count+1, ((InputEvent)args).current.mousePosition));
        }
        public void MenuMouseRight_RemoveNode(object args)
        {
            var pEvent = (InputEvent)args;
            if (pEvent.NodeCount == 1)
            {
                pEvent.GetNode(0).DisconnectAll();
                pEvent.ForEachNodes((NodeComponent pNode) => {
                    pNode.Selected = false;
                });
                var pNode = pEvent.GetNode(0);
                pEvent.ClearNodes();

                _m_listNodes.Remove(pNode);
            }
        }
        public void MenuMouseRight_ConnectNode(object args)
        {
            var pEvent = (InputEvent)args;
            pEvent.m_pState.ClearState(InputState.OP_DISCONNECT_NODE);
            pEvent.m_pState.SetState(InputState.OP_CONNECT_NODE);
        }
        public void MenuMouseRight_DisconnectNode(object args)
        {
            var pEvent = (InputEvent)args;
            pEvent.m_pState.ClearState(InputState.OP_CONNECT_NODE);
            pEvent.m_pState.SetState(InputState.OP_DISCONNECT_NODE);
        }

        public void MenuMouseRight_Inspector(object args)
        {
            var pEvent = (InputEvent)args;
            _m_pInspectorWindow = NodeInspectorWindow.OpenNodeInspector(pEvent.GetNode(0));
        }

        public void GUI_ContextMenu_EmptyPlace(InputEvent pEvent)
        {
            GenericMenu pMenu = new GenericMenu();
            pMenu.AddItem(new GUIContent("Add Node"), false, MenuMouseRight_AddNode, pEvent);
            pMenu.ShowAsContext();
        }
        public void GUI_ContextMenu_OnNode(InputEvent pEvent)
        {
            GenericMenu pMenu = new GenericMenu();
            pMenu.AddItem(new GUIContent("Inspector"), false, MenuMouseRight_Inspector, pEvent);
            pMenu.AddItem(new GUIContent("Remove Node"), false, MenuMouseRight_RemoveNode, pEvent);
            pMenu.AddSeparator("");
            pMenu.AddItem(new GUIContent("Add Connection"), false, MenuMouseRight_ConnectNode, pEvent);
            pMenu.AddItem(new GUIContent("Remove Connection"), false, MenuMouseRight_DisconnectNode, pEvent);
            pMenu.ShowAsContext(); 
        }
    }
}
