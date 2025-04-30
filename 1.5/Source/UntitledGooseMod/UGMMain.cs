using System;
using UntitledGooseMod.Debugging;
using Verse;

namespace UntitledGooseMod
{
    [StaticConstructorOnStartup]
    public static class UGMMain
    {
        static UGMMain()
        {
            UGMLog.Message($"{DateTime.Now.Date.ToShortDateString()} " +
                           $"[1.5 Release | Nothing to report.] " +
                           $"Beware the geese...");
        }
    }
}