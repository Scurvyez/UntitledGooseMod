using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using UntitledGooseMod.Defs;
using UntitledGooseMod.Settings;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace UntitledGooseMod.JobDrivers
{
    public class JobDriver_ShooGooseAway : JobDriver
    {
        private const TargetIndex TargetGooseInd = TargetIndex.A;
        private float _shooSuccessChance;
        
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(TargetGooseInd), job, 
                1, -1, null, errorOnFailed);
        }
        
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetGooseInd);
            this.FailOnAggroMentalStateAndHostile(TargetGooseInd);
            
            yield return Toils_Goto.GotoThing(TargetGooseInd, PathEndMode.Touch)
                .FailOnDespawnedNullOrForbidden(TargetGooseInd)
                .FailOnSomeonePhysicallyInteracting(TargetGooseInd);
            
            Pawn goose = job.GetTarget(TargetGooseInd).Pawn;
            
            Toil shooGooseToil = new()
            {
                initAction = () =>
                {
                    if (goose == null 
                        || goose.DestroyedOrNull() 
                        || goose.DeadOrDowned)
                        return;
                    
                    if (goose.CurJobDef != UGMDefOf.UGM_GooseHaulNearby
                        && goose.CurJobDef != UGMDefOf.UGM_GooseChaseChildren)
                        return;
                    
                    string moteText = RandomShooText();
                    MoteMaker.ThrowText((pawn.DrawPos + goose.DrawPos) / 2f, 
                        goose.Map, moteText, 5f);
                    
                    _shooSuccessChance = UGMSettings.ShooGooseAwaySuccessChance;
                    if (Rand.Chance(_shooSuccessChance) && _shooSuccessChance < 1.0f)
                    {
                        goose.jobs.StopAll();
                        
                        if (pawn.skills == null)
                            return;
                        
                        int pawnsSkillLevel = Mathf.Clamp(pawn.skills
                            .GetSkill(SkillDefOf.Animals).Level, 0, 20);
                        float secondaryChance = pawnsSkillLevel / 20f;
                        
                        if (Rand.Chance(secondaryChance))
                        {
                            goose.mindState.mentalStateHandler.TryStartMentalState(
                                UGMDefOf.UGM_SpookedGoose, null, true);
                        }
                        
                        UGMDefOf.Pawn_Goose_Call.PlayOneShot(pawn);
                    }
                    else if (Mathf.Approximately(_shooSuccessChance, 1.0f))
                    {
                        goose.mindState.mentalStateHandler.TryStartMentalState(
                            UGMDefOf.UGM_SpookedGoose, null, true);
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return shooGooseToil;
        }
        
        private static string RandomShooText()
        {
            string[] shooTexts =
            [
                "Shoo!", 
                "Get outta here!", 
                "Bad goose!", 
                "Scram!", 
                "Go on, git!", 
                "No geese allowed!", 
                "Shoo, featherbrain!",
                "Stop that!",
                "Beat it, honker!"
            ];
            return shooTexts.RandomElement();
        }
    }
}