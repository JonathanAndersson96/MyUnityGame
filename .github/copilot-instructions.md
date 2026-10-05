# Workspace Guidance

- This workspace is a Unity game project using C#.
- Follow Unity conventions: scripts that derive from `MonoBehaviour` should use matching filenames, and Unity-facing fields should use `[SerializeField]` when appropriate.
- Visual direction: 2.5D overworld inspired by Pokemon HeartGold and SoulSilver, using 3D environments, 2D-style characters, and a fixed elevated camera. Do not copy that game's characters, assets, or mechanics.
- Do not assume the game genre, platform, input scheme, or specific render pipeline unless the user specifies one.
- Avoid editing Unity-generated project metadata by hand; let the installed Unity Editor manage it.
- Keep changes scoped, and validate C# changes in Unity or with the project's configured editor tooling when available.
- Before adding any feature, system, or design mechanic, ask for explicit user approval instead of guessing or assuming intent.
- Do not broaden scope beyond the current request. If a task is ambiguous, ask one clarifying question before making changes.
- Prefer simple, clean, scalable, and maintainable code. Favor small, reusable abstractions over duplicated state and hardcoded values.
- Avoid spreading the same configuration or HP values across unrelated scripts. Centralize shared data when it is reused.
- Keep class responsibilities narrow. Do not mix battle logic, world logic, and UI logic in one place unless the task is intentionally tiny and local.
