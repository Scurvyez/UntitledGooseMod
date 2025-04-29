using UntitledGooseMod.FinderUtils;
using UntitledGooseMod.Settings;
using Verse;
using Verse.AI;

namespace UntitledGooseMod.MentalStates
{
    public class MentalState_MischievousAnimal : MentalState
    {
        public Thing TargetThing;
        
        private const int CheckInterval = 500;
        
        public override void MentalStateTick()
        {
            if (!UGMSettings.AllowMischievousGeese)
            {
                RecoverFromState();
            }
            
            bool flag = false;
            if (pawn.IsHashIntervalTick(CheckInterval))
            {
                if (TargetThing == null || !GooseTargetFinder
                        .IsThingValid(TargetThing, pawn))
                {
                    TargetThing = GooseTargetFinder.TargetThing(pawn);
                    if (TargetThing == null)
                    {
                        RecoverFromState();
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                base.MentalStateTick();
            }
        }
        
        public override void PostStart(string reason)
        {
            base.PostStart(reason);
            TargetThing = GooseTargetFinder.TargetThing(pawn);
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref TargetThing, "TargetThing");
        }
    }
}