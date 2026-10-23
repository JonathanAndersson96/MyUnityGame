using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyUnityGame.Battle;
using UnityEngine;

namespace MyUnityGame.Gameplay
{
    [CreateAssetMenu(menuName = "My Unity Game/World/Grid Settings", fileName = "GridWorldSettings")]
    public class GridWorldSettings : ScriptableObject
    {
        private static readonly Vector2Int[] DefaultObstacleOffsets =
        {
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            new Vector2Int(1, 2),
            new Vector2Int(3, 2),
            new Vector2Int(2, 4),
            new Vector2Int(4, 4),
            new Vector2Int(0, 4),
            new Vector2Int(4, 0)
        };

        [SerializeField] private Vector2Int gridSize = new Vector2Int(35, 35);
        [SerializeField] private Vector2Int playerStartCell = new Vector2Int(17, 17);
        [SerializeField] private string encounterName = "Demon";
        [SerializeField] private DemonDefinition wildDemon = new DemonDefinition();
        [SerializeField] private Vector2Int demonOffset = new Vector2Int(4, 3);
        [SerializeField] private Vector2Int[] obstacleOffsets = DefaultObstacleOffsets;
        [SerializeField] private string idleSpriteResourcePath = "Characters/MainCharacter";
        [SerializeField] private string downWalkResourcePath = "Characters/Walk";
        [SerializeField] private string upWalkResourcePath = "Characters/WalkUp";
        [SerializeField] private string leftWalkResourcePath = "Characters/WalkLeft";
        [SerializeField] private string rightWalkResourcePath = "Characters/WalkRight";
        private ReadOnlyCollection<Vector2Int> obstacleOffsetsView;

        public Vector2Int GridSize => gridSize;
        public Vector2Int PlayerStartCell => playerStartCell;
        public string EncounterName => encounterName;
        public DemonDefinition WildDemon => wildDemon;
        public Vector2Int DemonCell => playerStartCell + demonOffset;
        public IReadOnlyList<Vector2Int> ObstacleOffsets =>
            obstacleOffsetsView ?? (obstacleOffsetsView = Array.AsReadOnly(obstacleOffsets ?? Array.Empty<Vector2Int>()));
        public string IdleSpriteResourcePath => idleSpriteResourcePath;
        public string DownWalkResourcePath => downWalkResourcePath;
        public string UpWalkResourcePath => upWalkResourcePath;
        public string LeftWalkResourcePath => leftWalkResourcePath;
        public string RightWalkResourcePath => rightWalkResourcePath;

        public bool TryValidate(out string error)
        {
            if (gridSize.x <= 0 || gridSize.y <= 0)
            {
                error = "Grid dimensions must both be greater than zero.";
                return false;
            }

            if (!IsWithinGrid(playerStartCell))
            {
                error = $"Player start cell {playerStartCell} is outside the grid.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(encounterName))
            {
                error = "Encounter name must not be empty.";
                return false;
            }

            if (wildDemon == null)
            {
                error = "The wild demon is missing.";
                return false;
            }

            if (!wildDemon.TryCreate(out _, out var wildDemonError))
            {
                error = $"The wild demon is invalid: {wildDemonError}";
                return false;
            }

            if (string.IsNullOrWhiteSpace(idleSpriteResourcePath) ||
                string.IsNullOrWhiteSpace(downWalkResourcePath) ||
                string.IsNullOrWhiteSpace(upWalkResourcePath) ||
                string.IsNullOrWhiteSpace(leftWalkResourcePath) ||
                string.IsNullOrWhiteSpace(rightWalkResourcePath))
            {
                error = "Character sprite resource paths must not be empty.";
                return false;
            }

            if (!IsWithinGrid(DemonCell) || DemonCell == playerStartCell)
            {
                error = $"Demon cell {DemonCell} must be inside the grid and separate from the player.";
                return false;
            }

            if (obstacleOffsets == null)
            {
                error = "Obstacle offsets must not be null.";
                return false;
            }

            var cells = new HashSet<Vector2Int>();
            foreach (var offset in obstacleOffsets)
            {
                var cell = playerStartCell + offset;
                if (!IsWithinGrid(cell) || cell == playerStartCell || cell == DemonCell || !cells.Add(cell))
                {
                    error = $"Obstacle cell {cell} is outside the grid, overlaps a reserved cell, or is duplicated.";
                    return false;
                }
            }

            error = null;
            return true;
        }

        private bool IsWithinGrid(Vector2Int cell)
        {
            return cell.x >= 0 && cell.x < gridSize.x && cell.y >= 0 && cell.y < gridSize.y;
        }

        private void OnValidate()
        {
            obstacleOffsetsView = null;
        }
    }
}
