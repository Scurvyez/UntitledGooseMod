using UntitledGooseMod.Defs;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.MentalStateWorkers
{
    public class MentalStateWorker_SpookedAnimal : MentalStateWorker
    {
        public override bool StateCanOccur(Pawn pawn)
        {
            if (!base.StateCanOccur(pawn))
                return false;
            
            if (pawn.kindDef != UGMDefOf.Goose 
                || pawn.MentalState?.def == UGMDefOf.UGM_SpookedGoose)
                return false;
            
            return pawn.Spawned && !pawn.DeadOrDowned;
        }
    }
}