using CommunityCoreLibrary;
using CommunityCoreLibrary.Controller;
using CommunityCoreLibrary.Detour;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Verse;

namespace ReclaimFabric
{
    public class DetourInjector : SpecialInjector
    {
        public override bool Inject()
        {
            MethodInfo method1 = typeof(Thing).GetMethod("SmeltProducts", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo method2 = typeof(_Thing).GetMethod("_SmeltProducts", BindingFlags.Static | BindingFlags.NonPublic);
            Detours.TryDetourFromTo(method1, method2);
            if (!Detours.TryDetourFromTo(method1, method2))
            {
                return false;
            }
            return true;
        }
    }
}
