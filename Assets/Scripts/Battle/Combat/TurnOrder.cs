namespace MyUnityGame.Battle
{
    public enum BattleSide
    {
        Player,
        Opponent
    }

    // Create one per battle. Speed decides who acts first. Speed ties alternate between the sides for the
    // whole battle; the first tie goes to the higher-level demon, and to the opponent if levels are equal.
    public class TurnOrder
    {
        private BattleSide? lastTieWinner;

        public BattleSide FirstToAct(int playerSpeed, int playerLevel, int opponentSpeed, int opponentLevel)
        {
            if (playerSpeed != opponentSpeed)
            {
                return playerSpeed > opponentSpeed ? BattleSide.Player : BattleSide.Opponent;
            }

            var winner = lastTieWinner.HasValue
                ? Other(lastTieWinner.Value)
                : playerLevel > opponentLevel ? BattleSide.Player : BattleSide.Opponent;
            lastTieWinner = winner;
            return winner;
        }

        private static BattleSide Other(BattleSide side) =>
            side == BattleSide.Player ? BattleSide.Opponent : BattleSide.Player;
    }
}
