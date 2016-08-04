using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace ReclaimFabric
{
    
    public static class Helper
    {

        public static bool                  IsClothes( this ThingDef def )
        {
            return(
                ( !def.menuHidden )&&
                (
                    ( def.thingClass == typeof( Apparel ) )||
                    ( def.thingClass.IsSubclassOf( typeof( Apparel ) ) )
                )&&
                ( !def.apparel.tags.NullOrEmpty() )&&
                (
                    ( !def.apparel.tags.Contains( "Military" ) )&&
                    ( !def.apparel.tags.Contains( "PersonalShield" ) )
                )
            );
        }

        public static bool                  IsAdvancedArmor( this ThingDef def )
        {
            return(
                ( !def.menuHidden )&&
                (
                    ( def.thingClass == typeof( Apparel ) )||
                    ( def.thingClass.IsSubclassOf( typeof( Apparel ) ) )
                )&&
                ( !def.apparel.tags.NullOrEmpty() )&&
                (
                    ( def.apparel.tags.Contains( "Military" ) )&&
                    ( def.apparel.tags.Contains( "Spacer" ) )
                )
            );
        }

        public static bool                  IsArmor( this ThingDef def )
        {
            return(
                ( !def.menuHidden )&&
                (
                    ( def.thingClass == typeof( Apparel ) )||
                    ( def.thingClass.IsSubclassOf( typeof( Apparel ) ) )
                )&&
                ( !def.apparel.tags.NullOrEmpty() )&&
                (
                    ( def.apparel.tags.Contains( "Military" ) )&&
                    ( !def.apparel.tags.Contains( "Spacer" ) )
                )
            );
        }

        public static bool                  IsEnergyWeapon( this ThingDef def )
        {
            return(
                ( !def.menuHidden )&&
                ( !def.weaponTags.NullOrEmpty() )&&
                (
                    ( def.weaponTags.Contains( "AdvancedGun" ) )||
                    ( def.weaponTags.Contains( "GrenadeEMP" ) )
                )
            );
        }

        public static bool                  IsGunpowderWeapon( this ThingDef def )
        {
            return(
                ( !def.menuHidden )&&
                ( !def.weaponTags.NullOrEmpty() )&&
                (
                    ( def.weaponTags.Contains( "Gun" ) )||
                    ( def.weaponTags.Contains( "GunHeavy" ) )||
                    ( def.weaponTags.Contains( "GrenadeDestructive" ) )
                )&&
                ( !def.weaponTags.Contains( "AdvancedGun" ) )
            );
        }

        public static bool                  IsMeleeWeapon( this ThingDef def )
        {
            return(
                ( !def.menuHidden )&&
                ( !def.weaponTags.NullOrEmpty() )&&
                (
                    ( def.weaponTags.Contains( "Melee" ) )||
                    ( def.weaponTags.Contains( "NeolithicMelee" ) )
                )
            );
        }

        public static bool                  IsRangedWeapon( this ThingDef def )
        {
            return(
                ( !def.menuHidden )&&
                ( !def.weaponTags.NullOrEmpty() )&&
                (
                    ( def.weaponTags.Contains( "NeolithicRanged" ) )
                )
            );
        }

        public static bool                  IsEnergyShield( this ThingDef def )
        {
            return(
                ( !def.menuHidden )&&
                (
                    ( def.thingClass == typeof( Apparel ) )||
                    ( def.thingClass.IsSubclassOf( typeof( Apparel ) ) )
                )&&
                ( !def.apparel.tags.NullOrEmpty() )&&
                (
                    ( def.apparel.tags.Contains( "PersonalShield" ) )
                )
            );
        }

        public static ThingDef              MendWithDef( this Thing thing )
        {
            if( thing.Stuff != null )
            {
                // Use stuff
                return thing.Stuff;
            }
            if( thing.def.costList != null )
            {
                // Use most abundant ingredient requirement
                var repairWith = (ThingDef) null;
                int num = 0;
                foreach( var thingCount in thing.def.costList )
                {
                    if( thingCount.count > num )
                    {
                        repairWith = thingCount.thingDef;
                        num = thingCount.count;
                    }
                }
                return repairWith;
            }
            // Can't make this item, take a "best guess"
            // TODO: Search recipes for ones with products of the thing to mend
            if( thing.def.IsClothes() )
            {
                if(
                    ( !thing.def.apparel.tags.NullOrEmpty() )&&
                    ( thing.def.apparel.tags.Contains( "Spacer" ) )
                )
                {
                    return ThingDef.Named( "Synthread" );
                }
                return ThingDefOf.Cloth;
            }
            if(
                ( thing.def.IsAdvancedArmor() )||
                ( thing.def.IsEnergyShield() )||
                ( thing.def.IsEnergyWeapon() )
            )
            {
                return ThingDefOf.Plasteel;
            }
            return ThingDefOf.Steel;
        }

        public static int                   MendCountMax( this Thing thing )
        {
            float baseCount = 0;
            if( thing.Stuff != null )
            {
                // Use stuff
                baseCount = thing.def.costStuffCount;
                if( thing.def.costList != null )
                {
                    // Add any cost list based ingredient which is the same as stuff
                    foreach( var thingCount in thing.def.costList )
                    {
                        if( thingCount.thingDef == thing.Stuff )
                        {
                            baseCount += thingCount.count;
                        }
                    }
                }
            }
            else
            {
                if( thing.def.costList != null )
                {
                    // Use most abundant ingredient requirement
                    foreach( var thingCount in thing.def.costList )
                    {
                        if( thingCount.count > baseCount )
                        {
                            baseCount = thingCount.count;
                        }
                    }
                }
                else
                {
                    // No recipe maker for this item, take a "best guess"
                    // TODO: Search recipes for ones with products of the thing to mend
                    var repairWith = MendWithDef ( thing );
                    if( repairWith == null )
                    {
                        // No resource, 0 count
                        return 0;
                    }
                    // Take item base value
                    var marketValue = thing.def.BaseMarketValue;
                    // And the resource value
                    var valuePerResource = repairWith.BaseMarketValue;
                    // Get the amount of work using this stuff
                    var statValueAbstract = StatExtension.GetStatValueAbstract( thing.def, StatDefOf.WorkToMake, repairWith );
                    // Calculate labour value (see StatWorker_MarketValue.GetValueUnfinalized)
                    var labourValue = (float) Mathf.RoundToInt( statValueAbstract / 0.004f );
                    // Calculate base material cost
                    var materialCost = marketValue - labourValue;
                    // Calculcate resource count from material cost / cost per material
                    baseCount = Mathf.Max( 1, ( materialCost / valuePerResource ) );
                }
            }
            // Return resource count
            return (int) baseCount;
        }

        public static float                 MendCountPerHitPoint( this Thing thing )
        {
            float countMax = MendCountMax( thing ) * 10f;
            if( countMax < 1 )
            {
                return 0;
            }
            // Round to 0.1 (min) per hit point
            var perHP = countMax / (float) thing.MaxHitPoints;
            return Mathf.Max( 1, Mathf.RoundToInt( countMax / (float)thing.MaxHitPoints ) ) * 0.1f;
        }

        public static int                   MendCountRequired( this Thing thing )
        {
            // Repair amount
            var repairAmount = (float) thing.MaxHitPoints - (float) thing.HitPoints;
            // Resources per hit point
            var perHP = MendCountPerHitPoint( thing );
            // Return resource count per hitpoint * number of hitpoints to repair
            return (int) ( perHP * repairAmount );
        }

    }

}
