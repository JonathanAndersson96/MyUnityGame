# Unity Game

A Unity 6.6.4f1 game project for a 2.5D overworld: 3D environments, 2D-style characters, and an elevated camera, inspired by the presentation of Pokemon HeartGold and SoulSilver. It uses the Universal Render Pipeline (URP).

## Open in Unity

1. In Unity Hub, add or open the existing project at `C:\development\MyUnityGame`.
2. Use Editor version `6000.6.4f1`. On first open, allow Unity to resolve packages and import assets.
3. The starter scene is `Assets/Scenes/SampleScene.unity`. Save future scenes in `Assets/Scenes` and set the desired startup scene in Build Profiles.

## Folders

- `Assets/Scenes` - game scenes
- `Assets/Scripts` - C# scripts
- `Assets/Prefabs` - reusable GameObjects
- `Assets/Materials` - materials and shaders
- `Assets/Art` - visual assets
- `Assets/Audio` - sound and music
- `Assets/Settings` - URP render pipeline and volume settings

The project uses a 3D URP foundation suited to 2.5D environments. The game direction is grid-based exploration, battles, and survival in Aujurlandar during the empire's heyday, with roaming demons as the threat and nuzlocke-style rules as the default. See [pre-Collapse game canon](docs/worldbuilding_pre_collapse.md) and the [post-Collapse reference](docs/worldbuilding_post_collapse.md).

## Runtime systems

- `Assets/Scripts/Core` owns scene-independent game state and bootstraps persistent services.
- `Assets/Scripts/Battle` is the battle library (`MyUnityGame.Battle`): demons, stats, investment, types, damage and the battle engine. It has no scene dependencies and is covered by the EditMode tests in `Assets/Tests/EditMode`.
- `Assets/Scripts/Data` contains the game phase.
- `Assets/Scripts/Gameplay` contains grid-world composition, movement contracts and adapters, party state, encounter handling, and the battle scene's connection to the battle library.
- `Assets/Scripts/UI` contains presentation components; battle presentation is separated from battle state.
- `Assets/Scripts/UI/GameMenu.cs` is the Tab menu, built with UI Toolkit from `Assets/Resources/UI`. Each entry is an `IGameMenuPage`; Journal, Character, Bag and Save are placeholders.
- `Assets/Resources/Gameplay/GridWorldSettings.asset` configures the prototype grid, the wild encounter (species, level and moves), obstacles, and character sprite resource paths.
- `Assets/Resources/Battle` holds placeholder battle content: species, moves, the starting party and the battle tuning.

Grid movement depends on `IGridWorld` and `IGridMovementInput`; the current keyboard input is an adapter, so alternate input schemes and world implementations can be supplied without changing the movement controller. The party of demons is kept by the persistent party service across scene changes. The battle library owns the battle rules (terms are defined in the [glossary](GLOSSARY.md)), and the battle manager and HUD only pass on the player's choices and show the result. HP persists between battles, banished demons leave the party for good, and defeated encounters stay cleared for the current game session.
