using System;
using UnityEngine;

namespace MyUnityGame.Gameplay
{
    [CreateAssetMenu(menuName = "My Unity Game/Battle Rules", fileName = "BattleRules")]
    public class BattleRules : ScriptableObject
    {
        [SerializeField] private int opponentMaxHp = 16;
        [SerializeField] private int playerMinimumDamage = 4;
        [SerializeField] private int playerMaximumDamage = 8;
        [SerializeField] private int enemyMinimumDamage = 3;
        [SerializeField] private int enemyMaximumDamage = 6;
        [SerializeField] private int guardDamageDivisor = 2;
        [SerializeField] private int minimumGuardDamage = 1;
        [SerializeField, Min(0f)] private float enemyTurnDelay = 0.6f;

        public int OpponentMaxHp => opponentMaxHp;
        public int PlayerMinimumDamage => playerMinimumDamage;
        public int PlayerMaximumDamage => playerMaximumDamage;
        public int EnemyMinimumDamage => enemyMinimumDamage;
        public int EnemyMaximumDamage => enemyMaximumDamage;
        public int GuardDamageDivisor => guardDamageDivisor;
        public int MinimumGuardDamage => minimumGuardDamage;
        public float EnemyTurnDelay => enemyTurnDelay;

        public bool TryValidate(out string error)
        {
            if (opponentMaxHp <= 0)
            {
                error = "Opponent maximum HP must be greater than zero.";
                return false;
            }

            if (!IsValidDamageRange(playerMinimumDamage, playerMaximumDamage) ||
                !IsValidDamageRange(enemyMinimumDamage, enemyMaximumDamage))
            {
                error = "Damage ranges must have a positive minimum and a valid maximum.";
                return false;
            }

            if (guardDamageDivisor < 2 || minimumGuardDamage <= 0)
            {
                error = "Guard settings must reduce damage without exceeding the enemy maximum.";
                return false;
            }

            var lowestGuardDamage = Mathf.Max(1, enemyMinimumDamage / guardDamageDivisor);
            if (minimumGuardDamage > lowestGuardDamage)
            {
                error = "Minimum guard damage cannot exceed the lowest guarded enemy hit.";
                return false;
            }

            if (float.IsNaN(enemyTurnDelay) || float.IsInfinity(enemyTurnDelay) || enemyTurnDelay < 0f)
            {
                error = "Enemy turn delay must be a finite, non-negative value.";
                return false;
            }

            error = null;
            return true;
        }

        private static bool IsValidDamageRange(int minimum, int maximum)
        {
            return minimum > 0 && maximum >= minimum && maximum < int.MaxValue;
        }
    }
}
