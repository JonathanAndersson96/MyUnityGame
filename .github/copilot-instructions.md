# Workspace Guidance

- This workspace is a Unity game project using C#.
- Follow Unity conventions: scripts that derive from `MonoBehaviour` should use matching filenames, and Unity-facing fields should use `[SerializeField]` when appropriate.
- Visual direction: 2.5D overworld inspired by Pokemon HeartGold and SoulSilver, using 3D environments, 2D-style characters, and a fixed elevated camera. Do not copy that game's characters, assets, or mechanics.
- Do not assume the game genre, platform, input scheme, or specific render pipeline unless the user specifies one.
- Avoid editing Unity-generated project metadata by hand; let the installed Unity Editor manage it.
- Keep changes scoped, and validate C# changes in Unity or with the project's configured editor tooling when available.
