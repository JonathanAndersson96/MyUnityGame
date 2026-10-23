using UnityEngine;

namespace MyUnityGame.Battle
{
    [CreateAssetMenu(menuName = "My Unity Game/Demon Species", fileName = "DemonSpecies")]
    public class DemonSpecies : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private DemonType type;
        [SerializeField, Range(1, AdaptationStages.MaxLineLength)] private int adaptationLineLength = 1;
        [SerializeField, Range(1, AdaptationStages.MaxLineLength)] private int adaptationStage = 1;
        [SerializeField] private StatBlock baseStats;

        public string DisplayName => displayName;
        public DemonType Type => type;
        public int AdaptationLineLength => adaptationLineLength;
        public int AdaptationStage => adaptationStage;
        public StatBlock BaseStats => baseStats;

        public bool TryValidate(out string error)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                error = "A demon species needs a display name.";
                return false;
            }

            if (!AdaptationStages.TryGetBaseStatTotal(adaptationLineLength, adaptationStage, out var expectedTotal))
            {
                error = $"Stage {adaptationStage} does not exist in a {adaptationLineLength}-stage line.";
                return false;
            }

            foreach (var stat in StatKinds.All)
            {
                if (baseStats[stat] <= 0)
                {
                    error = $"Base {stat} must be greater than zero.";
                    return false;
                }
            }

            if (baseStats.Total != expectedTotal)
            {
                error = $"Base stats total {baseStats.Total}, but stage {adaptationStage} of a " +
                        $"{adaptationLineLength}-stage line must total {expectedTotal}.";
                return false;
            }

            error = null;
            return true;
        }

        internal static DemonSpecies Create(string displayName, DemonType type, int lineLength, int stage,
            StatBlock baseStats)
        {
            var species = CreateInstance<DemonSpecies>();
            species.displayName = displayName;
            species.type = type;
            species.adaptationLineLength = lineLength;
            species.adaptationStage = stage;
            species.baseStats = baseStats;
            return species;
        }
    }
}
