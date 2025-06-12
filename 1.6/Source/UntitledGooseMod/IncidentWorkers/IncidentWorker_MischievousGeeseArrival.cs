using System.Linq;
using RimWorld;
using UntitledGooseMod.Defs;
using UntitledGooseMod.Defs.DefModExtensions;
using UntitledGooseMod.MapComponents;
using UntitledGooseMod.Settings;
using Verse;

namespace UntitledGooseMod.IncidentWorkers
{
    public class IncidentWorker_MischievousGeeseArrival : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            
            if (!UGMSettings.AllowMischievousGeese 
                || map.dangerWatcher?.DangerRating == StoryDanger.High)
                return false;
            
            if (map.gameConditionManager
                .ConditionIsActive(GameConditionDefOf.ToxicFallout)) 
                return false;
            
            if (ModsConfig.BiotechActive && 
                map.gameConditionManager
                    .ConditionIsActive(GameConditionDefOf.NoxiousHaze)) 
                return false;
            
            if (map.mapPawns.AllPawnsSpawned
                    .Count(p => p.def == UGMDefOf.Goose.race) >= 2)
                return false;
            
            if (!map.mapTemperature
                    .SeasonAndOutdoorTemperatureAcceptableFor(UGMDefOf.Goose.race)) 
                return false;
            
            return TryFindBestEntryCell(map, out IntVec3 _);
        }
        
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!TryFindBestEntryCell(map, out IntVec3 cell))
                return false;
            
            PawnKindDef gooseKind = UGMDefOf.Goose;
            ModExtension_GooseUnhindered ext = gooseKind.race
                .GetModExtension<ModExtension_GooseUnhindered>();
            
            if (ext == null)
                return false;
            
            int numGeese = Rand.RangeInclusive(
                ext.incidentNumGeeseMin, ext.incidentNumGeeseMax);
            int exitAfterTicks = Rand.RangeInclusive(
                ext.incidentExitMapMinTick, ext.incidentExitMapMaxTick);
            
            MapComponent_MischievousGooseArrival mapComp = map
                .GetComponent<MapComponent_MischievousGooseArrival>();
            
            if (mapComp == null) 
                return false;
            
            mapComp.StartSpawningGeese(numGeese, cell, gooseKind, exitAfterTicks);
            return true;
        }
        
        private static bool TryFindBestEntryCell(Map map, out IntVec3 cell)
        {
            return RCellFinder.TryFindRandomPawnEntryCell(out cell, map, 
                CellFinder.EdgeRoadChance_Animal + 0.2f);
        }
    }
}