using UnityEngine;
using Verse;

namespace ResizeCharacterInfo
{
    public class MyModSettings : ModSettings
    {
        public float bioHeightOffset = 0f;
        public string bioHeightOffsetBuf;
        public float needsHeightOffset = 0f;
        public string needsHeightOffsetBuf;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref bioHeightOffset, "bioHeightOffset", 0f);
            Scribe_Values.Look(ref needsHeightOffset, "needsHeightOffset", 0f);
            base.ExposeData();
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.TextFieldNumericLabeled(
                "ResizeCharacterInfo.BioHeightOffset".Translate(),
                ref bioHeightOffset, ref bioHeightOffsetBuf, 0f, 7680f);
            listingStandard.TextFieldNumericLabeled(
                "ResizeCharacterInfo.NeedsHeightOffset".Translate(),
                ref needsHeightOffset, ref needsHeightOffsetBuf, 0f, 7680f);
            listingStandard.End();
        }
    }
}
