using Verse;

namespace UntitledGooseMod.Settings
{
    public class UGMSettings : ModSettings
    {
        private static UGMSettings _instance;
        
        public UGMSettings()
        {
            _instance = this;
        }
        
        public static bool AllowMischievousGeese => _instance._allowMischievousGeese;
        public static bool AllowTyrannicalGeese => _instance._allowTyrannicalGeese;
        public static float ShooGooseAwaySuccessChance => _instance._shooGooseAwaySuccessChance;

        public bool _allowMischievousGeese = true;
        public bool _allowTyrannicalGeese = true;
        public float _shooGooseAwaySuccessChance = 0.5f;
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _allowMischievousGeese, "_allowMischievousGeese", true);
            Scribe_Values.Look(ref _allowTyrannicalGeese, "_allowTyrannicalGeese", true);
            Scribe_Values.Look(ref _shooGooseAwaySuccessChance, "_shooGooseAwaySuccessChance", 0.5f);
        }
    }
}