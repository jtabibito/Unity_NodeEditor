using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NodeEditor.Component;
using NodeEditor.Tools;

namespace NodeEditor.Data
{
    public enum DataAssetType
    {
        UI,
        Script
    }

    public class DataAssetLabel
    {
        public const string mc_LabelID = "ID:";
        public const string mc_LabelDataType = "DataType:";

        public const string mc_LabelStart = "#Start";
        public const string mc_LabelEnd = "#End";

        public const string mc_LabelDataStart = "##DataStart";
        public const string mc_LabelDataEnd = "##DataEnd";

        public const string mc_LabelInNodes = "In:";
        public const string mc_LabelOutNodes = "Out:";

        public const string mc_LabelGraphData = "Graph:";

        public const string mc_ErrorDataType = "None";
    }

    public class DataAsset
    {
        private List<int> _m_listHasOperated;
        private bool _m_bComplete;

        public string m_strDataSource;

        public DataAsset()
        {
            _m_listHasOperated = new List<int>(8);
            _m_bComplete = true;
        }

        public Task<List<NodeComponent>> LoadGraph()
        {
            Task<List<NodeComponent>> t = new Task<List<NodeComponent>>(() => {
                List<NodeComponent> listNodes = new List<NodeComponent>(8);
                using (FileStream fs = new FileStream(Config.PathConfig.ms_ExportPath + m_strDataSource + "_Graph.ui", FileMode.OpenOrCreate, FileAccess.Read))
                using (StreamReader sr = new StreamReader(fs))
                {
                    try
                    {
                        CustomData pCustomData = new CustomData();
                        NodeComponent pNode = null;
                        StringBuilder sb = new StringBuilder();
                        int state = 0;
                        while (!sr.EndOfStream)
                        {
                            string strLine = sr.ReadLine();
                            if (strLine == DataAssetLabel.mc_LabelStart)
                            {
                                pNode = new NodeComponent();
                            }
                            else if (strLine == DataAssetLabel.mc_LabelEnd)
                            {
                                listNodes.Add(pNode);
                                pNode = null;
                            }
                            else if (pNode != null)
                            {
                                int index;
                                if ((index = strLine.IndexOf(DataAssetLabel.mc_LabelDataEnd)) != -1)
                                {
                                    pNode.Deserializer(sb);
                                    state = 0;
                                    sb.Clear();
                                }
                                else if ((index = strLine.IndexOf(DataAssetLabel.mc_LabelDataStart)) != -1)
                                {
                                    state = 0x01;
                                    sb.Clear();
                                }
                                else if ((index = strLine.IndexOf(DataAssetLabel.mc_LabelInNodes)) != -1)
                                {
                                    int length = DataAssetLabel.mc_LabelInNodes.Length;
                                    var inNodes = strLine.Substring(index + length, strLine.Length - index - length).Split('|');
                                    pCustomData.Set(pNode.UUID + "_In", new List<int>(8));
                                    for (int i = 0; i < inNodes.Length; i++)
                                    {
                                        if (int.TryParse(inNodes[i], out int id))
                                        {
                                            pCustomData.Get<List<int>>(pNode.UUID + "_In").Add(id);
                                        }
                                    }
                                }
                                else if ((index = strLine.IndexOf(DataAssetLabel.mc_LabelOutNodes)) != -1)
                                {
                                    int length = DataAssetLabel.mc_LabelOutNodes.Length;
                                    var outNodes = strLine.Substring(index + length, strLine.Length - index - length).Split('|');
                                    pCustomData.Set(pNode.UUID + "_Out", new List<int>(8));
                                    for (int i = 0; i < outNodes.Length; i++)
                                    {
                                        if (int.TryParse(outNodes[i], out int id))
                                        {
                                            pCustomData.Get<List<int>>(pNode.UUID + "_Out").Add(id);
                                        }
                                    }
                                }
                                else if (state == 0)
                                {
                                    if ((index = strLine.IndexOf(DataAssetLabel.mc_LabelID)) != -1)
                                    {
                                        int length = DataAssetLabel.mc_LabelID.Length;
                                        pNode.SetID(int.Parse(strLine.Substring(index + length, strLine.Length - index - length)));
                                    }
                                }
                                else if (state == 0x01)
                                {
                                    sb.AppendLine(strLine);
                                }
                            }
                        }
                        for (int i = 0; i < listNodes.Count; i++)
                        {
                            var node = listNodes[i];
                            if (pCustomData.Get<List<int>>(node.UUID + "_In") != null)
                            {
                                var listConnectNodes = pCustomData.Get<List<int>>(node.UUID + "_In");
                                for (int j = 0; j < listConnectNodes.Count; j++)
                                {
                                    int id = listConnectNodes[j];
                                    var inNode = listNodes.Find((NodeComponent n) => { return n.ID == id; });
                                    if (inNode != null)
                                    {
                                        node.Connect(inNode, node);
                                    }
                                }
                            }
                            if (pCustomData.Get<List<int>>(node.UUID + "_Out") != null)
                            {
                                var listConnectNodes = pCustomData.Get<List<int>>(node.UUID + "_Out");
                                for (int j = 0; j < listConnectNodes.Count; j++)
                                {
                                    int id = listConnectNodes[j];
                                    var outNode = listNodes.Find((NodeComponent n) => { return n.ID == id; });
                                    if (outNode != null)
                                    {
                                        node.Connect(node, outNode);
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        UnityEngine.Debug.LogError(ex);
                        listNodes.Clear();
                    }
                }
                if (listNodes.Count > 0)
                {
                    listNodes[0].SetRoot();
                }
                return listNodes;
            });
            return t;
        }
        public Task<Dictionary<int, DataBase>> LoadData()
        {
            Task<Dictionary<int, DataBase>> t = new Task<Dictionary<int, DataBase>>(() => {
                Dictionary<int, DataBase> dictData = new Dictionary<int, DataBase>(8);
                using (FileStream fs = new FileStream(Config.PathConfig.ms_ExportPath + m_strDataSource + "_Data.data", FileMode.OpenOrCreate, FileAccess.Read))
                using (StreamReader sr = new StreamReader(fs))
                {
                    try
                    {
                        DataBase pData = null;
                        StringBuilder sb = new StringBuilder();
                        int state = 0;
                        while (!sr.EndOfStream)
                        {
                            string strLine = sr.ReadLine();
                            if (strLine == DataAssetLabel.mc_LabelStart)
                            {
                            }
                            else if (strLine.IndexOf(DataAssetLabel.mc_LabelDataType) != -1)
                            {
                                int index = strLine.IndexOf(DataAssetLabel.mc_LabelDataType);
                                int length = DataAssetLabel.mc_LabelDataType.Length;
                                string strDataType = strLine.Substring(index + length, strLine.Length - index - length);
                                if (strDataType != DataAssetLabel.mc_ErrorDataType)
                                {
                                    pData = Activator.CreateInstance(strDataType) as DataBase;
                                }
                            }
                            else if (strLine == DataAssetLabel.mc_LabelEnd)
                            {
                                if (pData != null)
                                {
                                    dictData[pData.ID] = pData;
                                    pData = null;
                                }
                            }
                            else if (pData != null)
                            {
                                int index;
                                if ((index = strLine.IndexOf(DataAssetLabel.mc_LabelDataEnd)) != -1)
                                {
                                    pData.Deserializer(sb);
                                    state = 0;
                                    sb.Clear();
                                }
                                else if ((index = strLine.IndexOf(DataAssetLabel.mc_LabelDataStart)) != -1)
                                {
                                    state = 0x01;
                                    sb.Clear();
                                }
                                else if (state == 0)
                                {
                                    if ((index = strLine.IndexOf(DataAssetLabel.mc_LabelID)) != -1)
                                    {
                                        int length = DataAssetLabel.mc_LabelID.Length;
                                        pData.ID = int.Parse(strLine.Substring(index + length, strLine.Length - index - length));
                                    }
                                }
                                else if (state == 0x01)
                                {
                                    sb.AppendLine(strLine);
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        UnityEngine.Debug.LogError(ex);
                        dictData.Clear();
                    }
                }
                return dictData;
            });
            return t;
        }
        public async Task<List<NodeComponent>> Load()
        {
            var t1 = LoadGraph();
            var t2 = LoadData();

            t1.Start();
            t2.Start();
            var listNodes = await t1;
            var dictData = await t2;

            foreach (var pNode in listNodes)
            {
               if (dictData.TryGetValue(pNode.ID, out pNode.m_pData))
               {
                   pNode.m_pScript = UnityEditor.EditorGUIUtility.Load(Config.PathConfig.ms_DataPath + pNode.m_pData.GetType() + ".cs") as UnityEditor.MonoScript;
               }
            }
            return listNodes;
        }
        private Task SaveGraph(List<NodeComponent> data)
        {
            Task t = new Task(() => {
                using (FileStream fs = new FileStream(Config.PathConfig.ms_ExportPath + m_strDataSource + "_Graph.ui", FileMode.Create, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var pNode in data)
                    {
                        sb.AppendLine(DataAssetLabel.mc_LabelStart);
                        sb.AppendLine(DataAssetLabel.mc_LabelID + pNode.ID);
                        sb.AppendLine(DataAssetLabel.mc_LabelDataStart);
                        sb.Append(pNode.Serializer());
                        sb.AppendLine();
                        sb.AppendLine(DataAssetLabel.mc_LabelDataEnd);
                        sb.Append(DataAssetLabel.mc_LabelInNodes);
                        int i = 0;
                        foreach (var pIn in pNode.GetInNodes())
                        {
                            ++i;
                            sb.Append(pIn.ID);
                            sb.Append("|");
                        }
                        if (i > 0)
                        {
                            sb.Remove(sb.Length - 1, 1);
                        }
                        sb.AppendLine();
                        sb.Append(DataAssetLabel.mc_LabelOutNodes);
                        i = 0;
                        foreach (var pOut in pNode.GetOutNodes())
                        {
                            ++i;
                            sb.Append(pOut.ID);
                            sb.Append("|");
                        }
                        if (i > 0)
                        {
                            sb.Remove(sb.Length - 1, 1);
                        }
                        sb.AppendLine();
                        sb.AppendLine(DataAssetLabel.mc_LabelEnd);
                    }
                    // sw.Write(sb);

                    List<NodeComponent> listCloseMap = new List<NodeComponent>(8);
                    string dfs(NodeComponent node)
                    {
                        if (node.GetOutNodes().Count == 0)
                        {
                            return node.ID.ToString();
                        }
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            int i = 0;
                            foreach (var pNode in node.GetOutNodes())
                            {
                                ++i;
                                if (!listCloseMap.Contains(pNode))
                                {
                                    listCloseMap.Add(pNode);
                                    sb.Append(dfs(pNode) + ',');
                                }
                                else
                                {
                                    sb.Append($"{pNode.ID},");
                                }
                            }
                            if (i > 0 && sb.Length > 1)
                            {
                                sb.Remove(sb.Length - 1, 1);
                            }
                            return node.ID.ToString() + '(' + sb.ToString() + ')';
                        }
                    }
                    var graphData = '(' + dfs(data[0]) + ')';
                    sb.AppendLine(DataAssetLabel.mc_LabelGraphData + graphData);
                    sw.Write(sb);
                }
            });
            return t;
        }
        private Task SaveData(DataBase[] pSerializer)
        {
            Task t = new Task(() => {
                using FileStream fs = new FileStream(Config.PathConfig.ms_ExportPath + m_strDataSource + "_Data.data", FileMode.Create, FileAccess.Write);
                using StreamWriter sw = new StreamWriter(fs);
                StringBuilder sb = new StringBuilder();
                foreach (var pData in pSerializer)
                {
                    sb.AppendLine(DataAssetLabel.mc_LabelStart);
                    if (pData != null)
                    {
                        sb.AppendLine(DataAssetLabel.mc_LabelDataType + pData.GetType());
                        sb.AppendLine(DataAssetLabel.mc_LabelID + pData.ID);
                        sb.AppendLine(DataAssetLabel.mc_LabelDataStart);
                        sb.Append(pData.Serializer());
                        sb.AppendLine();
                        sb.AppendLine(DataAssetLabel.mc_LabelDataEnd);
                    }
                    else
                    {
                        sb.AppendLine(DataAssetLabel.mc_LabelDataType + DataAssetLabel.mc_ErrorDataType);
                        sb.AppendLine(DataAssetLabel.mc_LabelID + "-1");
                        sb.AppendLine(DataAssetLabel.mc_LabelDataStart);
                        sb.AppendLine(DataAssetLabel.mc_LabelDataEnd);
                    }
                    sb.AppendLine(DataAssetLabel.mc_LabelEnd);
                }
                sw.Write(sb);
            });
            return t;
        }
        public async void Save(List<NodeComponent> data)
        {
            List<DataBase> pSerializer = new List<DataBase>(8);
            foreach (var pNode in data)
            {
                pSerializer.Add(pNode.m_pData);
            }

            var t1 = SaveGraph(data);
            var t2 = SaveData(pSerializer.ToArray());

            t1.Start();
            t2.Start();
            await t1;
            await t2;
        }
    }
}