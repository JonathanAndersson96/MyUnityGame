using System;

namespace MyUnityGame.Battle
{
    public class Demon
    {
        public const int MinLevel = 1;
        public const int MaxLevel = 100;

        public DemonSpecies Species { get; private set; }
        public int Level { get; private set; }
        public InvestmentLedger Investments { get; }

        public Demon(DemonSpecies species, int level, InvestmentLedger investments = null)
        {
            Species = species != null ? species : throw new ArgumentNullException(nameof(species));
            if (level < MinLevel || level > MaxLevel)
            {
                throw new ArgumentOutOfRangeException(nameof(level));
            }

            Level = level;
            Investments = investments ?? new InvestmentLedger();
        }

        public DemonType Type => Species.Type;

        // Points from skipped levels (for example, when caught at a high level) land in the bank too.
        public int BankedPoints => InvestmentRules.PointsEarnedBy(Level) - Investments.PointsSpent;

        public int GetStat(StatKind stat, BattleTuning tuning) =>
            StatCalculator.Calculate(Species.BaseStats[stat], Investments.ShiftFor(stat), Level, tuning);

        public InvestmentResult TryInvest(StatKind stat) =>
            Investments.TryInvest(stat, Species.BaseStats, BankedPoints);

        public void LevelUpTo(int level)
        {
            if (level < Level || level > MaxLevel)
            {
                throw new ArgumentOutOfRangeException(nameof(level));
            }

            Level = level;
        }

        // Investments carry over through adaptation.
        public void Adapt(DemonSpecies nextSpecies)
        {
            Species = nextSpecies != null ? nextSpecies : throw new ArgumentNullException(nameof(nextSpecies));
        }
    }
}
