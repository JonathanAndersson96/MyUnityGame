using NUnit.Framework;

namespace MyUnityGame.Battle.Tests
{
    public class DamageTests : BattleTestFixture
    {
        private const int Level = 50;

        // Even 100s at level 50 give 120 in every stat, so a 60-power move has a base damage of 28.
        private const int NeutralDamage = 28;

        [TestCase(200, 100, 11000)]
        [TestCase(300, 100, 11000)]
        [TestCase(150, 100, 10500)]
        [TestCase(125, 100, 10250)]
        [TestCase(100, 100, 10000)]
        [TestCase(100, 125, 9750)]
        [TestCase(100, 150, 9500)]
        [TestCase(100, 200, 9000)]
        [TestCase(100, 400, 9000)]
        public void SpeedModifier_MatchesTheDesignDocTable(int attackerSpeed, int targetSpeed, int expected)
        {
            Assert.AreEqual(expected, SpeedModifier.ForAttacker(attackerSpeed, targetSpeed, Tuning));
        }

        [Test]
        public void Damage_IsExactWithoutStab()
        {
            var result = Hit(DemonType.Matter, DemonType.Order, DemonType.Matter);

            Assert.AreEqual(NeutralDamage, result.Damage);
            Assert.IsFalse(result.IsStab);
            Assert.AreEqual(BasisPoints.One, result.SpeedModifierBasisPoints);
        }

        [Test]
        public void Damage_AppliesStab()
        {
            var result = Hit(DemonType.Matter, DemonType.Matter, DemonType.Matter);

            Assert.IsTrue(result.IsStab);
            Assert.AreEqual(42, result.Damage);
        }

        [Test]
        public void Damage_AppliesTypeEffectiveness()
        {
            Assert.AreEqual(NeutralDamage * 2, Hit(DemonType.Matter, DemonType.Fire, DemonType.Matter).Damage);
            Assert.AreEqual(NeutralDamage / 2, Hit(DemonType.Matter, DemonType.Fire, DemonType.Water).Damage);
        }

        [Test]
        public void Damage_IsZeroAgainstAnImmuneTarget()
        {
            var result = Hit(DemonType.Matter, DemonType.Earth, DemonType.Air);

            Assert.AreEqual(Effectiveness.NoEffect, result.Effectiveness);
            Assert.AreEqual(0, result.Damage);
        }

        [Test]
        public void Damage_RewardsTheFasterDemon()
        {
            var attacker = new Demon(CreateSpecies(DemonType.Matter, new StatBlock(100, 100, 100, 100, 100, 210)), Level);
            var target = new Demon(CreateSpecies(DemonType.Matter, EvenStats()), Level);

            var result = DamageCalculator.Calculate(attacker, target, CreateMove(DemonType.Order), Tuning);

            Assert.AreEqual(11000, result.SpeedModifierBasisPoints);
            Assert.AreEqual(30, result.Damage);
        }

        [Test]
        public void TurnOrder_LetsTheFasterDemonActFirst()
        {
            var order = new TurnOrder();

            Assert.AreEqual(BattleSide.Player, order.FirstToAct(120, 10, 100, 50));
            Assert.AreEqual(BattleSide.Opponent, order.FirstToAct(100, 50, 120, 10));
        }

        [Test]
        public void TurnOrder_GivesTheFirstTieToTheHigherLevelThenAlternates()
        {
            var order = new TurnOrder();

            Assert.AreEqual(BattleSide.Player, order.FirstToAct(100, 31, 100, 30));
            Assert.AreEqual(BattleSide.Opponent, order.FirstToAct(100, 31, 100, 30));
            Assert.AreEqual(BattleSide.Player, order.FirstToAct(100, 31, 100, 30));
        }

        [Test]
        public void TurnOrder_GivesTheFirstTieToTheOpponentAtEqualLevels()
        {
            Assert.AreEqual(BattleSide.Opponent, new TurnOrder().FirstToAct(100, 30, 100, 30));
        }

        private DamageResult Hit(DemonType attackerType, DemonType moveType, DemonType targetType)
        {
            var attacker = new Demon(CreateSpecies(attackerType, EvenStats()), Level);
            var target = new Demon(CreateSpecies(targetType, EvenStats()), Level);
            return DamageCalculator.Calculate(attacker, target, CreateMove(moveType), Tuning);
        }
    }
}
