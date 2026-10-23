using System;

namespace MyUnityGame.Battle
{
    public readonly struct DamageResult
    {
        public DamageResult(int damage, Effectiveness effectiveness, bool isStab, int speedModifierBasisPoints)
        {
            Damage = damage;
            Effectiveness = effectiveness;
            IsStab = isStab;
            SpeedModifierBasisPoints = speedModifierBasisPoints;
        }

        public int Damage { get; }
        public Effectiveness Effectiveness { get; }
        public bool IsStab { get; }

        // Shown in battle the way a critical hit is shown in Pokémon.
        public int SpeedModifierBasisPoints { get; }
    }

    // Fully deterministic: no damage roll, no critical hits, no misses.
    public static class DamageCalculator
    {
        private const long ModifierScale = (long)BasisPoints.One * BasisPoints.One * BasisPoints.One;

        public static DamageResult Calculate(Demon attacker, Demon target, MoveDefinition move, BattleTuning tuning)
        {
            if (attacker == null)
            {
                throw new ArgumentNullException(nameof(attacker));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (move == null)
            {
                throw new ArgumentNullException(nameof(move));
            }

            if (tuning == null)
            {
                throw new ArgumentNullException(nameof(tuning));
            }

            var physical = move.Category == MoveCategory.Physical;
            var attack = attacker.GetStat(physical ? StatKind.PhysicalAttack : StatKind.MagicAttack, tuning);
            var defense = target.GetStat(physical ? StatKind.PhysicalDefense : StatKind.MagicDefense, tuning);
            var effectiveness = TypeChart.GetEffectiveness(move.Type, target.Type);
            var isStab = move.Type == attacker.Type;
            var speedModifier = SpeedModifier.ForAttacker(
                attacker.GetStat(StatKind.Speed, tuning), target.GetStat(StatKind.Speed, tuning), tuning);

            if (effectiveness == Effectiveness.NoEffect)
            {
                return new DamageResult(0, effectiveness, isStab, speedModifier);
            }

            var stab = isStab ? tuning.StabBasisPoints : BasisPoints.One;
            var damage = BaseDamage(attacker.Level, move.Power, attack, defense) *
                         stab * effectiveness.ToBasisPoints() * speedModifier / ModifierScale;
            return new DamageResult((int)Math.Max(1L, damage), effectiveness, isStab, speedModifier);
        }

        // Placeholder: the Gen 4 base damage formula without the random roll.
        private static long BaseDamage(int level, int power, int attack, int defense) =>
            (2L * level / 5 + 2) * power * attack / defense / 50 + 2;
    }
}
