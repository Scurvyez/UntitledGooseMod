using RimWorld;
using UnityEngine;
using UntitledGooseMod.Defs;
using Verse;
using Verse.Sound;
using Rand = Verse.Rand;

namespace UntitledGooseMod.JobDrivers
{
    public static class JobDriverUtils
    {
        public static void TryPlayWaddleAnimation(Pawn pawn, AnimationDef animationDef)
        {
            if (ModsConfig.IsActive("com.yayo.yayoani.continued"))
                return;
            
            Building_Door door = pawn.pather?.nextCell.GetDoor(pawn.Map);
            if (door == null)
            {
                if (pawn.Drawer?.renderer?.CurAnimation != animationDef)
                {
                    pawn.Drawer?.renderer?.SetAnimation(animationDef);
                }
            }
            else
            {
                pawn.Drawer?.renderer?.SetAnimation(null);
            }
        }
        
        public static void TryDoHonkEffect(Pawn pawn, ref Mote mote)
        {
            if (mote is { Destroyed: false })
                return;
            
            mote = MoteMaker.MakeAttachedOverlay(pawn, 
                UGMDefOf.UGM_Mote_TyrannicalAnimal, Vector3.zero);
            
            if (Rand.Bool)
                UGMDefOf.Pawn_Goose_Call.PlayOneShot(pawn);
            else
                UGMDefOf.Pawn_Goose_Angry.PlayOneShot(pawn);
        }
        
        public static string RandomShooText()
        {
            string[] shooTexts =
            [
                "UGM_ShooSuccess1".Translate(), 
                "UGM_ShooSuccess2".Translate(), 
                "UGM_ShooSuccess3".Translate(), 
                "UGM_ShooSuccess4".Translate(), 
                "UGM_ShooSuccess5".Translate(), 
                "UGM_ShooSuccess6".Translate(), 
                "UGM_ShooSuccess7".Translate(),
                "UGM_ShooSuccess8".Translate(),
                "UGM_ShooSuccess9".Translate()
            ];
            return shooTexts.RandomElement();
        }
    }
}