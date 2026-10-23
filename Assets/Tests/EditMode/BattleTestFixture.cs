using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace MyUnityGame.Battle.Tests
{
    public abstract class BattleTestFixture
    {
        private readonly List<Object> createdObjects = new List<Object>();

        protected BattleTuning Tuning { get; private set; }

        [SetUp]
        public void CreateTuning()
        {
            Tuning = Track(ScriptableObject.CreateInstance<BattleTuning>());
        }

        [TearDown]
        public void DestroyCreatedObjects()
        {
            foreach (var createdObject in createdObjects)
            {
                Object.DestroyImmediate(createdObject);
            }

            createdObjects.Clear();
        }

        protected static StatBlock EvenStats(int value = 100) =>
            new StatBlock(value, value, value, value, value, value);

        protected DemonSpecies CreateSpecies(DemonType type, StatBlock baseStats, int lineLength = 3, int stage = 3) =>
            Track(DemonSpecies.Create($"{type} demon", type, lineLength, stage, baseStats));

        protected MoveDefinition CreateMove(DemonType type, MoveCategory category = MoveCategory.Physical,
            int power = 60) =>
            Track(MoveDefinition.Create($"{type} move", type, category, power));

        private T Track<T>(T createdObject) where T : Object
        {
            createdObjects.Add(createdObject);
            return createdObject;
        }
    }
}
