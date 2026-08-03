# Build guide

## Required Unity installation

Install Unity `6000.3.18f1` with:

- Android Build Support;
- Android SDK & NDK Tools;
- OpenJDK.

If the build script reports that Android is unsupported, open Unity Hub, select the editor installation, choose **Add modules**, and install all three Android components above.

## Prepare the project

Switch to:

```text
develop/0.1.2-alpha
```

Open the repository root in Unity Hub. During the first clean import Unity will:

1. compile the committed runtime and editor code;
2. create the start scene when missing;
3. generate the application icon;
4. generate the premium PNG art pack;
5. generate the original WAV audio pack;
6. import and compress the generated assets.

The first import is intentionally longer than 0.1.1 because it creates 32 lighthouse scenes, six pieces, five boosters, UI textures, ambience, music and SFX. Do not close Unity while the progress bar is active.

Generated files are reproducible and ignored under:

```text
Assets/LumaBay/Resources/
Assets/LumaBay/Generated/
```

Manual regeneration commands:

```text
Luma Bay → Generate Premium Art Pack
Luma Bay → Generate Game Audio Pack
Luma Bay → Generate Branding
Luma Bay → Setup Project
```

## Editor build

After the Console has no red errors, select:

```text
Luma Bay → Build Android Alpha
```

The build command now checks that Android Build Support is installed and switches the active editor platform to Android when necessary.

Output:

```text
Builds/Android/LumaBay-0.1.2-alpha.apk
```

Diagnostic report:

```text
Builds/Android/LumaBay-0.1.2-alpha-build-report.txt
```

Configuration:

- ARM64;
- IL2CPP;
- medium managed-code stripping;
- portrait orientation;
- Android 8.0 minimum;
- non-development build;
- debug signing for internal testing.

## Command-line build

```powershell
& "C:\Program Files\Unity\Hub\Editor\6000.3.18f1\Editor\Unity.exe" `
  -batchmode -nographics -quit `
  -buildTarget Android `
  -projectPath "$PWD" `
  -executeMethod LumaBay.Editor.LumaBayBuildScript.BuildAndroid `
  -logFile "Builds/Android/LumaBay-0.1.2-alpha-build.log"
```

The `-buildTarget Android` argument is required in batch mode. Unity cannot switch the active build target from inside an executing batch-mode method.

A command-line build should be run only after the generated Resources assets exist from one editor import. The repository root also contains `Build-Android-Alpha.bat`, which supplies the correct target and prints the end of the Unity log when a build fails.

## Install without deleting progress

```powershell
adb install -r "Builds\Android\LumaBay-0.1.2-alpha.apk"
```

The package remains `com.norvexa.lumabay`. Save schema version 3 migrates the previous level, star, coin and lighthouse progress.

## Local validation

```powershell
python tools\validate_project.py
```

Unity EditMode tests are available from:

```text
Window → General → Test Runner → EditMode
```

## GitHub Actions

Workflows remain manual to avoid spending Actions minutes on every art iteration. Local Unity builds are the primary validation path because Android compilation requires a valid Unity installation and activation.

## Public release preparation

The control APK still uses debug signing. Before Google Play distribution:

1. create an upload keystore outside Git;
2. move signing values to protected secrets;
3. build an Android App Bundle;
4. verify target API and Data Safety requirements;
5. run closed testing and Play pre-launch reports;
6. profile memory, startup time and frame pacing on several Android devices.
