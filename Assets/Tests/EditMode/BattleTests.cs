using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MyUnityGame.Battle.Tests
{
    public class BattleTests : BattleTestFixture
    {
        private const int Level = 50;

        [Test]
        public void Start_PutsTheFirstPartyDemonForwardAgainstTheWildDemon()
        {
            var first = CreateDemon(DemonType.Fire, EvenStats(), Level, CreateMove(DemonType.Fire));
            var second = CreateDemon(DemonType.Water, EvenStats(), Level, CreateMove(DemonType.Water));
            var wild = CreateDemon(DemonType.Death, EvenStats(), Level, CreateMove(DemonType.Death));

            var battle = new DemonBattle(new Party(new[] { first, second }), wild, Tuning);

            Assert.AreSame(first, battle.PlayerDemon);
            Assert.AreSame(wild, battle.OpponentDemon);
            Assert.AreEqual(BattleOutcome.Ongoing, battle.Outcome);
        }

        [Test]
        public void UseMove_LetsTheFasterDemonActFirst()
        {
            var fast = new StatBlock(100, 100, 100, 100, 100, 150);
            var slow = EvenStats();

            var playerFaster = StartBattle(fast, slow).UseMove(0);
            var opponentFaster = StartBattle(slow, fast).UseMove(0);

            CollectionAssert.AreEqual(new[] { BattleSide.Player, BattleSide.Opponent }, MoveUsers(playerFaster));
            CollectionAssert.AreEqual(new[] { BattleSide.Opponent, BattleSide.Player }, MoveUsers(opponentFaster));
        }

        [Test]
        public void UseMove_DealsExactDamageToEachDemon()
        {
            // Even 100s at level 50 give 120 HP, and a neutral 60-power hit deals 28.
            var battle = StartBattle(EvenStats(), EvenStats());

            var damage = battle.UseMove(0).OfType<DamageDealtEvent>().ToArray();

            Assert.AreEqual(2, damage.Length);
            Assert.IsTrue(damage.All(hit => hit.Result.Damage == 28 && hit.Result.Effectiveness == Effectiveness.Neutral));
            Assert.AreEqual(92, battle.PlayerDemon.GetCurrentHp(Tuning));
            Assert.AreEqual(92, battle.OpponentDemon.GetCurrentHp(Tuning));
        }

        [Test]
        public void Opponent_UsesItsHighestDamageMoveRatherThanItsStrongest()
        {
            var strongNeutral = CreateMove(DemonType.Order, power: 80);
            var superEffective = CreateMove(DemonType.Fire, power: 60);
            var player = CreateDemon(DemonType.Air, EvenStats(), Level, CreateMove(DemonType.Order));
            var opponent = CreateDemon(DemonType.Matter, EvenStats(), Level, strongNeutral, superEffective);
            var battle = new DemonBattle(new Party(new[] { player }), opponent, Tuning);

            var opponentMove = battle.UseMove(0).OfType<MoveUsedEvent>().Single(used => used.Side == BattleSide.Opponent);

            Assert.AreSame(superEffective, opponentMove.Move);
        }

        [Test]
        public void BanishingTheWildDemon_WinsBeforeItCanAct()
        {
            // Base Health 10 at level 50 gives 12 HP, less than one hit.
            var battle = StartBattle(new StatBlock(100, 100, 100, 100, 100, 150), new StatBlock(10, 100, 100, 100, 100, 100));

            var events = battle.UseMove(0);

            CollectionAssert.AreEqual(new[] { BattleSide.Player }, MoveUsers(events));
            // 28 base damage, +5% for the 1.5:1 faster attacker.
            var finishingHit = events.OfType<DamageDealtEvent>().Single();
            Assert.AreEqual(29, finishingHit.Result.Damage);
            Assert.AreEqual(12, finishingHit.HpLost, "Only the HP the demon had left is lost.");
            Assert.AreEqual(BattleSide.Opponent, events.OfType<DemonBanishedEvent>().Single().Side);
            Assert.AreEqual(0, battle.OpponentDemon.GetCurrentHp(Tuning));
            Assert.AreEqual(BattleOutcome.Won, battle.Outcome);
        }

        [Test]
        public void BanishedPlayerDemon_LeavesThePartyAndTheBattleIsLost()
        {
            var fragile = CreateDemon(DemonType.Matter, new StatBlock(10, 100, 100, 100, 100, 100), Level,
                CreateMove(DemonType.Order));
            var backup = CreateDemon(DemonType.Matter, EvenStats(), Level, CreateMove(DemonType.Order));
            var wild = CreateDemon(DemonType.Matter, new StatBlock(100, 100, 100, 100, 100, 150), Level,
                CreateMove(DemonType.Order));
            var party = new Party(new[] { fragile, backup });
            var battle = new DemonBattle(party, wild, Tuning);

            var events = battle.UseMove(0);

            CollectionAssert.AreEqual(new[] { BattleSide.Opponent }, MoveUsers(events));
            Assert.IsTrue(fragile.IsBanished);
            CollectionAssert.AreEqual(new[] { backup }, party.Members);
            Assert.AreEqual(BattleOutcome.Lost, battle.Outcome);
        }

        [Test]
        public void DamageEvents_ReportEffectivenessStabAndTheSpeedModifier()
        {
            // The 2:1 faster Fire demon gets STAB and the full +10% speed bonus; the slower Water demon gets −10%.
            var player = CreateDemon(DemonType.Fire, new StatBlock(100, 100, 100, 100, 100, 200), Level,
                CreateMove(DemonType.Fire));
            var wild = CreateDemon(DemonType.Water, EvenStats(), Level, CreateMove(DemonType.Order));
            var battle = new DemonBattle(new Party(new[] { player }), wild, Tuning);

            var hits = battle.UseMove(0).OfType<DamageDealtEvent>().ToArray();
            var playerHit = hits.Single(hit => hit.TargetSide == BattleSide.Opponent);
            var opponentHit = hits.Single(hit => hit.TargetSide == BattleSide.Player);

            // Water resists Fire: 28 × 1.5 STAB × ½ × 1.1 speed = 23.
            Assert.AreEqual(Effectiveness.Resisted, playerHit.Result.Effectiveness);
            Assert.IsTrue(playerHit.Result.IsStab);
            Assert.AreEqual(11000, playerHit.Result.SpeedModifierBasisPoints);
            Assert.AreEqual(23, playerHit.Result.Damage);
            Assert.AreEqual(9000, opponentHit.Result.SpeedModifierBasisPoints);
        }

        [Test]
        public void Opponent_PicksTheFirstListedMoveWhenTwoHitEquallyHard()
        {
            var first = CreateMove(DemonType.Order);
            var second = CreateMove(DemonType.Chaos);
            var player = CreateDemon(DemonType.Matter, EvenStats(), Level, CreateMove(DemonType.Order));
            var wild = CreateDemon(DemonType.Matter, EvenStats(), Level, first, second);
            var battle = new DemonBattle(new Party(new[] { player }), wild, Tuning);

            var opponentMove = battle.UseMove(0).OfType<MoveUsedEvent>().Single(used => used.Side == BattleSide.Opponent);

            Assert.AreSame(first, opponentMove.Move);
        }

        [Test]
        public void SpeedTies_AlternateAcrossTurnsStartingWithTheOpponentAtEqualLevels()
        {
            var battle = StartBattle(EvenStats(), EvenStats());

            CollectionAssert.AreEqual(new[] { BattleSide.Opponent, BattleSide.Player }, MoveUsers(battle.UseMove(0)));
            CollectionAssert.AreEqual(new[] { BattleSide.Player, BattleSide.Opponent }, MoveUsers(battle.UseMove(0)));
        }

        [Test]
        public void ImmuneTarget_TakesNoDamage()
        {
            // Earth is immune to Death.
            var player = CreateDemon(DemonType.Earth, EvenStats(), Level, CreateMove(DemonType.Order));
            var wild = CreateDemon(DemonType.Death, EvenStats(), Level, CreateMove(DemonType.Death));
            var battle = new DemonBattle(new Party(new[] { player }), wild, Tuning);

            var hitOnPlayer = battle.UseMove(0).OfType<DamageDealtEvent>().Single(hit => hit.TargetSide == BattleSide.Player);

            Assert.AreEqual(Effectiveness.NoEffect, hitOnPlayer.Result.Effectiveness);
            Assert.AreEqual(0, hitOnPlayer.Result.Damage);
            Assert.AreEqual(120, player.GetCurrentHp(Tuning));
        }

        [Test]
        public void HpLost_CarriesOverIntoTheNextBattle()
        {
            var player = CreateDemon(DemonType.Matter, EvenStats(), Level, CreateMove(DemonType.Order));
            var party = new Party(new[] { player });
            new DemonBattle(party, CreateDemon(DemonType.Matter, EvenStats(), Level, CreateMove(DemonType.Order)), Tuning)
                .UseMove(0);

            var nextBattle = new DemonBattle(party, CreateDemon(DemonType.Matter, EvenStats(), Level,
                CreateMove(DemonType.Order)), Tuning);

            Assert.AreEqual(92, nextBattle.PlayerDemon.GetCurrentHp(Tuning));
        }

        private DemonBattle StartBattle(StatBlock playerStats, StatBlock opponentStats)
        {
            var player = CreateDemon(DemonType.Matter, playerStats, Level, CreateMove(DemonType.Order));
            var opponent = CreateDemon(DemonType.Matter, opponentStats, Level, CreateMove(DemonType.Order));
            return new DemonBattle(new Party(new[] { player }), opponent, Tuning);
        }

        private static BattleSide[] MoveUsers(IEnumerable<BattleEvent> events) =>
            events.OfType<MoveUsedEvent>().Select(used => used.Side).ToArray();
    }
}
