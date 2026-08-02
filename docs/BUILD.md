# Build guide

## Required Unity installation

Install Unity `6000.3.18f1` with:

- Android Build Support;
- Android SDK & NDK Tools;
- OpenJDK.

## Editor build

Open the project and select:

```text
Luma Bay → Build Android Alpha
```

The build script creates a development APK at:

```text
Builds/Android/LumaBay-0.1.0-alpha.apk
```

## Command-line build

Windows example:

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.3.18f1\Editor\Unity.exe" `
  -batchmode -quit `
  -projectPath "$PWD" `
  -executeMethod LumaBay.Editor.LumaBayBuildScript.BuildAndroid `
  -logFile "Builds/unity-build.log"
```

## GitHub Actions

The manual `Build Android Alpha` workflow uses GameCI. Add repository secrets:

- `UNITY_LICENSE` — the contents of a valid Unity Personal/Pro license file;
- `UNITY_EMAIL` — Unity account email when required by the selected GameCI activation method;
- `UNITY_PASSWORD` — Unity account password when required.

The workflow uploads the APK as `LumaBay-Android-Alpha`.

## Release signing

Version 0.1 Alpha intentionally uses a development build and debug signing. Before public distribution, create a protected upload keystore outside Git, store it as encrypted CI secrets, disable `Development Build`, and generate an Android App Bundle.
