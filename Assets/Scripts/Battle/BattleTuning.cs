using UnityEngine;

namespace MyUnityGame.Battle
{
    [CreateAssetMenu(menuName = "My Unity Game/Battle Tuning", fileName = "BattleTuning")]
    public class BattleTuning : ScriptableObject
    {
        [Header("Stats")]
        [SerializeField, Min(0f), Tooltip("How much one invested point is worth compared with one base point.")]
        private float investedPointValue = 1f;
        [SerializeField, Min(1), Tooltip("No stat can ever drop below this value.")]
        private int minimumStatValue = 1;

        [Header("Damage")]
        [SerializeField, Min(1f)] private float stabMultiplier = 1.5f;
        [SerializeField, Range(0f, 1f), Tooltip("Bonus for the faster demon and penalty for the slower one at a 2:1 speed ratio.")]
        private float maximumSpeedModifier = 0.1f;

        public float InvestedPointValue => investedPointValue;
        public int MinimumStatValue => minimumStatValue;
        public int StabBasisPoints => BasisPoints.FromMultiplier(stabMultiplier);
        public int MaximumSpeedModifierBasisPoints => BasisPoints.FromMultiplier(maximumSpeedModifier);
    }
}
