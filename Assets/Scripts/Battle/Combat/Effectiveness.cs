using System;

namespace MyUnityGame.Battle
{
    public enum Effectiveness
    {
        NoEffect,
        Resisted,
        Neutral,
        SuperEffective
    }

    public static class EffectivenessExtensions
    {
        public static int ToBasisPoints(this Effectiveness effectiveness) => effectiveness switch
        {
            Effectiveness.NoEffect => 0,
            Effectiveness.Resisted => BasisPoints.One / 2,
            Effectiveness.Neutral => BasisPoints.One,
            Effectiveness.SuperEffective => BasisPoints.One * 2,
            _ => throw new ArgumentOutOfRangeException(nameof(effectiveness), effectiveness, null)
        };
    }
}
