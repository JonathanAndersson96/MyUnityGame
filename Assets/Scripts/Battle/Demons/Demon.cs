using System;
using System.Collections.Generic;
using System.Linq;

namespace MyUnityGame.Battle
{
    public class Demon
    {
        public const int MinLevel = 1;
        public const int MaxLevel = 100;

        public const int MaxMoves = 4;

        private readonly MoveDefinition[] moves;

        public DemonSpecies Species { get; private set; }
        public int Level { get; private set; }
        public InvestmentLedger Investments { get; }
        public IReadOnlyList<MoveDefinition> Moves => moves;

        public Demon(DemonSpecies species, int level, IEnumerable<MoveDefinition> moves = null,
            InvestmentLedger investments = null)
        {
            Species = species != null ? species : throw new ArgumentNullException(nameof(species));
            if (level < MinLevel || level > MaxLevel)
            {
                throw new ArgumentOutOfRangeException(nameof(level));
            }

            this.moves = moves?.ToArray() ?? Array.Empty<MoveDefinition>();
            if (this.moves.Length > MaxMoves || this.moves.Any(move => move == null))
            {
                throw new ArgumentException($"A demon knows at most {MaxMoves} moves, none of them missing.",
                    nameof(moves));
            }

            Level = level;
            Investments = investments ?? new InvestmentLedger();
        }

        public DemonType Type => Species.Type;

        // Points from skipped levels (for example, when caught at a high level) land in the bank too.
        public int BankedPoints => InvestmentRules.PointsEarnedBy(Level) - Investments.PointsSpent;

        // Damage is kept as HP lost rather than HP left, so it carries over when maximum HP changes.
        public int HpLost { get; private set; }

        public int GetStat(StatKind stat, BattleTuning tuning) =>
            StatCalculator.Calculate(Species.BaseStats[stat], Investments.ShiftFor(stat), Level, tuning);

        public int GetCurrentHp(BattleTuning tuning) => Math.Max(0, GetStat(StatKind.Health, tuning) - HpLost);

        // Banishment is permanent: a banished demon never fights again.
        public bool IsBanished { get; private set; }

        internal void Banish() => IsBanished = true;

        internal void TakeDamage(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            HpLost += amount;
        }

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
