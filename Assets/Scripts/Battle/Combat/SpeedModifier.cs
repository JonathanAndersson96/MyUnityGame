using System;

namespace MyUnityGame.Battle
{
    // Replaces critical hits. The faster demon deals extra damage and the slower demon deals less,
    // scaled by the speed ratio and capped at 2:1.
    public static class SpeedModifier
    {
        public static int ForAttacker(int attackerSpeed, int targetSpeed, BattleTuning tuning)
        {
            if (tuning == null)
            {
                throw new ArgumentNullException(nameof(tuning));
            }

            if (attackerSpeed <= 0 || targetSpeed <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attackerSpeed), "Speed must be greater than zero.");
            }

            if (attackerSpeed == targetSpeed)
            {
                return BasisPoints.One;
            }

            var faster = Math.Max(attackerSpeed, targetSpeed);
            var slower = Math.Min(attackerSpeed, targetSpeed);
            var shift = (int)((long)tuning.MaximumSpeedModifierBasisPoints * Math.Min(slower, faster - slower) / slower);
            return attackerSpeed > targetSpeed ? BasisPoints.One + shift : BasisPoints.One - shift;
        }
    }
}
