using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using UnityEngine;
using Verse.AI;

namespace ReclaimFabric
{
    public class RecipeWorker_ReclaimClothing : RecipeWorker
    {
        public RecipeDef recipe;

        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
        {
            var costListAdj = ingredient.CostListAdjusted();
            if (ingredient.def.IsClothes())
            {
                var resList = ingredient.Map.reservationManager.GetFieldViaReflection<List<object>>("Reservations");
                var match = resList.Find(o => o.GetFieldViaReflection<object>("target") == ingredient);
                Pawn crafter = match.GetFieldViaReflection<Pawn>("claimant");
                float skillPerc = ((float)crafter.skills.GetSkill(SkillDefOf.Crafting).Level / 20);
                float num = Mathf.Lerp(0.5f, 1.5f, skillPerc);
                float healthPerc = ((float)ingredient.HitPoints / (float)ingredient.MaxHitPoints);
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
            ingredient.Destroy(DestroyMode.Vanish);
        }
    }
}
