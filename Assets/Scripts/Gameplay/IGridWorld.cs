using UnityEngine;

namespace MyUnityGame.Gameplay
{
    public interface IGridWorld
    {
        Vector3 GridToWorld(Vector2Int cell);
        bool IsWithinGrid(Vector2Int cell);
        bool IsBlocked(Vector2Int cell);
    }
}
