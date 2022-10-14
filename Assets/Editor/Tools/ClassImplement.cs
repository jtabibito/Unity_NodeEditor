using Sys_Activator = System.Activator;

namespace NodeEditor.Tools
{
    public class Activator
    {
        public static System.Object CreateInstance(System.Type pType)
        {
            return Sys_Activator.CreateInstance(pType);
        }
    }
}
