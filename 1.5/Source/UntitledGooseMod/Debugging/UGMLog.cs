using UnityEngine;
using Verse;

namespace UntitledGooseMod.Debugging
{
    public class UGMLog
    {
        public static Color ErrorMsgCol = new Color(0.4f, 0.54902f, 1.0f);
        public static Color WarningMsgCol = new Color(0.70196f, 0.4f, 1.0f);
        public static Color MessageMsgCol = new Color(0.4f, 1.0f, 0.54902f);
        
        public static void Error(string msg)
        {
            Log.Error("[Untitled Goose Mod] "
                .Colorize(ErrorMsgCol) + msg);
        }
        
        public static void Warning(string msg)
        {
            Log.Warning("[Untitled Goose Mod] "
                .Colorize(WarningMsgCol) + msg);
        }
        
        public static void Message(string msg)
        {
            Log.Message("[Untitled Goose Mod] "
                .Colorize(MessageMsgCol) + msg);
        }
    }
}