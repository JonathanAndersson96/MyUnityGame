using System;
using MyUnityGame.Core;
using UnityEngine;

namespace MyUnityGame.Gameplay
{
    [RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
    public class GridPlayerController : MonoBehaviour
    {
        private const float MoveDurationWithoutAnimation = 0.2f;
        private const int InitialColliderBufferSize = 8;

        private IGridWorld world;
        private IGridMovementInput movementInput;
        private SpriteWalkAnimator walkAnimator;
        private Collider playerCollider;
        private Collider[] colliderBuffer = new Collider[InitialColliderBufferSize];
        private Vector2Int currentCell;
        private Vector2Int targetCell;
        private Vector3 moveStartPosition;
        private Vector3 moveTargetPosition;
        private float moveDuration;
        private float moveElapsed;
        private bool isMoving;

        public Vector2Int CurrentCell => currentCell;

        public void Initialize(Vector2Int startCell, IGridWorld gridWorld, IGridMovementInput input)
        {
            if (gridWorld == null)
            {
                throw new ArgumentNullException(nameof(gridWorld));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (!gridWorld.IsWithinGrid(startCell))
            {
                throw new ArgumentOutOfRangeException(nameof(startCell), "The player start cell must be within the grid.");
            }

            world = gridWorld;
            movementInput = input;
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

            if (BattleManager.Instance != null && BattleManager.Instance.IsActive)
            {
                return;
            }

            if (GameManager.Instance != null && GameManager.Instance.IsMenuOpen)
            {
                return;
            }

            if (isMoving)
            {
                UpdateMovement();
                return;
            }

            var moveDirection = movementInput.ReadMoveDirection();
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
            if (playerCollider == null || !playerCollider.enabled)
            {
                Debug.LogError("Player movement requires an enabled collider; rejecting the move.", this);
                return true;
            }

            var targetPosition = world.GridToWorld(cell);
            var colliderCenterOffset = playerCollider.bounds.center - transform.position;
            var overlapCount = Physics.OverlapSphereNonAlloc(
                targetPosition + colliderCenterOffset,
                playerCollider.bounds.extents.x,
                colliderBuffer,
                ~0,
                QueryTriggerInteraction.Ignore);
            while (overlapCount == colliderBuffer.Length)
            {
                Array.Resize(ref colliderBuffer, checked(colliderBuffer.Length * 2));
                overlapCount = Physics.OverlapSphereNonAlloc(
                    targetPosition + colliderCenterOffset,
                    playerCollider.bounds.extents.x,
                    colliderBuffer,
                    ~0,
                    QueryTriggerInteraction.Ignore);
            }

            for (var i = 0; i < overlapCount; i++)
            {
                var overlap = colliderBuffer[i];
                if (overlap != null && overlap.transform != transform && !overlap.transform.IsChildOf(transform))
                {
                    Array.Clear(colliderBuffer, 0, overlapCount);
                    return true;
                }
            }

            Array.Clear(colliderBuffer, 0, overlapCount);
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
