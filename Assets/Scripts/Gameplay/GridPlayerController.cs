using UnityEngine;
using UnityEngine.InputSystem;

namespace MyUnityGame.Gameplay
{
    public class GridPlayerController : MonoBehaviour
    {
        private const float MoveDurationWithoutAnimation = 0.2f;

        private GridPrototypeBootstrap world;
        private SpriteWalkAnimator walkAnimator;
        private Collider playerCollider;
        private Vector2Int currentCell;
        private Vector2Int targetCell;
        private Vector3 moveStartPosition;
        private Vector3 moveTargetPosition;
        private float moveDuration;
        private float moveElapsed;
        private bool isMoving;

        public Vector2Int CurrentCell => currentCell;

        public void Initialize(Vector2Int startCell, GridPrototypeBootstrap bootstrap)
        {
            world = bootstrap;
            walkAnimator = GetComponentInChildren<SpriteWalkAnimator>();
            playerCollider = GetComponent<Collider>();
            currentCell = startCell;
            transform.position = world.GridToWorld(currentCell);
        }

        private void Update()
        {
            if (world == null)
            {
                return;
            }

            if (isMoving)
            {
                UpdateMovement();
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

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                moveDirection = new Vector2Int(0, 1);
            }
            else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                moveDirection = new Vector2Int(0, -1);
            }
            else if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                moveDirection = new Vector2Int(-1, 0);
            }
            else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
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
            targetCell = currentCell + direction;

            if (!world.IsWithinGrid(targetCell) ||
                world.IsBlocked(targetCell) ||
                HasBlockingColliderAt(targetCell))
            {
                return;
            }

            moveStartPosition = transform.position;
            moveTargetPosition = world.GridToWorld(targetCell);
            moveElapsed = 0f;
            moveDuration = walkAnimator != null
                ? Mathf.Max(MoveDurationWithoutAnimation, walkAnimator.PlayWalk(direction))
                : MoveDurationWithoutAnimation;
            isMoving = true;
        }

        private bool HasBlockingColliderAt(Vector2Int cell)
        {
            if (playerCollider == null)
            {
                return false;
            }

            var targetPosition = world.GridToWorld(cell);
            var colliderCenterOffset = playerCollider.bounds.center - transform.position;
            var overlaps = Physics.OverlapSphere(
                targetPosition + colliderCenterOffset,
                playerCollider.bounds.extents.x,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);

            foreach (var overlap in overlaps)
            {
                if (overlap.transform != transform && !overlap.transform.IsChildOf(transform))
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateMovement()
        {
            moveElapsed = Mathf.Min(moveElapsed + Time.deltaTime, moveDuration);
            var progress = moveElapsed / moveDuration;
            transform.position = Vector3.Lerp(moveStartPosition, moveTargetPosition, progress);

            if (moveElapsed >= moveDuration)
            {
                transform.position = moveTargetPosition;
                currentCell = targetCell;
                isMoving = false;
            }
        }
    }
}
