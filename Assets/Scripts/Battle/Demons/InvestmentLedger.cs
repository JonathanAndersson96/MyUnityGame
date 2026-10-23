using System;
using System.Linq;
using UnityEngine;

namespace MyUnityGame.Battle
{
    public enum InvestmentResult
    {
        Invested,
        NotEnoughPoints,
        StatHasGivenPoints,
        StatAtMaximum,
        OppositeAtFloor
    }

    // Records the points each stat has gained. A stat's opposite loses the same amount, so the BST never changes.
    [Serializable]
    public class InvestmentLedger
    {
        [SerializeField] private int[] gained = new int[StatKinds.All.Count];

        private int[] Gained => gained != null && gained.Length == StatKinds.All.Count
            ? gained
            : gained = new int[StatKinds.All.Count];

        public int PointsSpent => Gained.Sum();

        public int GainedBy(StatKind stat) => Gained[(int)stat];

        public int ShiftFor(StatKind stat) => GainedBy(stat) - GainedBy(stat.Opposite());

        public InvestmentResult CanInvest(StatKind stat, StatBlock baseStats, int availablePoints)
        {
            if (availablePoints < InvestmentRules.PointsPerInvestment)
            {
                return InvestmentResult.NotEnoughPoints;
            }

            // The first investment in a pair locks its direction.
            var opposite = stat.Opposite();
            if (GainedBy(opposite) > 0)
            {
                return InvestmentResult.StatHasGivenPoints;
            }

            var nextGain = GainedBy(stat) + InvestmentRules.PointsPerInvestment;
            if (nextGain > InvestmentRules.MaxGainPerStat)
            {
                return InvestmentResult.StatAtMaximum;
            }

            return nextGain > InvestmentRules.MaxLoss(baseStats[opposite])
                ? InvestmentResult.OppositeAtFloor
                : InvestmentResult.Invested;
        }

        public InvestmentResult TryInvest(StatKind stat, StatBlock baseStats, int availablePoints)
        {
            var result = CanInvest(stat, baseStats, availablePoints);
            if (result == InvestmentResult.Invested)
            {
                Gained[(int)stat] += InvestmentRules.PointsPerInvestment;
            }

            return result;
        }
    }
}
