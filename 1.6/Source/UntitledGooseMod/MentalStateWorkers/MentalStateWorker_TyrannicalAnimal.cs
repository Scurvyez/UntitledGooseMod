using RimWorld;
using UntitledGooseMod.Defs;
using UntitledGooseMod.FinderUtils;
using UntitledGooseMod.Settings;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.MentalStateWorkers
{
    public class MentalStateWorker_TyrannicalAnimal : MentalStateWorker
    {
        private Pawn _childToTerrorize;
        
        public override bool StateCanOccur(Pawn pawn)
        {
            if (!base.StateCanOccur(pawn))
                return false;
            
            if (!UGMSettings.AllowTyrannicalGeese)
                return false;
            
            if (pawn.kindDef != UGMDefOf.Goose 
                || pawn.MentalState?.def == UGMDefOf.UGM_MischievousGoose
                || pawn.Map.dangerWatcher.DangerRating == StoryDanger.High)
                return false;
            
            _childToTerrorize = GooseTargetFinder.ClosestChildForMaximumTerror(pawn);
            return _childToTerrorize != null;
        }
    }
}