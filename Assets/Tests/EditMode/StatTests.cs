using NUnit.Framework;

namespace MyUnityGame.Battle.Tests
{
    public class StatTests : BattleTestFixture
    {
        [Test]
        public void Calculate_FollowsTheDesignDocLevelCurve()
        {
            // 2 × Base × (Level + 25) ÷ 125: twice the base at level 100, 1.2× at level 50.
            Assert.AreEqual(200, StatCalculator.Calculate(100, 0, 100, Tuning));
            Assert.AreEqual(120, StatCalculator.Calculate(100, 0, 50, Tuning));
        }

        [Test]
        public void Demon_GivesHealthTheSameCurveAsEveryOtherStat()
        {
            var species = CreateSpecies(DemonType.Matter, EvenStats());
            Assert.AreEqual(200, new Demon(species, 100).GetStat(StatKind.Health, Tuning));
            Assert.AreEqual(200, new Demon(species, 100).GetStat(StatKind.Speed, Tuning));
            Assert.AreEqual(120, new Demon(species, 50).GetStat(StatKind.Health, Tuning));
        }

        [Test]
        public void Calculate_AddsInvestedPointsBeforeLevelScaling()
        {
            Assert.AreEqual(300, StatCalculator.Calculate(100, 50, 100, Tuning));
        }

        [Test]
        public void Calculate_NeverDropsBelowHalfTheBase()
        {
            Assert.AreEqual(100, StatCalculator.Calculate(100, -100, 100, Tuning));
        }

        [Test]
        public void Calculate_NeverDropsBelowTheMinimumStatValue()
        {
            // 2 × 1 × 26 ÷ 125 rounds down to 0, so the minimum stat value of 1 applies.
            Assert.AreEqual(1, StatCalculator.Calculate(1, 0, 1, Tuning));
        }

        [TestCase(1, 1, 500, true)]
        [TestCase(2, 1, 400, true)]
        [TestCase(2, 2, 600, true)]
        [TestCase(3, 2, 500, true)]
        [TestCase(3, 3, 500, false)]
        [TestCase(1, 2, 500, false)]
        public void Species_ValidatesItsBaseStatTotal(int lineLength, int stage, int total, bool expectedValid)
        {
            var share = total / 6;
            var baseStats = new StatBlock(share, share, share, share, share, total - share * 5);
            var species = CreateSpecies(DemonType.Matter, baseStats, lineLength, stage);

            Assert.AreEqual(expectedValid, species.TryValidate(out _));
        }
    }
}
