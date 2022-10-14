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

    public void Test()
    {
        int e = 100;
        int f = 200;
        string g = "hello";
        string h = "world";
    }
}
