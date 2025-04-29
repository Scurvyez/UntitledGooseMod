using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.FinderUtils
{
    public static class GooseTargetFinder
    {
        public static bool IsThingValid(Thing thing, Pawn goose, 
            bool ignoreReachability = false)
        {
            if (thing == null || thing.Destroyed) return false;
            if (goose.carryTracker.CarriedThing == thing) return false;
            if (thing.Spawned)
            {
                if (goose.Map.reservationManager.IsReserved(thing))
                    return false;
                return ignoreReachability 
                       || goose.CanReach(thing, 
                           PathEndMode.ClosestTouch, Danger.Deadly);
            }
            
            if (thing.ParentHolder is Building_Storage 
                    { Spawned: not false } buildingStorage)
            {
                if (goose.Map.reservationManager.IsReserved(buildingStorage))
                    return false;
                return ignoreReachability 
                       || goose.CanReach(buildingStorage,
                           PathEndMode.InteractionCell, Danger.Deadly);
            }
            return false;
        }
        
        public static Thing TargetThing(Pawn goose)
        {
            if (!goose.Spawned) return null;
            
            Map map = goose.Map;
            IEnumerable<Thing> potentialTargets = map.listerThings.AllThings
                .Where(t => t.def is { category: ThingCategory.Item } 
                            && IsInAdequatePlayerStorage(t.Position, map)
                            && IsThingValid(t, goose, ignoreReachability: true)
                            && !t.Position.Fogged(map) 
                            && !t.IsBurning());
            List<Thing> list = potentialTargets.ToList();
            
            if (list.Count == 0) return null;
            List<(Thing thing, float weight)> weightedList = [];
            
            foreach (Thing thing in list)
            {
                float weight = GetThingWeight(thing);
                weightedList.Add((thing, weight));
            }
            
            float totalWeight = weightedList.Sum(entry => entry.weight);
            
            if (totalWeight <= 0f) return null;
            float choice = Rand.Range(0f, totalWeight);
            float cumulative = 0f;
            
            foreach ((Thing thing, float weight) entry in weightedList)
            {
                cumulative += entry.weight;
                if (choice <= cumulative)
                {
                    return entry.thing;
                }
            }
            return null;
        }
        
        private static float GetThingWeight(Thing thing)
        {
            if (ModsConfig.AnomalyActive)
            {
                if (thing.def == ThingDefOf.GoldenCube) return 10000f;
            }
            
            if (thing.def == ThingDefOf.Gold) return 1000f;
            if (thing.Stuff == ThingDefOf.Gold) return 75f;
            if (thing.Stuff == ThingDefOf.Silver) return 25f;
            return 10f;
        }
        
        public static bool HasEnoughTargets(Pawn goose, int minimumCount)
        {
            if (!goose.Spawned) return false;
            
            Map map = goose.Map;
            IEnumerable<Thing> potentialTargets = map.listerThings.AllThings
                .Where(t => t.def is { category: ThingCategory.Item }
                            && IsInAdequatePlayerStorage(t.Position, map)
                            && IsThingValid(t, goose, ignoreReachability: true)
                            && !t.Position.Fogged(map)
                            && !t.IsBurning());
            
            return potentialTargets.Take(minimumCount).Count() >= minimumCount;
        }
        
        private static bool IsInAdequatePlayerStorage(IntVec3 pos, Map map)
        {
            Zone zone = map.zoneManager.ZoneAt(pos);
            
            if (zone is Zone_Stockpile) return true;
            List<Thing> thingsInCell = pos.GetThingList(map);
            
            foreach (Thing thing in thingsInCell)
            {
                if (thing is Building_Storage) return true;
            }
            return false;
        }
        
        public static Pawn ClosestChildForMaximumTerror(Pawn goose)
        {
            List<Pawn> potentialGoblins = goose.Map?.mapPawns?.AllPawnsSpawned
                .Where(child =>
                    child.DevelopmentalStage.Child() 
                    && IsChildValid(child, goose, ignoreReachability: true))
                .ToList();
            
            return potentialGoblins.Any() 
                ? potentialGoblins.RandomElement() 
                : null;
        }
        
        public static bool IsChildValid(Pawn child, Pawn goose,
            bool ignoreReachability = false)
        {
            if (child == null || child.Destroyed || child.DeadOrDowned) 
                return false;
            
            if (child.Spawned)
            {
                return ignoreReachability 
                       || goose.CanReach(child, 
                           PathEndMode.Touch, Danger.Deadly);
            }
            return false;
        }
    }
}