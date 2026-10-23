using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUnityGame.Gameplay
{
    public class KeyboardGridMovementInput : MonoBehaviour, IGridMovementInput
    {
        public Vector2Int ReadMoveDirection()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2Int.zero;
            }

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                return Vector2Int.up;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                return Vector2Int.down;
            }

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                return Vector2Int.left;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                return Vector2Int.right;
            }

            return Vector2Int.zero;
        }
    }
}
