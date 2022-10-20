using System;
using System.Collections;
using System.Collections.Generic;

namespace NodeEditor.Data
{
    public class CustomData
    {
        private Dictionary<Type, Dictionary<string, object>> _m_pData;

        public CustomData()
        {
            _m_pData = new Dictionary<Type, Dictionary<string, object>>(8);
        }
        public void Set<T>(string strKey, T pData)
        {
            if (!_m_pData.ContainsKey(typeof(T)))
            {
                _m_pData.Add(typeof(T), new Dictionary<string, object>(8));
            }
            _m_pData[typeof(T)][strKey] = pData;
        }
        public void Get<T>(string strKey, out T pData)
        {
            pData = default;
            if (_m_pData.ContainsKey(typeof(T)))
            {
                if (_m_pData[typeof(T)].ContainsKey(strKey))
                {
                    pData = (T)_m_pData[typeof(T)][strKey];
                }
            }
        }
        public T Get<T>(string strKey)
        {
            if (_m_pData.ContainsKey(typeof(T)))
            {
                if (_m_pData[typeof(T)].ContainsKey(strKey))
                {
                    return (T)_m_pData[typeof(T)][strKey];
                }
            }
            return default;
        }
        public void Remove<T>(string strKey)
        {
            if (_m_pData.ContainsKey(typeof(T)))
            {
                if (_m_pData[typeof(T)].ContainsKey(strKey))
                {
                    _m_pData[typeof(T)].Remove(strKey);
                }
            }
        }
    }

    public abstract class DataBase
    {
        public int ID;

        public abstract object Serializer();
        public abstract void Deserializer(object pData);
    }
}