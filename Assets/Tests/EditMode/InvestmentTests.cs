using NUnit.Framework;

namespace MyUnityGame.Battle.Tests
{
    public class InvestmentTests : BattleTestFixture
    {
        private const int PlentyOfPoints = 1000;

        [TestCase(1, 0)]
        [TestCase(9, 0)]
        [TestCase(10, 10)]
        [TestCase(49, 40)]
        [TestCase(50, 50)]
        [TestCase(54, 50)]
        [TestCase(55, 60)]
        [TestCase(100, 150)]
        public void PointsEarnedBy_FollowsTheSchedule(int level, int expectedPoints)
        {
            Assert.AreEqual(expectedPoints, InvestmentRules.PointsEarnedBy(level));
        }

        [Test]
        public void Invest_MovesPointsFromTheOppositeStat()
        {
            var ledger = new InvestmentLedger();

            Assert.AreEqual(InvestmentResult.Invested, ledger.TryInvest(StatKind.Health, EvenStats(), PlentyOfPoints));
            Assert.AreEqual(10, ledger.ShiftFor(StatKind.Health));
            Assert.AreEqual(-10, ledger.ShiftFor(StatKind.Speed));
            Assert.AreEqual(10, ledger.PointsSpent);
        }

        [Test]
        public void Invest_StopsWhenTheOppositeStatReachesHalfItsBase()
        {
            var ledger = new InvestmentLedger();
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(InvestmentResult.Invested,
                    ledger.TryInvest(StatKind.Health, EvenStats(), PlentyOfPoints));
            }

            Assert.AreEqual(InvestmentResult.OppositeAtFloor,
                ledger.TryInvest(StatKind.Health, EvenStats(), PlentyOfPoints));
        }

        [Test]
        public void Invest_StopsAtTheMaximumGainPerStat()
        {
            var baseStats = new StatBlock(100, 100, 100, 100, 250, 100);
            var ledger = new InvestmentLedger();
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(InvestmentResult.Invested,
                    ledger.TryInvest(StatKind.PhysicalAttack, baseStats, PlentyOfPoints));
            }

            Assert.AreEqual(InvestmentResult.StatAtMaximum,
                ledger.TryInvest(StatKind.PhysicalAttack, baseStats, PlentyOfPoints));
        }

        [Test]
        public void Invest_LocksThePairDirectionAfterTheFirstInvestment()
        {
            var ledger = new InvestmentLedger();
            ledger.TryInvest(StatKind.MagicAttack, EvenStats(), PlentyOfPoints);

            Assert.AreEqual(InvestmentResult.StatHasGivenPoints,
                ledger.TryInvest(StatKind.PhysicalDefense, EvenStats(), PlentyOfPoints));
        }

        [Test]
        public void Invest_RequiresBankedPoints()
        {
            var ledger = new InvestmentLedger();

            Assert.AreEqual(InvestmentResult.NotEnoughPoints, ledger.TryInvest(StatKind.Speed, EvenStats(), 9));
        }

        [Test]
        public void BankedPoints_IncludesSkippedLevelsAndShrinksWhenSpent()
        {
            var demon = new Demon(CreateSpecies(DemonType.Fire, EvenStats()), 60);
            Assert.AreEqual(70, demon.BankedPoints);

            demon.TryInvest(StatKind.Speed);
            Assert.AreEqual(60, demon.BankedPoints);
        }

        [Test]
        public void Adapt_KeepsInvestments()
        {
            var demon = new Demon(CreateSpecies(DemonType.Fire, EvenStats(80), 3, 2), 50);
            demon.TryInvest(StatKind.PhysicalAttack);

            demon.Adapt(CreateSpecies(DemonType.Fire, EvenStats()));

            Assert.AreEqual(10, demon.Investments.ShiftFor(StatKind.PhysicalAttack));
            Assert.AreEqual(40, demon.BankedPoints);
        }
    }
}
