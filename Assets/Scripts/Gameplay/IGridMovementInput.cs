using UnityEngine;

namespace MyUnityGame.Gameplay
{
    public interface IGridMovementInput
    {
        Vector2Int ReadMoveDirection();
    }
}
