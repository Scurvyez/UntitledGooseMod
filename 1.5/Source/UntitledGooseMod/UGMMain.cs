using UntitledGooseMod.Debugging;
using Verse;

namespace UntitledGooseMod
{
    [StaticConstructorOnStartup]
    public static class UGMMain
    {
        static UGMMain()
        {
            UGMLog.Message($"Nothing to report. Beware the geese...");
        }
    }
}