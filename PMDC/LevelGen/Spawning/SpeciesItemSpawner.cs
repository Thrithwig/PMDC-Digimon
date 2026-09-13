using RogueElements;
using System;
using System.IO;
using System.Collections.Generic;
using RogueEssence.Dungeon;
using RogueEssence.LevelGen;
using RogueEssence;
using RogueEssence.Data;
using System.Xml;
using PMDC.Data;

namespace PMDC.LevelGen
{

    [Serializable]
    public abstract class SpeciesItemSpawner<TGenContext> : IStepSpawner<TGenContext, MapItem>
        where TGenContext : BaseMapGenContext
    {
        public SpeciesItemSpawner()
        {
        }

        public SpeciesItemSpawner(IntRange rarity, RandRange amount)
        {
            this.Rarity = rarity;
            this.Amount = amount;
        }

        public IntRange Rarity { get; set; }

        public RandRange Amount { get; set; }

        // The Digimon conversion intentionally has no family-exclusive items.  Older
        // special-room generators still use this spawner, so give those chests a
        // broadly useful single-use Digimon TM whenever their family reward table is
        // empty.  The normal rarity map always wins when a project supplies one.
        private static readonly string[][] DigimonTMFallbacks = new string[][]
        {
            new string[]
            {
                "digi_tm_burst_flame_i", "digi_tm_gaia_element_i", "digi_tm_gale_storm_i",
                "digi_tm_heaven_s_thunder_i", "digi_tm_holy_light_i", "digi_tm_hydro_water_i"
            },
            new string[]
            {
                "digi_tm_burst_flame_ii", "digi_tm_gaia_element_ii", "digi_tm_gale_storm_ii",
                "digi_tm_heaven_s_thunder_ii", "digi_tm_holy_light_ii", "digi_tm_hydro_water_ii"
            },
            new string[]
            {
                "digi_tm_burst_flame_iii", "digi_tm_gaia_element_iii", "digi_tm_gale_storm_iii",
                "digi_tm_heaven_s_thunder_iii", "digi_tm_holy_light_iii", "digi_tm_hydro_water_iii"
            }
        };

        public abstract IEnumerable<string> GetPossibleSpecies(TGenContext map);

        public List<MapItem> GetSpawns(TGenContext map)
        {
            int chosenAmount = Amount.Pick(map.Rand);

            RarityData rarity = DataManager.Instance.UniversalData.Get<RarityData>();
            List<string> possibleItems = new List<string>();
            foreach (string baseSpecies in GetPossibleSpecies(map))
            {
                for (int ii = Rarity.Min; ii < Rarity.Max; ii++)
                {
                    Dictionary<int, List<string>> rarityTable;
                    if (rarity.RarityMap.TryGetValue(baseSpecies, out rarityTable))
                    {
                        if (rarityTable.ContainsKey(ii))
                        {
                            foreach (string item in rarityTable[ii])
                            {
                                EntrySummary summary = DataManager.Instance.DataIndices[DataManager.DataType.Item].Get(item);
                                if (summary.Released)
                                    possibleItems.Add(item);
                            }
                        }
                    }
                }
            }

            if (possibleItems.Count == 0)
            {
                int tier = Math.Min(Math.Max(Rarity.Min - 1, 0), DigimonTMFallbacks.Length - 1);
                foreach (string item in DigimonTMFallbacks[tier])
                {
                    EntrySummary summary = DataManager.Instance.DataIndices[DataManager.DataType.Item].Get(item);
                    if (summary.Released)
                        possibleItems.Add(item);
                }
            }

            List<MapItem> results = new List<MapItem>();
            if (possibleItems.Count > 0)
            {
                for (int ii = 0; ii < chosenAmount; ii++)
                {
                    string chosenItem = possibleItems[map.Rand.Next(possibleItems.Count)];
                    results.Add(new MapItem(chosenItem));
                }
            }

            return results;
        }

        public override string ToString()
        {
            return string.Format("{0}: Rarity:{1} Amt:{2}", this.GetType().GetFormattedTypeName(), this.Rarity.ToString(), this.Amount.ToString());
        }
    }
}
