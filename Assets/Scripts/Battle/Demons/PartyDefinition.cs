using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyUnityGame.Battle
{
    // Authored data for a party, such as the one a new session starts with.
    [CreateAssetMenu(menuName = "My Unity Game/Party", fileName = "Party")]
    public class PartyDefinition : ScriptableObject
    {
        [SerializeField] private DemonDefinition[] members = Array.Empty<DemonDefinition>();

        public bool TryCreateParty(out Party party, out string error)
        {
            party = null;
            if (members == null || members.Length == 0 || members.Length > Party.MaxSize)
            {
                error = $"A party needs 1 to {Party.MaxSize} demons.";
                return false;
            }

            var demons = new List<Demon>(members.Length);
            foreach (var member in members)
            {
                if (member == null)
                {
                    error = "A party member is missing.";
                    return false;
                }

                if (!member.TryCreate(out var demon, out error))
                {
                    return false;
                }

                demons.Add(demon);
            }

            party = new Party(demons);
            error = null;
            return true;
        }
    }
}
