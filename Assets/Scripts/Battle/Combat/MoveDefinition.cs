using UnityEngine;

namespace MyUnityGame.Battle
{
    public enum MoveCategory
    {
        Physical,
        Magic
    }

    // Every move hits. Costs for strong moves (Health, stats, a turn, a forced switch, a status on the user)
    // are still an open question in the design doc.
    [CreateAssetMenu(menuName = "My Unity Game/Move", fileName = "Move")]
    public class MoveDefinition : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private DemonType type;
        [SerializeField] private MoveCategory category;
        [SerializeField, Min(1)] private int power = 40;
        [SerializeField, Tooltip("Contact moves miss a target under Float.")] private bool makesContact;

        public string DisplayName => displayName;
        public DemonType Type => type;
        public MoveCategory Category => category;
        public int Power => power;
        public bool MakesContact => makesContact;

        public bool TryValidate(out string error)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                error = "A move needs a display name.";
                return false;
            }

            if (power <= 0)
            {
                error = "Move power must be greater than zero.";
                return false;
            }

            error = null;
            return true;
        }

        internal static MoveDefinition Create(string displayName, DemonType type, MoveCategory category, int power,
            bool makesContact = false)
        {
            var move = CreateInstance<MoveDefinition>();
            move.displayName = displayName;
            move.type = type;
            move.category = category;
            move.power = power;
            move.makesContact = makesContact;
            return move;
        }
    }
}
