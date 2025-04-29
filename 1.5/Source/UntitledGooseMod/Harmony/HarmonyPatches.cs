using System.Text;
using Verse;
using HarmonyLib;
using RimWorld;
using UntitledGooseMod.Debugging;
using UntitledGooseMod.Defs;
using UntitledGooseMod.Defs.DefModExtensions;
using UntitledGooseMod.MentalStates;

namespace UntitledGooseMod
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new (id: "rimworld.scurvyez.untitledgoosemod");
            
            harmony.Patch(original: AccessTools.Method(typeof(Building_Door), "PawnCanOpen"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(PawnCanOpenPostfix)));
        }
        
        public static void PawnCanOpenPostfix(Pawn p, ref bool __result)
        {
            if (p == null || p.kindDef != UGMDefOf.Goose) 
                return;

            if (p.MentalState is not (MentalState_MischievousAnimal
                or MentalState_TyrannicalAnimal
                or MentalState_SpookedAnimal)) 
                return;
            
            __result = true;;
        }
    }
}