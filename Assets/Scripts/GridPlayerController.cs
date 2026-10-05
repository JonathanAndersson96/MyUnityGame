using UnityEngine;
using UnityEngine.InputSystem;

public class GridPlayerController : MonoBehaviour
{
    private GridPrototypeBootstrap world;
    private Vector2Int currentCell;

    public Vector2Int CurrentCell => currentCell;

    public void Initialize(Vector2Int startCell, GridPrototypeBootstrap bootstrap)
    {
        world = bootstrap;
        currentCell = startCell;
        transform.position = world.GridToWorld(currentCell);
    }

    private void Update()
    {
        if (world == null)
        {
            return;
        }

        if (BattleManager.Instance != null && BattleManager.Instance.IsActive)
        {
            return;
        }

        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        Vector2Int moveDirection = Vector2Int.zero;

        if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
        {
            moveDirection = new Vector2Int(0, 1);
        }
        else if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
        {
            moveDirection = new Vector2Int(0, -1);
        }
        else if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
        {
            moveDirection = new Vector2Int(-1, 0);
        }
        else if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
        {
            moveDirection = new Vector2Int(1, 0);
        }

        if (moveDirection == Vector2Int.zero)
        {
            return;
        }

        TryMove(moveDirection);
    }

    private void TryMove(Vector2Int direction)
    {
        var targetCell = currentCell + direction;

        if (!world.IsWithinGrid(targetCell) || world.IsBlocked(targetCell))
        {
            return;
        }

        currentCell = targetCell;
        transform.position = world.GridToWorld(currentCell);
    }
}
