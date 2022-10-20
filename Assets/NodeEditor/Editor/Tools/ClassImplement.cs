using Sys_Activator = System.Activator;

namespace NodeEditor.Tools
{
    public class Activator
    {
        public static System.Object CreateInstance(System.Type pType)
        {
            try
            {
                return Sys_Activator.CreateInstance(pType);
            }
            catch
            {
                return null;
            }
        }
        public static System.Object CreateInstance(string strType)
        {
            try
            {
                return Sys_Activator.CreateInstance(System.Type.GetType(strType));
            }
            catch
            {
                return null;
            }
        }
    }
}
