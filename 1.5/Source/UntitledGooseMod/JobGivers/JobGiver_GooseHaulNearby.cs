using RimWorld;
using UntitledGooseMod.Defs;
using UntitledGooseMod.Defs.DefModExtensions;
using UntitledGooseMod.MentalStates;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.JobGivers
{
    public class JobGiver_GooseHaulNearby : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.kindDef != UGMDefOf.Goose)
                return null;
            
            if (pawn.MentalState is not MentalState_MischievousAnimal 
                    { TargetThing: var targetThing })
                return null;
            
            if (targetThing is not { Spawned: true } 
                || pawn.Map.reservationManager.IsReserved(targetThing)
                || !pawn.CanReach(targetThing, PathEndMode.ClosestTouch, Danger.Deadly))
                return null;
            
            ModExtension_GooseUnhindered ext = pawn.def
                .GetModExtension<ModExtension_GooseUnhindered>();
            if (ext == null) 
                return null;
            
            if (!RCellFinder.TryFindRandomCellNearWith(
                    pawn.Position,
                    c => c.InBounds(pawn.Map)
                         && !pawn.Map.reservationManager.IsReserved(c)
                         && pawn.CanReach(pawn.Position, c, 
                             PathEndMode.ClosestTouch, Danger.Deadly, true,
                             true, TraverseMode.PassDoors)
                         && !c.Fogged(pawn.Map)
                         && c.Standable(pawn.Map),
                    pawn.Map,
                    out IntVec3 destCell,
                    ext.minCarryDist,
                    ext.maxCarryDist))
            {
                return null;
            }
            
            Job job = JobMaker.MakeJob(UGMDefOf.UGM_GooseHaulNearby, targetThing, destCell);
            job.count = 1;
            return job;
        }
    }
}