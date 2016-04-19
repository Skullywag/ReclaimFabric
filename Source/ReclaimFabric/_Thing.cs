using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse.AI;
using Verse;

namespace ReclaimFabric
{
    public static class _Thing
    {
        internal static IEnumerable<Thing> _SmeltProducts(this Thing _this, Pawn crafter, float efficiency)
        {
            if (_this.def.smeltProducts != null)
            {
                foreach (var counter in _this.def.smeltProducts)
                {
                    do
                    {
                        var thing = ThingMaker.MakeThing(counter.thingDef);
                        thing.stackCount = Mathf.Min(thing.def.stackLimit, counter.count);
                        counter.count -= thing.stackCount;
                        yield return thing;
                    } while (counter.count > 0);
                }
            }
            if (_this.def.costStuffCount >= 0)
            {
                float skillPerc = ((float)crafter.skills.GetSkill(SkillDefOf.Crafting).level / 20);
                float num = Mathf.Lerp(0.5f, 1.5f, skillPerc);
                float healthPerc = ((float)_this.HitPoints / (float)_this.MaxHitPoints);
                float num1 = Mathf.Lerp(0f, 0.4f, healthPerc);
                var mainSmeltProductCount = (_this.def.costStuffCount * num1 / _this.Stuff.VolumePerUnit) * num;
                var adjustedCount = GenMath.RoundRandom(mainSmeltProductCount);
                if (adjustedCount > 0)
                {
                    do
                    {
                        var thing = ThingMaker.MakeThing(_this.Stuff);
                        thing.stackCount = Mathf.Min(thing.def.stackLimit, adjustedCount);
                        adjustedCount -= thing.stackCount;
                        yield return thing;
                    } while (adjustedCount > 0);
                }
            }
        }
    }
}
