using System.Collections.Generic;
using RimWorld;
using UntitledGooseMod.Defs;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.JobDrivers
{
    public class JobDriver_GooseChaseChildren : JobDriver
    {
        private const TargetIndex TargetChildInd = TargetIndex.A;
        private const int ChaseDistance = 5;
        private const int HonkInterval = 180;
        
        private AnimationDef _mischiefWaddleAnimation;
        private Mote _mischiefMote;
        
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
            _mischiefWaddleAnimation = UGMDefOf.UGM_MischievousWaddleAnimation;
            
            Toil chaseChild = new()
            {
                initAction = () =>
                {
                    if (_mischiefMote is { Destroyed: false }) 
                        return;
                    
                    JobDriverUtils.TryDoHonkEffect(pawn, ref _mischiefMote);
                },
                tickAction = () =>
                {
                    if (child == null || child.DestroyedOrNull() || child.DeadOrDowned)
                    {
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }
                    
                    JobDriverUtils.TryPlayWaddleAnimation(pawn, 
                        _mischiefWaddleAnimation);
                    
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
                        
                        JobDriverUtils.TryDoHonkEffect(pawn, ref _mischiefMote);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Never
            };
            chaseChild.AddFinishAction(() =>
            {
                pawn.Drawer?.renderer?.SetAnimation(null);
            });
            yield return chaseChild;
        }
    }
}