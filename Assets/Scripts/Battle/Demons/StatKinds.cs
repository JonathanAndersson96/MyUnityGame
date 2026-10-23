using System;
using System.Collections.Generic;

namespace MyUnityGame.Battle
{
    public static class StatKinds
    {
        public static readonly IReadOnlyList<StatKind> All = new[]
        {
            StatKind.Health,
            StatKind.PhysicalAttack,
            StatKind.PhysicalDefense,
            StatKind.MagicAttack,
            StatKind.MagicDefense,
            StatKind.Speed
        };

        // Each pair trades aggression against durability.
        public static StatKind Opposite(this StatKind stat) => stat switch
        {
            StatKind.PhysicalAttack => StatKind.MagicDefense,
            StatKind.MagicDefense => StatKind.PhysicalAttack,
            StatKind.MagicAttack => StatKind.PhysicalDefense,
            StatKind.PhysicalDefense => StatKind.MagicAttack,
            StatKind.Speed => StatKind.Health,
            StatKind.Health => StatKind.Speed,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
        };
    }
}
