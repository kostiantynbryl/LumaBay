# Luma Bay: Match & Restore

Portrait mobile Match-3 restoration game for Android, developed under **Norvexa Games**.

## Current release target

**0.1.2 Alpha — Art Rebuild** replaces the 0.1.1 prototype presentation with a dedicated original asset, audio and animation pipeline:

- 60 progressively balanced Match-3 levels;
- 8×8 board, cascades, automatic reshuffle and four special-piece types;
- collection and fog goals plus crate, ice and net obstacles;
- five usable nautical boosters;
- animated swaps, invalid-move return, piece drops, match particles and idle move hints;
- 48 authored lighthouse tasks across six chapters;
- 32 visible lighthouse states from storm damage to a living coastal landmark;
- first lighthouse launch during the early campaign, followed by upgrades, pier, garden and observatory;
- generated premium PNG art pack: background, UI, pieces, boosters and lighthouse states;
- original casual-game SFX, coastal ambience and tonal background music;
- rebuilt main menu, map, gameplay HUD, 60-level selector, settings and result windows;
- versioned local saves with migration from 0.1.0 and 0.1.1;
- Russian and English interface;
- original project-generated content with no third-party game assets.

## Engine

- Unity **6000.3.18f1 / Unity 6.3 LTS**
- C#
- uGUI
- Android 8.0+
- ARM64 / IL2CPP
- package: `com.norvexa.lumabay`

## First project import

1. Install Unity 6000.3.18f1 with Android Build Support, SDK, NDK and OpenJDK.
2. Clone the repository and switch to `develop/0.1.2-alpha`.
3. Open the repository root in Unity Hub.
4. Wait for compilation and the one-time art/audio generation. Unity creates the reproducible files under `Assets/LumaBay/Resources/` locally.
5. Open `Assets/LumaBay/Scenes/LumaBayMain.unity` when needed.
6. Press Play.

The generated art/audio folders are ignored by Git because they are reproduced from the committed C# generators. Force regeneration with:

```text
Luma Bay → Generate Premium Art Pack
Luma Bay → Generate Game Audio Pack
```

## Android control build

Use:

```text
Luma Bay → Build Android Alpha
```

Output:

```text
Builds/Android/LumaBay-0.1.2-alpha.apk
```

The control APK is a non-development ARM64 IL2CPP build with medium managed-code stripping and debug signing for internal testing.

## Validation

Run the repository checks without a Unity license:

```text
python tools/validate_project.py
```

Unity EditMode tests cover board generation, obstacles, the 60-level campaign and the 48-task lighthouse roadmap. GitHub Actions workflows remain manual to avoid unnecessary usage.

## Branch policy

- `main` — reviewed milestones;
- `develop/0.1-alpha` — tested 0.1.0 baseline;
- `develop/0.1.1-alpha` — archived first premium-style experiment;
- `develop/0.1.2-alpha` — current full art/audio/animation rebuild.

## Legal

All source code and generated visual/audio assets are original Luma Bay materials. No Playrix, Gardenscapes, Homescapes or other third-party game assets are included.
