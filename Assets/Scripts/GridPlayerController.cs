using UnityEngine;

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

        Vector2Int moveDirection = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveDirection = new Vector2Int(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDirection = new Vector2Int(0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveDirection = new Vector2Int(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
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
