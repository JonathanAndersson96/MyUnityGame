using System;
using System.Collections.Generic;

namespace MyUnityGame.Battle
{
    // A singles battle between the player's party and one wild demon.
    public class DemonBattle
    {
        private readonly Party party;
        private readonly BattleTuning tuning;
        private readonly TurnOrder turnOrder = new TurnOrder();

        public DemonBattle(Party party, Demon wildDemon, BattleTuning tuning)
        {
            this.party = party ?? throw new ArgumentNullException(nameof(party));
            this.tuning = tuning != null ? tuning : throw new ArgumentNullException(nameof(tuning));
            OpponentDemon = wildDemon ?? throw new ArgumentNullException(nameof(wildDemon));
            if (party.Members.Count == 0)
            {
                throw new ArgumentException("A battle needs at least one demon in the party.", nameof(party));
            }

            PlayerDemon = party.Members[0];
        }

        public Demon PlayerDemon { get; private set; }
        public Demon OpponentDemon { get; }
        public BattleOutcome Outcome { get; private set; }

        public IReadOnlyList<BattleEvent> UseMove(int moveIndex)
        {
            if (Outcome != BattleOutcome.Ongoing)
            {
                throw new InvalidOperationException("The battle is already over.");
            }

            var playerMove = PlayerDemon.Moves[moveIndex];
            var opponentMove = ChooseOpponentMove();
            var first = turnOrder.FirstToAct(
                PlayerDemon.GetStat(StatKind.Speed, tuning), PlayerDemon.Level,
                OpponentDemon.GetStat(StatKind.Speed, tuning), OpponentDemon.Level);

            var events = new List<BattleEvent>();
            if (first == BattleSide.Player)
            {
                Act(BattleSide.Player, playerMove, events);
                Act(BattleSide.Opponent, opponentMove, events);
            }
            else
            {
                Act(BattleSide.Opponent, opponentMove, events);
                Act(BattleSide.Player, playerMove, events);
            }

            return events;
        }

        // Predictable on purpose: the move that hits hardest right now, the first listed on a tie.
        private MoveDefinition ChooseOpponentMove()
        {
            MoveDefinition best = null;
            var bestDamage = -1;
            foreach (var move in OpponentDemon.Moves)
            {
                var damage = DamageCalculator.Calculate(OpponentDemon, PlayerDemon, move, tuning).Damage;
                if (damage > bestDamage)
                {
                    best = move;
                    bestDamage = damage;
                }
            }

            return best;
        }

        private void Act(BattleSide side, MoveDefinition move, List<BattleEvent> events)
        {
            var user = DemonOn(side);
            if (Outcome != BattleOutcome.Ongoing || user.IsBanished)
            {
                return;
            }

            var targetSide = side == BattleSide.Player ? BattleSide.Opponent : BattleSide.Player;
            var target = DemonOn(targetSide);

            events.Add(new MoveUsedEvent(side, user, move));
            var result = DamageCalculator.Calculate(user, target, move, tuning);
            var hpLost = Math.Min(result.Damage, target.GetCurrentHp(tuning));
            target.TakeDamage(hpLost);
            events.Add(new DamageDealtEvent(targetSide, target, result, hpLost));

            if (target.GetCurrentHp(tuning) == 0)
            {
                target.Banish();
                if (targetSide == BattleSide.Player)
                {
                    party.Remove(target);
                }

                events.Add(new DemonBanishedEvent(targetSide, target));
                Outcome = targetSide == BattleSide.Opponent ? BattleOutcome.Won : BattleOutcome.Lost;
            }
        }

        private Demon DemonOn(BattleSide side) => side == BattleSide.Player ? PlayerDemon : OpponentDemon;
    }
}
