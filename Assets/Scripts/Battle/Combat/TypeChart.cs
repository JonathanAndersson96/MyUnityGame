using System;

namespace MyUnityGame.Battle
{
    public static class TypeChart
    {
        // Copied from the design doc. Rows attack, columns defend, both in DemonType order.
        // 2 = super effective, ½ = resisted, 0 = no effect, · = neutral (including matchups not yet decided).
        private static readonly string[] Rows =
        {
            //           Fir Ear Wat Air Lif Dea Mat Spi Ord Cha Lig Sha
            /* Fire   */ "·   ·   ½   2   2   ·   2   ·   ½   ·   ½   2",
            /* Earth  */ "2   ·   ·   0   ½   ·   ·   ·   ½   ·   2   ·",
            /* Water  */ "½   2   ·   ·   ½   ·   ·   ·   ½   ·   ·   ·",
            /* Air    */ "·   ·   2   ·   ·   ·   ·   ·   ½   ·   ·   ·",
            /* Life   */ "·   2   2   ·   ·   ½   ·   ·   ·   ·   ·   ·",
            /* Death  */ "·   0   ·   ·   2   ·   ·   2   ·   ·   ·   ½",
            /* Matter */ "·   ·   ·   ·   ·   ½   ·   0   ·   ·   ·   ½",
            /* Spirit */ "·   ½   ½   ·   ·   2   0   ·   ·   ·   2   2",
            /* Order  */ "·   ·   ·   ·   ·   ·   ·   ·   ·   2   ·   ·",
            /* Chaos  */ "·   ·   ·   ·   ·   ·   ·   ·   2   ·   ·   ·",
            /* Light  */ "·   ½   ½   ·   ·   2   ½   ·   ·   ·   ·   2",
            /* Shadow */ "·   ·   ·   ·   2   ½   ·   ·   2   ·   ½   ·"
        };

        private static readonly Effectiveness[,] Chart = Parse(Rows);

        public static Effectiveness GetEffectiveness(DemonType attackType, DemonType defendType)
        {
            var effectiveness = Chart[(int)attackType, (int)defendType];

            // Chaos ignores resistances and immunities: its attacks are never below neutral.
            return attackType == DemonType.Chaos && effectiveness < Effectiveness.Neutral
                ? Effectiveness.Neutral
                : effectiveness;
        }

        private static Effectiveness[,] Parse(string[] rows)
        {
            var typeCount = Enum.GetValues(typeof(DemonType)).Length;
            if (rows.Length != typeCount)
            {
                throw new InvalidOperationException($"The type chart needs {typeCount} rows.");
            }

            var chart = new Effectiveness[typeCount, typeCount];
            for (var attack = 0; attack < typeCount; attack++)
            {
                var cells = rows[attack].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (cells.Length != typeCount)
                {
                    throw new InvalidOperationException(
                        $"Type chart row {(DemonType)attack} needs {typeCount} cells.");
                }

                for (var defend = 0; defend < typeCount; defend++)
                {
                    chart[attack, defend] = cells[defend] switch
                    {
                        "2" => Effectiveness.SuperEffective,
                        "½" => Effectiveness.Resisted,
                        "0" => Effectiveness.NoEffect,
                        "·" => Effectiveness.Neutral,
                        _ => throw new InvalidOperationException(
                            $"Unknown type chart cell '{cells[defend]}' in row {(DemonType)attack}.")
                    };
                }
            }

            return chart;
        }
    }
}
