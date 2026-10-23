using System;
using MyUnityGame.Battle;
using UnityEngine;

namespace MyUnityGame.Gameplay
{
    internal static class GridActorFactory
    {
        private const float SpriteGroundClearance = 0.04f;
        private const float CameraFocusHeight = 0.75f;
        private static readonly Vector3 CameraFollowOffset = new Vector3(0f, 7.5f, -6.5f);

        public static GridPlayerController CreatePlayer(
            Transform parent,
            Vector2Int startCell,
            IGridWorld world,
            GridWorldSettings settings)
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.tag = "Player";
            player.name = "Player";
            player.transform.SetParent(parent, true);
            player.transform.position = world.GridToWorld(startCell);

            var rigidbody = player.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            rigidbody.useGravity = false;

            player.GetComponent<MeshRenderer>().enabled = false;
            var playerCollider = player.GetComponent<CapsuleCollider>();
            playerCollider.radius = 0.25f;
            playerCollider.height = 0.7f;
            playerCollider.center = new Vector3(0f, 0.35f, 0f);

            CreatePlayerSprite(player.transform, settings);

            var controller = player.AddComponent<GridPlayerController>();
            var movementInput = player.AddComponent<KeyboardGridMovementInput>();
            controller.Initialize(startCell, world, movementInput);
            return controller;
        }

        private static void CreatePlayerSprite(Transform player, GridWorldSettings settings)
        {
            var idleSprite = Resources.Load<Sprite>(settings.IdleSpriteResourcePath);
            if (idleSprite == null)
            {
                Debug.LogError($"Idle sprite is missing from Resources/{settings.IdleSpriteResourcePath}.", player);
                return;
            }

            var spriteObject = new GameObject("MainCharacterSprite");
            spriteObject.transform.SetParent(player, false);
            spriteObject.transform.localPosition = new Vector3(0f, SpriteGroundClearance, 0f);

            var spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = idleSprite;
            spriteRenderer.sortingOrder = 1;

            var animator = spriteObject.AddComponent<SpriteWalkAnimator>();
            animator.Initialize(spriteRenderer, idleSprite, LoadFrames(settings.DownWalkResourcePath));
            animator.SetUpwardWalkFrames(LoadFrames(settings.UpWalkResourcePath));
            animator.SetHorizontalWalkFrames(
                LoadFrames(settings.LeftWalkResourcePath),
                LoadFrames(settings.RightWalkResourcePath));
        }

        private static Sprite[] LoadFrames(string resourcePath)
        {
            var frames = Resources.LoadAll<Sprite>(resourcePath);
            Array.Sort(frames, (left, right) => string.CompareOrdinal(left.name, right.name));
            return frames;
        }

        public static GameObject CreateDemon(
            Transform parent,
            Vector2Int cell,
            IGridWorld world,
            GridPlayerController player,
            string encounterName,
            DemonDefinition wildDemon,
            Material material)
        {
            var demon = GameObject.CreatePrimitive(PrimitiveType.Cube);
            demon.name = $"{encounterName}Encounter";
            demon.transform.SetParent(parent, true);
            demon.transform.position = world.GridToWorld(cell) + Vector3.up * 0.6f;
            demon.transform.localScale = new Vector3(0.7f, 1.2f, 0.7f);

            var collider = demon.GetComponent<BoxCollider>();
            collider.isTrigger = true;
            demon.GetComponent<Renderer>().sharedMaterial = material;
            demon.AddComponent<GridEncounterTrigger>().Initialize(player, cell, encounterName, wildDemon);
            return demon;
        }

        public static void CreateCamera(Transform player)
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
            }

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.09f, 0.10f, 0.15f);
            camera.fieldOfView = 38f;
            camera.enabled = true;
            camera.gameObject.SetActive(true);
            camera.transform.SetParent(player, true);
            var focusPosition = player.position + Vector3.up * CameraFocusHeight;
            camera.transform.position = focusPosition + CameraFollowOffset;
            camera.transform.LookAt(focusPosition);
            camera.transform.localScale = Vector3.one;
        }
    }
}
