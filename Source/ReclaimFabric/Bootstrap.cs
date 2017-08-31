using System;
using System.Reflection;
using Verse;
using RimWorld;
using Harmony;

namespace ReclaimFabric
{
    [StaticConstructorOnStartup]
    class Bootstrap
    {
        static Bootstrap()
        {
            var harmony = HarmonyInstance.Create("com.github.pardeike.reclaimfabric");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}