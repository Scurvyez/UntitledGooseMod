using System.Collections.Generic;
using RimWorld;
using UntitledGooseMod.Defs;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.JobDrivers
{
    public class JobDriver_GooseHaulNearby : JobDriver
    {
        private const TargetIndex TargetThingInd = TargetIndex.A;
        private const TargetIndex DestCellInd = TargetIndex.B;
        
        private AnimationDef _mischiefWaddleAnimation;
        private Mote _mischiefMote;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetThingInd), job, 
                1, -1, null, errorOnFailed);
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetThingInd);
            this.FailOnBurningImmobile(DestCellInd);
            
            _mischiefWaddleAnimation = UGMDefOf.UGM_MischievousWaddleAnimation;
            
            yield return Toils_Reserve.Reserve(TargetThingInd);
            yield return Toils_Reserve.Reserve(DestCellInd);
            Toil gotoThingToil = Toils_Goto.GotoThing(TargetThingInd, 
                    PathEndMode.ClosestTouch).FailOnDespawnedOrNull(TargetThingInd);
            gotoThingToil.tickAction = () =>
            {
                JobDriverUtils.TryPlayWaddleAnimation(pawn, 
                    _mischiefWaddleAnimation);
            };
            yield return gotoThingToil;
            
            yield return Toils_Haul.StartCarryThing(TargetThingInd);
            
            Toil carryToil =  Toils_Haul.CarryHauledThingToCell(DestCellInd);
            carryToil.AddPreInitAction(() =>
            {
                if (job.GetTarget(DestCellInd).IsValid)
                {
                    JobDriverUtils.TryDoHonkEffect(pawn, ref _mischiefMote);
                }
            });
            carryToil.tickAction = () =>
            {
                JobDriverUtils.TryPlayWaddleAnimation(pawn, 
                    _mischiefWaddleAnimation);
            };
            carryToil.AddFinishAction(() =>
            {
                pawn.Drawer.renderer.SetAnimation(null);
            });
            yield return carryToil;
            
            Toil placeToil = Toils_Haul.PlaceHauledThingInCell(DestCellInd,
                null, storageMode: false);
            placeToil.AddFinishAction(() =>
            {
                Thing carriedThing = job.GetTarget(TargetThingInd).Thing;
                carriedThing.SetForbidden(true, false);
            });
            yield return placeToil;
        }
    }
}