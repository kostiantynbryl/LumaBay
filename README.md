# Luma Bay: Match & Restore

Portrait mobile Match-3 restoration game for Android, developed under **Norvexa Games**.

## Current release target

**0.1 Alpha** — a playable vertical slice with:

- 30 deterministic Match-3 levels;
- 8×8 board, cascades, reshuffle and four special pieces;
- collection and fog-clearing goals;
- level progression, stars, coins and local save;
- lighthouse restoration meta loop;
- Russian and English interface;
- procedural maritime artwork, so the repository has no unlicensed stock assets;
- accessibility symbols on every colored piece;
- Android portrait configuration and CI build workflow.

## Engine

- Unity **6000.3.18f1 / Unity 6.3 LTS**
- C#
- uGUI
- Android 8.0+
- package: `com.norvexa.lumabay`

Unity 6.3 LTS is the production baseline for this branch. Open the repository folder in Unity Hub. On the first script reload, the editor bootstrap automatically creates `Assets/LumaBay/Scenes/LumaBayMain.unity`, configures Android settings, and adds the scene to Build Settings.

## Run locally

1. Install Unity 6000.3.18f1 with Android Build Support, SDK, NDK and OpenJDK.
2. Clone the repository and open its root directory in Unity Hub.
3. Wait for package import and script compilation.
4. Open `Assets/LumaBay/Scenes/LumaBayMain.unity` if it is not already open.
5. Press Play.

The bootstrap can be run manually from **Luma Bay → Setup Project**.

## Android build

Use **Luma Bay → Build Android Alpha** or run:

```powershell
Unity.exe -batchmode -quit -projectPath . -executeMethod LumaBay.Editor.LumaBayBuildScript.BuildAndroid
```

Output:

```text
Builds/Android/LumaBay-0.1.0-alpha.apk
```

The GitHub Actions Android workflow requires a Unity license secret. See [`docs/BUILD.md`](docs/BUILD.md).

## Branch policy

- `main` — reviewed milestones;
- `develop/0.1-alpha` — active alpha implementation;
- feature branches — isolated systems and fixes.

## Legal

All original source code and procedurally generated visual assets are copyright Norvexa Games. No Playrix, Gardenscapes, Homescapes or other third-party game assets are included.
