using UnityEngine;
using Verse;

namespace ResizeCharacterInfo
{
    public class MyModSettings : ModSettings
    {
        public const float DefaultBioLeftHeight = 355f;
        public float bioLeftHeight = DefaultBioLeftHeight;
        public string bioLeftHeightBuf;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref bioLeftHeight, "height", DefaultBioLeftHeight);
            base.ExposeData();
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.TextFieldNumericLabeled(
                "ResizeCharacterInfo.BioLeftHeight".Translate(),
                ref bioLeftHeight, ref bioLeftHeightBuf, 0f, 7680f);
            listingStandard.End();
        }
    }
}
