using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using UntitledGooseMod.Defs;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace UntitledGooseMod.JobDrivers
{
    public class JobDriver_GooseHaulNearby : JobDriver
    {
        private const TargetIndex TargetThingInd = TargetIndex.A;
        private const TargetIndex DestCellInd = TargetIndex.B;
        
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
            
            yield return Toils_Reserve.Reserve(TargetThingInd);
            yield return Toils_Reserve.Reserve(DestCellInd);
            yield return Toils_Goto.GotoThing(TargetThingInd, PathEndMode.ClosestTouch)
                .FailOnDespawnedOrNull(TargetThingInd);
            
            yield return Toils_General.Do(delegate
            {
                FleckMaker.Static(pawn.TrueCenter() , pawn.Map, FleckDefOf.MetaPuff, 2f);
                
                if (!Rand.Chance(0.25f)) return;
                UGMDefOf.Pawn_Goose_Call.PlayOneShot(pawn);
            });
            
            yield return Toils_Haul.StartCarryThing(TargetThingInd);
            
            Toil carryToil =  Toils_Haul.CarryHauledThingToCell(DestCellInd);
            carryToil.AddPreInitAction(() =>
            {
                if (job.GetTarget(DestCellInd).IsValid)
                {
                    // easy entry points for effects/anything else when carrying thing to cell
                    if (_mischiefMote == null || _mischiefMote.Destroyed)
                    {
                        _mischiefMote = MoteMaker.MakeAttachedOverlay(pawn, 
                            UGMDefOf.UGM_Mote_MischievousAnimal, Vector3.zero);
                    }
                }
            });
            yield return carryToil;
            
            Toil placeToil = Toils_Haul.PlaceHauledThingInCell(DestCellInd,
                null, storageMode: false);
            placeToil.AddPreInitAction(() =>
            {
                if (job.GetTarget(DestCellInd).IsValid)
                {
                    // easy entry points for effects/anything else when placing thing in cell
                    
                }
            });
            placeToil.AddFinishAction(() =>
            {
                Thing carriedThing = job.GetTarget(TargetThingInd).Thing;
                carriedThing.SetForbidden(true, false);
            });
            yield return placeToil;
        }
    }
}