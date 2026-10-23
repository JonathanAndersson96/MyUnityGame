using System.Collections.Generic;
using MyUnityGame.Battle;

namespace MyUnityGame.Gameplay
{
    // Turns battle events into the lines the battle screen shows. STAB is applied silently.
    public static class BattleMessages
    {
        public static void Describe(BattleEvent battleEvent, List<string> lines)
        {
            switch (battleEvent)
            {
                case MoveUsedEvent used:
                    lines.Add($"{NameOf(used.User, used.Side)} used {used.Move.DisplayName}!");
                    break;
                case DamageDealtEvent hit:
                    DescribeHit(hit, lines);
                    break;
                case DemonBanishedEvent banished:
                    lines.Add($"{NameOf(banished.Demon, banished.Side)} was banished!");
                    break;
            }
        }

        private static void DescribeHit(DamageDealtEvent hit, List<string> lines)
        {
            var result = hit.Result;
            switch (result.Effectiveness)
            {
                case Effectiveness.NoEffect:
                    lines.Add($"It has no effect on {NameOf(hit.Target, hit.TargetSide)}...");
                    return;
                case Effectiveness.SuperEffective:
                    lines.Add("It's super effective!");
                    break;
                case Effectiveness.Resisted:
                    lines.Add("It was resisted...");
                    break;
            }

            var shift = (result.SpeedModifierBasisPoints - BasisPoints.One) / (float)BasisPoints.One * 100f;
            if (shift > 0f)
            {
                lines.Add($"Faster strike +{shift:0.#}%!");
            }
            else if (shift < 0f)
            {
                lines.Add($"Slower strike {shift:0.#}%.");
            }

            lines.Add($"{NameOf(hit.Target, hit.TargetSide)} took {hit.HpLost} damage.");
        }

        private static string NameOf(Demon demon, BattleSide side) =>
            side == BattleSide.Opponent ? $"The wild {demon.Species.DisplayName}" : demon.Species.DisplayName;
    }
}
