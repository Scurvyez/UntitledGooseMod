using RimWorld;
using UntitledGooseMod.FinderUtils;
using UntitledGooseMod.Settings;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.MentalStates
{
    public class MentalState_TyrannicalAnimal : MentalState
    {
        public Pawn TargetChild;
        
        private const int CheckInterval = 500;
        
        public override string InspectLine => string
            .Format(def.baseInspectLine, TargetChild.NameShortColored);
        
        public override void MentalStateTick()
        {
            if (!UGMSettings.AllowTyrannicalGeese)
            {
                RecoverFromState();
            }
            
            bool recoveredThisTick = false;
            if (pawn.IsHashIntervalTick(CheckInterval))
            {
                if (pawn.Map?.dangerWatcher?.DangerRating == StoryDanger.High)
                {
                    RecoverFromState();
                    recoveredThisTick = true;
                }
                
                if (TargetChild == null || !GooseTargetFinder
                        .IsChildValid(TargetChild, pawn))
                {
                    TargetChild = GooseTargetFinder
                        .ClosestChildForMaximumTerror(pawn);
                    if (TargetChild == null)
                    {
                        RecoverFromState();
                        recoveredThisTick = true;
                    }
                }
            }
            if (!recoveredThisTick)
            {
                base.MentalStateTick();
            }
        }
        
        public override void PostStart(string reason)
        {
            base.PostStart(reason);
            TargetChild = GooseTargetFinder.ClosestChildForMaximumTerror(pawn);
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref TargetChild, "TargetChild");
        }
    }
}