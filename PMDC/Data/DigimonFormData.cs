using System;
using System.Collections.Generic;
using RogueEssence;
using RogueEssence.Data;
using RogueEssence.Dungeon;

namespace PMDC.Data
{
    /// <summary>Data-driven level curves, independent of the Pokemon base-stat formulas.</summary>
    [Serializable]
    public class DigimonFormData : MonsterFormData
    {
        public List<int[]> LevelStats = new List<int[]>();
        private int Column(Stat stat)
        {
            switch (stat)
            {
                case Stat.HP: return 0;
                case Stat.Attack: return 1;
                case Stat.Defense: return 2;
                case Stat.MAtk: return 3;
                case Stat.MDef: return 4;
                case Stat.Speed: return 5;
                default: throw new ArgumentOutOfRangeException(nameof(stat));
            }
        }
        public override int GetStat(int level, Stat stat, int bonus)
        {
            if (LevelStats.Count == 0) throw new InvalidOperationException("Missing Digimon level curve");
            int index = Math.Clamp(level - 1, 0, LevelStats.Count - 1);
            return Math.Max(1, LevelStats[index][Column(stat)] + Math.Clamp(bonus, 0, MAX_STAT_BOOST));
        }
        public override int GetMaxStat(Stat stat, int level) => GetStat(level, stat, MAX_STAT_BOOST);
        public override int ReverseGetStat(Stat stat, int value, int level) => Math.Max(0, value - GetStat(level, stat, 0));
    }
}
