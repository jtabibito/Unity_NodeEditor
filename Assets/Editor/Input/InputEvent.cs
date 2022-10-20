using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditor.Component;

namespace NodeEditor.Input
{
    public class InputEvent
    {
        public Event current;
        public InputState m_pState;

        private List<NodeComponent> _m_listNodes;

        public InputEvent()
        {
            m_pState = new InputState();
            _m_listNodes = new List<NodeComponent>(8);
        }

        public int NodeCount
        {
            get => _m_listNodes.Count;
        }
        public NodeComponent GetNode(int index)
        {
            return _m_listNodes[index];
        }
        public void AddNode(NodeComponent pNode)
        {
            _m_listNodes.Add(pNode);
        }
        public void RemoveNode(NodeComponent pNode)
        {
            _m_listNodes.Remove(pNode);
        }
        public NodeComponent MatchNode(Predicate<NodeComponent> pMatch)
        {
            return _m_listNodes.Find(pMatch);
        }
        public void ForEachNodes(Action<NodeComponent> pAction)
        {
            _m_listNodes.ForEach(pAction);
        }
        public void ClearNodes()
        {
            _m_listNodes.Clear();
        }
    }
}
