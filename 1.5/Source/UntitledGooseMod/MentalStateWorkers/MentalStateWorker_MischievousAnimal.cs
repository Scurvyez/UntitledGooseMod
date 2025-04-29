using UntitledGooseMod.Defs;
using UntitledGooseMod.FinderUtils;
using UntitledGooseMod.Settings;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.MentalStateWorkers
{
    public class MentalStateWorker_MischievousAnimal : MentalStateWorker
    {
        public override bool StateCanOccur(Pawn pawn)
        {
            if (!base.StateCanOccur(pawn))
                return false;
            
            if (!UGMSettings.AllowMischievousGeese)
                return false;
            
            if (pawn.kindDef != UGMDefOf.Goose 
                || pawn.MentalState?.def == UGMDefOf.UGM_TyrannicalGoose)
                return false;
            
            return GooseTargetFinder.HasEnoughTargets(pawn, 2);
        }
    }
}