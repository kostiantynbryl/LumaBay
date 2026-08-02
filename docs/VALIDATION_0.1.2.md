# Luma Bay 0.1.2 control validation

## Automated source checks

- Unity project version and required packages
- Android ARM64 / IL2CPP release configuration
- 60-level campaign declaration
- 48-task lighthouse roadmap and six chapters
- generated PNG art and WAV audio pipelines
- save version 3 and previous-alpha migration
- swap, invalid-return and idle-hint animation systems
- obstacle and booster systems
- stable Unity `.meta` files
- one central `LumaBayGame.Update()` lifecycle method
- balanced C# braces and required source files

## Unity control pass

1. Clean import in Unity 6000.3.18f1.
2. Confirm art/audio generation finishes without red Console errors.
3. Run all EditMode tests.
4. Play the menu, map, level selector and first gameplay level.
5. Build `LumaBay-0.1.2-alpha.apk`.
6. Install over 0.1.0/0.1.1 and confirm save migration.
7. Check swap, invalid return, hints, boosters, obstacles, victory, defeat and lighthouse task progression on Android.
