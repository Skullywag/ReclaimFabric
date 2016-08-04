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
        internal static IEnumerable<Thing> _SmeltProducts(this Thing _this, float efficiency)
        {
            var costListAdj = _this.CostListAdjusted();
            if (!costListAdj.NullOrEmpty())
            {
                foreach (var thingCost in costListAdj)
                {
                    if (!thingCost.thingDef.intricate)
                    {
                        var twentyFivePercentLimit = (float)thingCost.count * 0.25f;
                        var randomUpToTwentyFivePercent = GenMath.RoundRandom(twentyFivePercentLimit);
                        if (randomUpToTwentyFivePercent > 0)
                        {
                            var resultantSmeltedThing = ThingMaker.MakeThing(thingCost.thingDef, null);
                            resultantSmeltedThing.stackCount = randomUpToTwentyFivePercent;
                            yield return resultantSmeltedThing;
                        }
                    }
                }
            }
            if (!_this.def.smeltProducts.NullOrEmpty())
            {
                foreach (var smeltProduct in _this.def.smeltProducts)
                {
                    var resultantSmeltedThing = ThingMaker.MakeThing(smeltProduct.thingDef, null);
                    resultantSmeltedThing.stackCount = smeltProduct.count;
                    yield return resultantSmeltedThing;
                }
            }

            if (_this.def.IsClothes())
            {
                Pawn crafter = _this.Position.GetEdifice().InteractionCell.GetFirstPawn();
                float skillPerc = ((float)crafter.skills.GetSkill(SkillDefOf.Crafting).level / 20);
                float num = Mathf.Lerp(0.5f, 1.5f, skillPerc);
                float healthPerc = ((float)_this.HitPoints / (float)_this.MaxHitPoints);
                float num1 = Mathf.Lerp(0f, 0.4f, healthPerc);
                var mainSmeltProductCount = (_this.def.costStuffCount * num1 / _this.Stuff.VolumePerUnit) * num;
                var adjustedCount = GenMath.RoundRandom(mainSmeltProductCount);
                //float healthPerc = ((float)_this.HitPoints / (float)_this.MaxHitPoints);
                //float num1 = Mathf.Lerp(0f, 0.4f, healthPerc);
                //var mainSmeltProductCount = (_this.def.costStuffCount * num1 / _this.Stuff.VolumePerUnit) * efficiency;
                //var adjustedCount = GenMath.RoundRandom(mainSmeltProductCount);
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
