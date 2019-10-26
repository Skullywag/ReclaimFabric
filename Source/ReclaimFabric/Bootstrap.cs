using System.Reflection;
using Verse;

namespace ReclaimFabric
{
    public class Bootstrap : Def
    {
        static Bootstrap()
        {
            {
                var method3 = typeof(Thing).GetMethod("SmeltProducts", BindingFlags.Instance | BindingFlags.Public);
                var method4 = typeof(_Thing_ReclaimFabric).GetMethod("_SmeltProducts", BindingFlags.Static | BindingFlags.NonPublic);
                Log.Message("Attempting detour from " + method3 + "to " + method4);
                if (!Detours.TryDetourFromTo(method3, method4))
                {
                    Log.Error("Reclaim Fabric Detour failed");
                    return;
                }
                Log.Message("Reclaim Fabric Detour Successful");
            }

            Assembly.Load("Assembly-CSharp.dll");
        }
    }
}