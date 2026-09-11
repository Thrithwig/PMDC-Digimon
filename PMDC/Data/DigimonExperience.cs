using System;
namespace PMDC.Data
{
    public static class DigimonExperience
    {
        // User's Cyber Sleuth formula, followed by an explicit anti-farming penalty.
        public static int Award(int baseExp, int defeatedLevel, int recipientLevel, string recipientGrowth = "digi_champion")
        {
            long reward = (long)baseExp * (defeatedLevel - 1) / 10 + baseExp;
            int gap = Math.Max(0, recipientLevel - defeatedLevel - 5);
            if (gap >= 10) return 0;
            // Stage group IDs remain stable even though all level-up curves are identical.
            int quarters = recipientGrowth switch
            {
                "digi_baby" => 8,
                "digi_in_training" => 8,
                "digi_rookie" => 6,
                "digi_ultimate" => 3,
                "digi_mega" => 2,
                _ => 4
            };
            return (int)(reward * quarters / (4L * (1L << gap)));
        }
    }
}
