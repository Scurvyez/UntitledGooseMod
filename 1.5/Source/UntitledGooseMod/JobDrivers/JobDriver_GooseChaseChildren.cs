using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using UntitledGooseMod.Defs;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace UntitledGooseMod.JobDrivers
{
    public class JobDriver_GooseChaseChildren : JobDriver
    {
        private const TargetIndex TargetChildInd = TargetIndex.A;
        private const int ChaseDistance = 5;
        private const int HonkInterval = 180;
        
        private Mote _aggroMote;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetChildInd), job, 
                1, -1, null, errorOnFailed);
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetChildInd);
            this.FailOnDowned(TargetChildInd);
            
            Pawn goose = pawn;
            Pawn child = job.GetTarget(TargetChildInd).Pawn;
            
            Toil chaseChild = new()
            {
                initAction = () =>
                {
                    if (_aggroMote == null || _aggroMote.Destroyed)
                    {
                        _aggroMote = MoteMaker.MakeAttachedOverlay(goose, 
                            UGMDefOf.UGM_Mote_TyrannicalAnimal, Vector3.zero);
                    }
                },
                tickAction = () =>
                {
                    if (child == null || child.DestroyedOrNull() || child.DeadOrDowned)
                    {
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }
                    
                    if (!goose.Position.InHorDistOf(child.Position, ChaseDistance) 
                        || !goose.Position.WithinRegions(child.Position, Map,
                            2, TraverseParms.For(goose)))
                    {
                        if (!goose.CanReach(child, PathEndMode.Touch, Danger.Deadly))
                        {
                            EndJobWith(JobCondition.Incompletable);
                        }
                        else if (!goose.pather.Moving || goose.pather.Destination != child)
                        {
                            goose.pather.StartPath(child, PathEndMode.Touch);
                        }
                    }
                    
                    if (goose.Position.InHorDistOf(child.Position, ChaseDistance))
                    {
                        Job childCurJob = child.CurJob;
                        if (childCurJob == null || childCurJob.def != JobDefOf.Flee)
                        {
                            Job fleeJob = FleeUtility.FleeJob(child, goose, ChaseDistance + 1);
                            child.jobs?.StartJob(fleeJob, JobCondition.InterruptForced);
                        }
                        
                        if (!goose.IsHashIntervalTick(HonkInterval)) 
                            return;
                        
                        if (Rand.Chance(0.5f))
                        {
                            UGMDefOf.Pawn_Goose_Call.PlayOneShot(pawn);
                        }
                        else
                        {
                            UGMDefOf.Pawn_Goose_Angry.PlayOneShot(pawn);
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Never
            };
            yield return chaseChild;
        }
    }
}