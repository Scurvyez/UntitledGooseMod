using UntitledGooseMod.Debugging;
using UntitledGooseMod.Defs;
using UntitledGooseMod.Defs.DefModExtensions;
using UntitledGooseMod.FinderUtils;
using UntitledGooseMod.MentalStates;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.JobGivers
{
    public class JobGiver_GooseChaseChildren : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!ModsConfig.BiotechActive || pawn == null || pawn.Dead)
                return null;
            
            if (pawn.kindDef != UGMDefOf.Goose 
                || pawn.MentalState?.def == UGMDefOf.UGM_MischievousGoose)
                return null;
            
            if (pawn.MentalState is not MentalState_TyrannicalAnimal 
                    { TargetChild: var targetChild })
                return null;
            
            if (targetChild is not { Spawned: true } 
                || pawn.Map.reservationManager.IsReserved(targetChild))
                return null;
            
            if (!GooseTargetFinder.IsChildValid(targetChild, pawn))
                return null;
            
            ModExtension_GooseUnhindered ext = pawn.def
                .GetModExtension<ModExtension_GooseUnhindered>();
            if (ext == null)
                return null;
            
            Job job = JobMaker.MakeJob(UGMDefOf.UGM_GooseChaseChildren, targetChild);
            job.count = 1;
            return job;
        }
    }
}