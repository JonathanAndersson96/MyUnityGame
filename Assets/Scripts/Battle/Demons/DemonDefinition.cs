using System;
using UnityEngine;

namespace MyUnityGame.Battle
{
    // Authored data for one demon: a starting party member or a wild encounter.
    [Serializable]
    public class DemonDefinition
    {
        public const int MinMoves = 2;

        [SerializeField] private DemonSpecies species;
        [SerializeField, Range(Demon.MinLevel, Demon.MaxLevel)] private int level = Demon.MinLevel;
        [SerializeField] private MoveDefinition[] moves = Array.Empty<MoveDefinition>();
        [SerializeField, Tooltip("Applied in order, 10 points each, from the points the demon has earned.")]
        private StatKind[] investments = Array.Empty<StatKind>();

        public DemonSpecies Species => species;
        public int Level => level;

        public bool TryCreate(out Demon demon, out string error)
        {
            demon = null;
            if (species == null)
            {
                error = "A demon needs a species.";
                return false;
            }

            if (!species.TryValidate(out error))
            {
                error = $"{species.name}: {error}";
                return false;
            }

            if (moves == null || moves.Length < MinMoves || moves.Length > Demon.MaxMoves)
            {
                error = $"{species.DisplayName} must know {MinMoves} to {Demon.MaxMoves} moves.";
                return false;
            }

            foreach (var move in moves)
            {
                if (move == null)
                {
                    error = $"{species.DisplayName} has an empty move slot.";
                    return false;
                }

                if (!move.TryValidate(out error))
                {
                    error = $"{move.name}: {error}";
                    return false;
                }
            }

            var created = new Demon(species, level, moves);
            foreach (var stat in investments ?? Array.Empty<StatKind>())
            {
                var result = created.TryInvest(stat);
                if (result != InvestmentResult.Invested)
                {
                    error = $"{species.DisplayName} can't invest in {stat}: {result}.";
                    return false;
                }
            }

            demon = created;
            error = null;
            return true;
        }
    }
}
