using RimWorld;
using UnityEngine;
using UntitledGooseMod.Defs;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.MentalStates
{
    public class MentalState_SpookedAnimal : MentalState
    {
        private const int GooseFleeDistance = 15;
        
        public override void MentalStateTick()
        {
            if (pawn.DeadOrDowned)
            {
                RecoverFromState();
                return;
            }
            
            if (!pawn.IsHashIntervalTick(60))
                return;
            
            if (pawn.CurJob != null 
                && pawn.CurJobDef == UGMDefOf.UGM_SpookedFleeShort) 
                return;
            
            if (!CellFinder.TryFindRandomReachableNearbyCell(
                    pawn.Position, pawn.Map, GooseFleeDistance,
                    TraverseParms.For(pawn),
                    c => c.Standable(pawn.Map) && !c.Fogged(pawn.Map),
                    null, out IntVec3 fleeDest)) 
                return;
            
            Job fleeJob = JobMaker.MakeJob(UGMDefOf.UGM_SpookedFleeShort, fleeDest);
            pawn.jobs.StartJob(fleeJob, JobCondition.InterruptForced);
            
            MoteMaker.MakeAttachedOverlay(pawn, 
                ThingDefOf.Mote_ColonistFleeing, Vector3.zero);
        }
    }
}