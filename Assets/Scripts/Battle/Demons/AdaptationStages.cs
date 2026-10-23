namespace MyUnityGame.Battle
{
    public static class AdaptationStages
    {
        public const int MaxLineLength = 3;

        public static bool TryGetBaseStatTotal(int lineLength, int stage, out int total)
        {
            total = (lineLength, stage) switch
            {
                (1, 1) => 500,
                (2, 1) => 400,
                (2, 2) => 600,
                (3, 1) => 400,
                (3, 2) => 500,
                (3, 3) => 600,
                _ => 0
            };
            return total > 0;
        }
    }
}
