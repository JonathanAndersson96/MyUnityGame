namespace MyUnityGame.Battle
{
    // What happened during a turn, in order. The game turns these into battle messages.
    public abstract class BattleEvent
    {
    }

    public class MoveUsedEvent : BattleEvent
    {
        public MoveUsedEvent(BattleSide side, Demon user, MoveDefinition move)
        {
            Side = side;
            User = user;
            Move = move;
        }

        public BattleSide Side { get; }
        public Demon User { get; }
        public MoveDefinition Move { get; }
    }

    public class DamageDealtEvent : BattleEvent
    {
        public DamageDealtEvent(BattleSide targetSide, Demon target, DamageResult result, int hpLost)
        {
            TargetSide = targetSide;
            Target = target;
            Result = result;
            HpLost = hpLost;
        }

        public BattleSide TargetSide { get; }
        public Demon Target { get; }
        public DamageResult Result { get; }
        // The damage dealt, capped at the HP the target had left.
        public int HpLost { get; }
    }

    public class DemonBanishedEvent : BattleEvent
    {
        public DemonBanishedEvent(BattleSide side, Demon demon)
        {
            Side = side;
            Demon = demon;
        }

        public BattleSide Side { get; }
        public Demon Demon { get; }
    }
}
