using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditor.Tools;
using NodeEditor.Data;

public class Test_NodeData : DataBase
{
    [ScriptField]
    public int a = 1;
    [ScriptField]
    public int b = 20;
    [ScriptField]
    public string c = "hello";
    [ScriptField]
    public string d = "world";

    [ScriptField]
    public Vector3 v3 = Vector3.zero;
    [ScriptField]
    public Vector2 v2 = Vector2.zero;
    [ScriptField]
    public Vector4 v4 = Vector4.zero;

    public override object Serializer()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("a = ").Append(a).AppendLine();
        sb.Append("b = ").Append(b).AppendLine();
        sb.Append("c = ").Append(c).AppendLine();
        sb.Append("d = ").Append(d).AppendLine();
        sb.Append("v3 = ").Append(v3).AppendLine();
        sb.Append("v2 = ").Append(v2);
        return sb;
    }
    public override void Deserializer(object pData)
    {
        System.Text.StringBuilder sb = (System.Text.StringBuilder)pData;
        var strLines = sb.ToString().Split('\n');
        foreach (var strLine in strLines)
        {
            var strKV = strLine.Split('=');
            if (strKV.Length == 2)
            {
                var strKey = strKV[0].Trim();
                var strValue = strKV[1].Trim();
                switch (strKey)
                {
                    case "a":
                        a = int.Parse(strValue);
                        break;
                    case "b":
                        b = int.Parse(strValue);
                        break;
                    case "c":
                        c = strValue;
                        break;
                    case "d":
                        d = strValue;
                        break;
                    case "v3":
                        // v3 = new Vector3(strValue);
                        break;
                    case "v2":
                        // v2 = new Vector2(strValue);
                        break;
                }
            }
        }
    }
}
