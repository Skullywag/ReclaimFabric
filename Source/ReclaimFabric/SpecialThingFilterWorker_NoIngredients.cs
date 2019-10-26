using Verse;

namespace ReclaimFabric
{
    public class SpecialThingFilterWorker_NoIngredients : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            return this.AlwaysMatches(t.def);
        }

        public override bool AlwaysMatches(ThingDef def)
        {
            return def.costList == null && def.stuffCategories == null;
        }
    }
}
