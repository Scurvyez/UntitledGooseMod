using System;
using UnityEngine;
using Verse;

namespace UntitledGooseMod.Settings
{
    public class UGMMod : Mod
    {
        public static UGMMod Mod;
        
        private UGMSettings _settings;
        private float _halfWidth;
        private Vector2 _leftScrollPos = Vector2.zero;
        
        private const float NewSectionGap = 6f;
        private const float Spacing = 10f;
        private const float SliderSpacing = 120f;
        private const float LabelWidth = 200f;
        private const float TextFieldWidth = 100f;
        private const float ElementHeight = 25f;
        
        public override string SettingsCategory() => "UGM_ModName".Translate();
        
        public UGMMod(ModContentPack content) : base(content)
        {
            Mod = this;
            _settings = GetSettings<UGMSettings>();
        }
        
        public override void DoSettingsWindowContents(Rect inRect)
        {
            _halfWidth = (inRect.width - 30) / 2;
            LeftSideScrollViewHandler(new Rect(inRect.x, inRect.y, 
                _halfWidth, inRect.height));
        }
        
        private void LeftSideScrollViewHandler(Rect inRect)
        {
            Listing_Standard list1 = new();
            list1.Gap(200f);
            Rect viewRect1 = new(inRect.x, inRect.y, inRect.width, inRect.height);
            Rect vROffset1 = new(0, 0, inRect.width - 20, inRect.height);
            
            Widgets.BeginScrollView(viewRect1, ref _leftScrollPos, vROffset1);
            list1.Begin(vROffset1);
            list1.Gap(NewSectionGap);
            
            list1.CheckboxLabeled("UGM_AllowMischievousGeese".Translate(), 
                ref _settings._allowMischievousGeese, 
                "UGM_AllowMischievousGeeseDesc".Translate());
            list1.Gap(Spacing);
            
            if (ModsConfig.BiotechActive)
            {
                list1.CheckboxLabeled("UGM_AllowTyrannicalGeese".Translate(), 
                    ref _settings._allowTyrannicalGeese, 
                    "UGM_AllowTyrannicalGeeseDesc".Translate());
                list1.Gap(Spacing);
            }
            
            DrawSettingWithSliderAndTextField(vROffset1, list1, 
                "UGM_ShooGooseAwaySuccessChance".Translate(), 
                "UGM_ShooGooseAwaySuccessChanceDesc".Translate(), 
                ref _settings._shooGooseAwaySuccessChance, 
                0f, 1f);
            list1.Gap(Spacing);
            
            list1.End();
            Widgets.EndScrollView();
        }
        
        private static void DrawSettingWithSliderAndTextField<T>(Rect inRect, 
            Listing_Standard list, string labelText, string tooltipText, 
            ref T settingValue, T minValue, T maxValue) where T : struct, IConvertible
        {
            float sliderWidth = inRect.width - SliderSpacing;
            float settingFloat = Convert.ToSingle(settingValue);
            float minFloat = Convert.ToSingle(minValue);
            float maxFloat = Convert.ToSingle(maxValue);
            
            Rect labelRect = new(0, list.CurHeight, LabelWidth, ElementHeight);
            Widgets.Label(labelRect, labelText);
            TooltipHandler.TipRegion(labelRect, tooltipText);
            
            Rect textFieldRect = new(sliderWidth - Spacing, 
                list.CurHeight, TextFieldWidth, ElementHeight);
            
            string textValue = typeof(T) == typeof(int) ? settingFloat
                .ToString("F0") : settingFloat.ToString("F2");
            
            Widgets.TextFieldNumeric(textFieldRect, ref settingFloat, 
                ref textValue, minFloat, maxFloat);
            
            list.Gap(Spacing * 1.75f);
            
            if (typeof(T) == typeof(int) || typeof(T) == typeof(float))
            {
                Rect sliderRect = new(0, list.CurHeight, 
                    sliderWidth + TextFieldWidth + Spacing, ElementHeight);
                
                float sliderValue = settingFloat;
                sliderValue = Widgets.HorizontalSlider(sliderRect, 
                    sliderValue, minFloat, maxFloat, true);
                settingFloat = sliderValue;
                settingValue = (T)Convert.ChangeType(settingFloat, typeof(T));
            }
            list.Gap(Spacing);
            list.Gap(30.00f);
        }
    }
}