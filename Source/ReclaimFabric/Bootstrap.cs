using System;
using System.Reflection;
using Verse;
using RimWorld;

namespace ReclaimFabric
{
    class Bootstrap : Def
    {
        private const BindingFlags UniversalBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        static Bootstrap()
        {
            {
                MethodInfo method3 = typeof(Thing).GetMethod("SmeltProducts", BindingFlags.Instance | BindingFlags.Public);
                MethodInfo method4 = typeof(_Thing_ReclaimFabric).GetMethod("_SmeltProducts", BindingFlags.Static | BindingFlags.NonPublic);
                Log.Message("Attempting detour from " + method3 + "to " + method4);
                if (!Detours.TryDetourFromTo(method3, method4))
                {
                    Log.Error("Reclaim Fabric Detour failed");
                    return;
                }
                Log.Message("Reclaim Fabric Detour Successful");
            }

            Assembly Assembly_CSharp = Assembly.Load("Assembly-CSharp.dll");
        }
    }
}