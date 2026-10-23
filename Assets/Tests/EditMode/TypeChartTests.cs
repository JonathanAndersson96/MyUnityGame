using System;
using NUnit.Framework;

namespace MyUnityGame.Battle.Tests
{
    public class TypeChartTests
    {
        [TestCase(DemonType.Water, DemonType.Earth, Effectiveness.SuperEffective)]
        [TestCase(DemonType.Earth, DemonType.Fire, Effectiveness.SuperEffective)]
        [TestCase(DemonType.Fire, DemonType.Air, Effectiveness.SuperEffective)]
        [TestCase(DemonType.Air, DemonType.Water, Effectiveness.SuperEffective)]
        [TestCase(DemonType.Fire, DemonType.Water, Effectiveness.Resisted)]
        [TestCase(DemonType.Water, DemonType.Fire, Effectiveness.Resisted)]
        [TestCase(DemonType.Earth, DemonType.Air, Effectiveness.NoEffect)]
        [TestCase(DemonType.Death, DemonType.Earth, Effectiveness.NoEffect)]
        [TestCase(DemonType.Spirit, DemonType.Matter, Effectiveness.NoEffect)]
        [TestCase(DemonType.Matter, DemonType.Spirit, Effectiveness.NoEffect)]
        [TestCase(DemonType.Order, DemonType.Chaos, Effectiveness.SuperEffective)]
        [TestCase(DemonType.Chaos, DemonType.Order, Effectiveness.SuperEffective)]
        [TestCase(DemonType.Air, DemonType.Order, Effectiveness.Resisted)]
        [TestCase(DemonType.Fire, DemonType.Matter, Effectiveness.SuperEffective)]
        [TestCase(DemonType.Earth, DemonType.Light, Effectiveness.SuperEffective)]
        [TestCase(DemonType.Light, DemonType.Death, Effectiveness.SuperEffective)]
        [TestCase(DemonType.Shadow, DemonType.Death, Effectiveness.Resisted)]
        [TestCase(DemonType.Order, DemonType.Fire, Effectiveness.Neutral)]
        public void GetEffectiveness_MatchesTheDesignDoc(DemonType attack, DemonType defend, Effectiveness expected)
        {
            Assert.AreEqual(expected, TypeChart.GetEffectiveness(attack, defend));
        }

        [Test]
        public void Chaos_IsNeverBelowNeutral()
        {
            foreach (DemonType defend in Enum.GetValues(typeof(DemonType)))
            {
                Assert.GreaterOrEqual(TypeChart.GetEffectiveness(DemonType.Chaos, defend), Effectiveness.Neutral,
                    $"Chaos against {defend}");
            }
        }

        [Test]
        public void StatusKinds_MapsElementsAndTheSpiritQuad()
        {
            Assert.AreEqual(StatusKind.Burn, StatusKinds.ForType(DemonType.Fire));
            Assert.AreEqual(StatusKind.Blind, StatusKinds.ForType(DemonType.Light));
            Assert.AreEqual(StatusKind.None, StatusKinds.ForType(DemonType.Chaos));
        }
    }
}
