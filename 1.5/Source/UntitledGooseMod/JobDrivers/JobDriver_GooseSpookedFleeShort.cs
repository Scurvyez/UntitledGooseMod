using System.Collections.Generic;
using UntitledGooseMod.Defs;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.JobDrivers
{
    public class JobDriver_GooseSpookedFleeShort : JobDriver
    {
        private const TargetIndex DestCellInd = TargetIndex.A;
        private const int MinFleeInterval = 120;
        private const int MaxFleeInterval = 380;
        
        private AnimationDef _mischiefWaddleAnimation;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(DestCellInd), job, 
                1, -1, null, errorOnFailed);
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            _mischiefWaddleAnimation = UGMDefOf.UGM_MischievousWaddleAnimation;
            
            yield return Toils_Goto.GotoCell(DestCellInd, PathEndMode.OnCell);

            Toil fleeToil = new()
            {
                initAction = () =>
                {
                    pawn.pather.StopDead();
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = Rand.RangeInclusive(MinFleeInterval, MaxFleeInterval),
                tickAction = () =>
                {
                    JobDriverUtils.TryPlayWaddleAnimation(pawn, 
                        _mischiefWaddleAnimation);
                }
            };
            yield return fleeToil;
            
            Toil recoverToil = new()
            {
                initAction = () =>
                {
                    pawn.Drawer?.renderer?.SetAnimation(null);
                    pawn.MentalState?.RecoverFromState();
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return recoverToil;
        }
    }
}