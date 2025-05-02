using RimWorld;
using Verse;

namespace UntitledGooseMod.Defs
{
    [DefOf]
    public class UGMDefOf
    {
        public static PawnKindDef Goose;
        public static MentalStateDef UGM_MischievousGoose;
        public static MentalStateDef UGM_TyrannicalGoose;
        public static MentalStateDef UGM_SpookedGoose;
        public static JobDef UGM_GooseHaulNearby;
        public static JobDef UGM_GooseChaseChildren;
        public static JobDef UGM_ShooGooseAway;
        public static JobDef UGM_SpookedFleeShort;
        public static SoundDef Pawn_Goose_Call;
        public static SoundDef Pawn_Goose_Angry;
        public static ThingDef UGM_Mote_TyrannicalAnimal;
        public static AnimationDef UGM_MischievousWaddleAnimation;
        public static HediffDef PsychicInvisibility;
        
        static UGMDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(UGMDefOf));
        }
    }
}