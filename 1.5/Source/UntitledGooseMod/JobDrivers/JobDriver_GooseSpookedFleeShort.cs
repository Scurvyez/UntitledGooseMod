using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.JobDrivers
{
    public class JobDriver_GooseSpookedFleeShort : JobDriver
    {
        private const TargetIndex DestCellInd = TargetIndex.A;
        
        private const int MinFleeInterval = 120;
        private const int MaxFleeInterval = 380;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(DestCellInd), job, 
                1, -1, null, errorOnFailed);
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(DestCellInd, PathEndMode.OnCell);

            Toil fleeToil = new()
            {
                initAction = () =>
                {
                    pawn.pather.StopDead();
                },
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = Rand.RangeInclusive(MinFleeInterval, MaxFleeInterval)
            };
            yield return fleeToil;
            
            Toil recoverToil = new()
            {
                initAction = () =>
                {
                    pawn.MentalState?.RecoverFromState();
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return recoverToil;
        }
    }
}