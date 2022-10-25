using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditor.Config
{
    public class PathConfig
    {
        public static string ms_ResourcePath = "Assets/NodeEditor/Resources/";
        public static string ms_DataPath = "Assets/NodeEditor/Editor/Data/";
        public static string ms_ExportPath = Application.dataPath + "/NodeEditor/Editor/Export/";

        public static string ms_CSharpSuffix = ".cs";

        public static string ms_GraphSuffix = "_Graph.ui";
        public static string ms_DataSuffix = "_Data.data";
    }
}
