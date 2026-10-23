using System;

namespace MyUnityGame.Battle
{
    // Every stat, Health included, follows the design doc's curve: 2 × Base × (Level + 25) ÷ 125,
    // so a stat is 1.2× its base at level 50 and twice its base at level 100.
    // Invested points enter the formula like EVs, so their effect grows with level.
    public static class StatCalculator
    {
        private const int LevelOffset = 25;
        private const int LevelDivisor = 125;

        public static int Calculate(StatKind stat, int baseValue, int investedShift, int level, BattleTuning tuning)
        {
            if (tuning == null)
            {
                throw new ArgumentNullException(nameof(tuning));
            }

            if (baseValue <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(baseValue), "Base stats must be greater than zero.");
            }

            if (level < Demon.MinLevel || level > Demon.MaxLevel)
            {
                throw new ArgumentOutOfRangeException(nameof(level));
            }

            var shifted = baseValue + (int)(investedShift * (double)tuning.InvestedPointValue);
            var effectiveBase = Math.Max(baseValue - InvestmentRules.MaxLoss(baseValue), shifted);
            var value = 2 * effectiveBase * (level + LevelOffset) / LevelDivisor;
            return Math.Max(tuning.MinimumStatValue, value);
        }
    }
}
