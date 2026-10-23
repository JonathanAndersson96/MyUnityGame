using NUnit.Framework;

namespace MyUnityGame.Battle.Tests
{
    public class StatTests : BattleTestFixture
    {
        [Test]
        public void Calculate_ScalesWithLevel()
        {
            Assert.AreEqual(205, StatCalculator.Calculate(StatKind.Speed, 100, 0, 100, Tuning));
            Assert.AreEqual(310, StatCalculator.Calculate(StatKind.Health, 100, 0, 100, Tuning));
            Assert.AreEqual(105, StatCalculator.Calculate(StatKind.Speed, 100, 0, 50, Tuning));
        }

        [Test]
        public void Calculate_AddsInvestedPointsBeforeLevelScaling()
        {
            Assert.AreEqual(305, StatCalculator.Calculate(StatKind.PhysicalAttack, 100, 50, 100, Tuning));
        }

        [Test]
        public void Calculate_NeverDropsBelowHalfTheBase()
        {
            Assert.AreEqual(105, StatCalculator.Calculate(StatKind.MagicDefense, 100, -100, 100, Tuning));
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
