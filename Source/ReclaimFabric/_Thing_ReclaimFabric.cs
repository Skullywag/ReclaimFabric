using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse.AI;
using Verse;
using Harmony;

namespace ReclaimFabric
{
    [HarmonyPatch(typeof(Thing), "SmeltProducts", new Type[] { typeof(float) })]
    public class _Thing_ReclaimFabric
    {
        static void Postfix(Thing __instance, IEnumerable<Thing> __result)
        {
            if (__instance.def.IsClothes() || __instance.def.IsAdvancedArmor() || __instance.def.IsArmor())
            {
                var result = _SmeltProducts(__instance).ToArray();
                Log.Message($"Intercepting {__instance?.def?.defName} with {string.Join(", ", result.Select(x => $"{x?.TryGetQuality(out QualityCategory qc) } {x?.def?.defName}").ToArray())}");
                __result = result;
            }
        }

        static IEnumerable<Thing> _SmeltProducts(Thing _this)
        {
            var costListAdj = _this.CostListAdjusted();
            Pawn crafter = _this.Position.GetEdifice(_this.Map).InteractionCell.GetFirstPawn(_this.Map);
            float skillPerc = ((float)crafter.skills.GetSkill(SkillDefOf.Crafting).Level / 20);
            float num = Mathf.Lerp(0.5f, 1.5f, skillPerc);
            float healthPerc = ((float)_this.HitPoints / (float)_this.MaxHitPoints);
            float num1 = Mathf.Lerp(0f, 0.4f, healthPerc);
            foreach (var thingCost in costListAdj)
            {
                if (!thingCost.thingDef.intricate)
                {
                    var PercentMaxLimit = (float)thingCost.count * num1;
                    var mainSmeltProductCount = (PercentMaxLimit * num);
                    if (mainSmeltProductCount > 0)
                    {
                        var resultantSmeltedThing = ThingMaker.MakeThing(thingCost.thingDef, null);
                        resultantSmeltedThing.stackCount = (int)mainSmeltProductCount;
                        yield return resultantSmeltedThing;
                    }
                }
            }
        }
    }
}
