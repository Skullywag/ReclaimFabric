using Verse;

namespace ReclaimFabric
{
    public class SpecialThingFilterWorker_ReclaimEnergyShields : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            return AlwaysMatches(t.def);
        }

        public override bool AlwaysMatches(ThingDef def)
        {
            return def.IsEnergyShield();
        }
    }
}
