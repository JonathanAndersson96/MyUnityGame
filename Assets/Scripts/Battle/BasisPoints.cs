using System;

namespace MyUnityGame.Battle
{
    // Multipliers are kept as integers (10000 = 1x) so damage is exact and identical on every platform.
    public static class BasisPoints
    {
        public const int One = 10000;

        public static int FromMultiplier(float multiplier)
        {
            if (float.IsNaN(multiplier) || float.IsInfinity(multiplier) || multiplier < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplier), "Multipliers must be finite and non-negative.");
            }

            return (int)Math.Round(multiplier * One, MidpointRounding.AwayFromZero);
        }
    }
}
