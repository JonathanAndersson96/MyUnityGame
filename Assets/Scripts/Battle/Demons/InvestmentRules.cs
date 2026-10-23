using System;

namespace MyUnityGame.Battle
{
    public static class InvestmentRules
    {
        public const int PointsPerInvestment = 10;
        public const int MaxGainPerStat = 100;

        // One investment every 10 levels from 10 to 50, then every 5 levels from 55 to 100: 150 points in total.
        public static int PointsEarnedBy(int level)
        {
            var clamped = Math.Clamp(level, Demon.MinLevel, Demon.MaxLevel);
            var investments = clamped < 10 ? 0
                : clamped <= 50 ? clamped / 10
                : 5 + (clamped - 50) / 5;
            return investments * PointsPerInvestment;
        }

        // A stat can never drop below 50% of its base value.
        public static int MaxLoss(int baseValue) => Math.Max(0, baseValue) / 2;
    }
}
