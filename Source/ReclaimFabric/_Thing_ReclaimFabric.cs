﻿using RimWorld;
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
    public static class _Thing_ReclaimFabric
    {
        internal static IEnumerable<Thing> _SmeltProducts(this Thing _this, float efficiency)
        {
            var costListAdj = _this.CostListAdjusted();
            if (_this.def.IsClothes() || _this.def.IsAdvancedArmor() || _this.def.IsArmor())
            {
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
            else if (!costListAdj.NullOrEmpty())
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
            else if (!_this.def.smeltProducts.NullOrEmpty())
            {
                foreach (var smeltProduct in _this.def.smeltProducts)
                {
                    var resultantSmeltedThing = ThingMaker.MakeThing(smeltProduct.thingDef, null);
                    resultantSmeltedThing.stackCount = smeltProduct.count;
                    yield return resultantSmeltedThing;
                }
            }
        }
    }
}
