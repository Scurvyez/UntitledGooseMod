using System.Collections.Generic;
using RimWorld;
using UntitledGooseMod.Defs;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.ThingComponents
{
    public class Comp_GooseBehaviorInterceptor : ThingComp
    {
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption option in base.CompFloatMenuOptions(selPawn))
            {
                yield return option;
            }
            
            if (selPawn.Faction != Faction.OfPlayer 
                || parent is not Pawn goose 
                || goose.kindDef != UGMDefOf.Goose) 
                yield break;
            
            if (goose.CurJobDef == UGMDefOf.UGM_GooseHaulNearby
                || goose.CurJobDef == UGMDefOf.UGM_GooseChaseChildren)
            {
                yield return new FloatMenuOption(
                    "UGM_ShooAwayGoose".Translate(goose.NameShortColored),
                    delegate
                    {
                        Job shooJob = JobMaker.MakeJob(UGMDefOf.UGM_ShooGooseAway, goose);
                        selPawn.jobs.TryTakeOrderedJob(shooJob);
                    },
                    MenuOptionPriority.Default,
                    null,
                    goose
                );
            }
        }
    }
    
    public class CompProperties_GooseBehaviorInterceptor : CompProperties
    {
        public CompProperties_GooseBehaviorInterceptor() => 
            compClass = typeof(Comp_GooseBehaviorInterceptor);
    }
}