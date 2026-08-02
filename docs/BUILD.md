# Build guide

## Required Unity installation

Install Unity `6000.3.18f1` with:

- Android Build Support;
- Android SDK & NDK Tools;
- OpenJDK.

## Prepare the project

Switch to branch:

```text
develop/0.1.1-alpha
```

Open the repository root in Unity Hub and wait until package import and script compilation finish. The editor bootstrap configures the Android player and generates the Luma Bay lighthouse application icon under the ignored local folder `Assets/LumaBay/Generated`.

The setup can be repeated manually from:

```text
Luma Bay → Setup Project
```

## Editor build

Select:

```text
Luma Bay → Build Android Alpha
```

The build script creates the control APK at:

```text
Builds/Android/LumaBay-0.1.1-alpha.apk
```

The 0.1.1 control build uses:

- ARM64;
- IL2CPP;
- medium managed-code stripping;
- portrait orientation;
- Android 8.0 minimum;
- non-development build options;
- debug signing for internal testing.

The `Development Build` watermark is therefore not expected in this APK.

## Command-line build

Windows example:

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.3.18f1\Editor\Unity.exe" `
  -batchmode -quit `
  -projectPath "$PWD" `
  -executeMethod LumaBay.Editor.LumaBayBuildScript.BuildAndroid `
  -logFile "Builds/unity-build.log"
```

## Install without deleting progress

```powershell
adb install -r "Builds\Android\LumaBay-0.1.1-alpha.apk"
```

The package ID remains `com.norvexa.lumabay`, and save schema migration preserves progress from the tested 0.1.0 Alpha.

## GitHub Actions

The optional `Build Android Alpha` workflow uses GameCI and is manual. It requires repository secrets appropriate to the selected Unity activation method, such as:

- `UNITY_LICENSE`;
- `UNITY_EMAIL`;
- `UNITY_PASSWORD`.

Local Unity builds remain the primary control-build path so GitHub Actions minutes are not consumed for every change.

## Release signing

The 0.1.1 control APK still uses debug signing. Before Google Play distribution:

1. create a protected upload keystore outside Git;
2. store its values as encrypted local or CI secrets;
3. generate an Android App Bundle;
4. verify target API requirements;
5. run closed testing and Play pre-launch reports.
