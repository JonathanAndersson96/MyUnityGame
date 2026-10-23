using System;

namespace MyUnityGame.Battle
{
    // Placeholder Gen 4-style level scaling until the stat formula is decided.
    // Invested points enter the formula like EVs, so their effect grows with level.
    public static class StatCalculator
    {
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
            var scaled = 2 * effectiveBase * level / 100;
            var value = stat == StatKind.Health ? scaled + level + 10 : scaled + 5;
            return Math.Max(tuning.MinimumStatValue, value);
        }
    }
}
