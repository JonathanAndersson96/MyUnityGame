using System;
using UnityEngine;

namespace MyUnityGame.Battle
{
    [Serializable]
    public struct StatBlock
    {
        [SerializeField] private int health;
        [SerializeField] private int physicalAttack;
        [SerializeField] private int physicalDefense;
        [SerializeField] private int magicAttack;
        [SerializeField] private int magicDefense;
        [SerializeField] private int speed;

        public StatBlock(int health, int physicalAttack, int physicalDefense, int magicAttack, int magicDefense,
            int speed)
        {
            this.health = health;
            this.physicalAttack = physicalAttack;
            this.physicalDefense = physicalDefense;
            this.magicAttack = magicAttack;
            this.magicDefense = magicDefense;
            this.speed = speed;
        }

        public int this[StatKind stat] => stat switch
        {
            StatKind.Health => health,
            StatKind.PhysicalAttack => physicalAttack,
            StatKind.PhysicalDefense => physicalDefense,
            StatKind.MagicAttack => magicAttack,
            StatKind.MagicDefense => magicDefense,
            StatKind.Speed => speed,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
        };

        public int Total => health + physicalAttack + physicalDefense + magicAttack + magicDefense + speed;
    }
}
