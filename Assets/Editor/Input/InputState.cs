using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditor.Input
{
    public class InputState
    {
        public const uint OP_CONNECT_NODE = 0x00000100;
        public const uint OP_DISCONNECT_NODE = 0x00000200;

        private uint _m_uiOP_STATE;
        public uint OP_STATE
        {
            get => _m_uiOP_STATE;
        }

        public void SetState(uint op)
        {
            _m_uiOP_STATE |= op;
        }
        public void ClearState(uint op)
        {
            _m_uiOP_STATE &= ~op;
        }
        public bool InState(uint op)
        {
            return (_m_uiOP_STATE&op) != 0;
        }
    }
}
