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
