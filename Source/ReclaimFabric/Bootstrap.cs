using System.Collections.Generic;
using System.Reflection;
using Verse;
using Harmony;
using RimWorld;
using UnityEngine;

namespace ReclaimFabric
{
    [StaticConstructorOnStartup]
    class Main
    {
        // this static constructor runs to create a HarmonyInstance and install a patch.
        static Main()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("com.github.Skullywag.ReclaimFabric");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("SmeltProducts")]
    static class ReclaimFabric_Thing
    {
        static void Prefix(this Thing __instance, ref IEnumerable<Thing> __result, float efficiency)
        {
            __result = new List<Thing>(__result);
            if (__instance.def.IsClothes() ||
                __instance.def.IsAdvancedArmor() ||
                __instance.def.IsArmor())
            {
                // Assume Direct Control
                Pawn crafter = __instance.Position.GetEdifice(__instance.Map).InteractionCell.GetFirstPawn(__instance.Map);
                float skillPerc = (float)crafter.skills.GetSkill(SkillDefOf.Crafting).Level / 20;
                float num = Mathf.Lerp(0.5f, 1.5f, skillPerc);
                float healthPerc = (float)__instance.HitPoints / __instance.MaxHitPoints;
                float num1 = Mathf.Lerp(0f, 0.4f, healthPerc);
                
                List<ThingCountClass> costListAdj = __instance.CostListAdjusted();
                foreach (ThingCountClass thingCost in costListAdj)
                {
                    if (!thingCost.thingDef.intricate)
                    {
                        float PercentMaxLimit = thingCost.count * num1;
                        int mainSmeltProductCount = (int)(PercentMaxLimit * num);
                        if (mainSmeltProductCount > 0)
                        {
                            Thing resultantSmeltedThing = ThingMaker.MakeThing(thingCost.thingDef);
                            resultantSmeltedThing.stackCount = mainSmeltProductCount;
                            __result.Add(resultantSmeltedThing);
                        }
                    }
                }
            }
        }
    }
}