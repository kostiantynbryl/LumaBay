#!/usr/bin/env python3
"""Fast repository checks that do not require a Unity license."""

from __future__ import annotations

import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
REQUIRED = [
    "Packages/manifest.json",
    "ProjectSettings/ProjectVersion.txt",
    "Assets/LumaBay/Runtime/LumaBayGame.cs",
    "Assets/LumaBay/Runtime/LumaBayGame.Gameplay.cs",
    "Assets/LumaBay/Runtime/LumaBayGame.Screens.cs",
    "Assets/LumaBay/Runtime/LumaBayGame.UI.cs",
    "Assets/LumaBay/Runtime/LumaBayGame.Goals.cs",
    "Assets/LumaBay/Runtime/LumaBayGame.Obstacles.cs",
    "Assets/LumaBay/Runtime/LumaBayGame.ArtOverrides.cs",
    "Assets/LumaBay/Runtime/LumaBayGame.MoveAnimation.cs",
    "Assets/LumaBay/Runtime/Meta/LighthouseTaskCatalog.cs",
    "Assets/LumaBay/Runtime/Model/Match3Board.cs",
    "Assets/LumaBay/Runtime/Model/BoardBoosters.cs",
    "Assets/LumaBay/Runtime/Model/LevelCatalog.cs",
    "Assets/LumaBay/Runtime/Presentation/LumaBayArtPack.cs",
    "Assets/LumaBay/Runtime/Presentation/BoardCellThemeInstaller.cs",
    "Assets/LumaBay/Runtime/Presentation/AudioSynth.cs",
    "Assets/LumaBay/Runtime/Presentation/NauticalTheme.cs",
    "Assets/LumaBay/Runtime/Presentation/ProceduralArt.cs",
    "Assets/LumaBay/Runtime/Presentation/UiMotion.cs",
    "Assets/LumaBay/Runtime/Presentation/UiBurstParticle.cs",
    "Assets/LumaBay/Runtime/Presentation/PieceDropMotion.cs",
    "Assets/LumaBay/Runtime/Services/SaveService.cs",
    "Assets/LumaBay/Editor/LumaBayProjectBootstrap.cs",
    "Assets/LumaBay/Editor/LumaBayBuildScript.cs",
    "Assets/LumaBay/Editor/LumaBayBrandingGenerator.cs",
    "Assets/LumaBay/Editor/LumaBayArtPackGenerator.cs",
    "Assets/LumaBay/Editor/LumaBayAudioPackGenerator.cs",
]


def fail(message: str) -> None:
    print(f"ERROR: {message}", file=sys.stderr)
    raise SystemExit(1)


def read(relative: str) -> str:
    return (ROOT / relative).read_text(encoding="utf-8")


def check_required() -> None:
    for relative in REQUIRED:
        if not (ROOT / relative).is_file():
            fail(f"missing required file: {relative}")


def check_manifest() -> None:
    manifest = json.loads(read("Packages/manifest.json"))
    dependencies = manifest.get("dependencies", {})
    for package in ("com.unity.ugui", "com.unity.test-framework"):
        if package not in dependencies:
            fail(f"required Unity package is not declared: {package}")


def check_version() -> None:
    if "6000.3.18f1" not in read("ProjectSettings/ProjectVersion.txt"):
        fail("unexpected Unity version")

    sources = {
        "runtime": read("Assets/LumaBay/Runtime/LumaBayGame.cs"),
        "bootstrap": read("Assets/LumaBay/Editor/LumaBayProjectBootstrap.cs"),
        "build": read("Assets/LumaBay/Editor/LumaBayBuildScript.cs"),
    }
    for name, source in sources.items():
        if "0.1.2-alpha" not in source:
            fail(f"0.1.2-alpha version missing from {name}")
    if "LumaBay-0.1.2-alpha.apk" not in sources["build"]:
        fail("0.1.2 APK output path is missing")


def check_mobile_configuration() -> None:
    bootstrap = read("Assets/LumaBay/Editor/LumaBayProjectBootstrap.cs")
    build = read("Assets/LumaBay/Editor/LumaBayBuildScript.cs")
    for token in (
        "com.norvexa.lumabay",
        "bundleVersionCode = 3",
        "AndroidArchitecture.ARM64",
        "ScriptingImplementation.IL2CPP",
        "ManagedStrippingLevel.Medium",
        "UIOrientation.Portrait",
    ):
        if token not in bootstrap:
            fail(f"Android configuration token missing: {token}")
    if "BuildOptions.Development" in build or "development = true" in build:
        fail("0.1.2 control build must not be a development build")


def check_campaign_and_meta() -> None:
    levels = read("Assets/LumaBay/Runtime/Model/LevelCatalog.cs")
    tasks = read("Assets/LumaBay/Runtime/Meta/LighthouseTaskCatalog.cs")
    save = read("Assets/LumaBay/Runtime/Services/SaveService.cs")

    if "CampaignLevelCount = 60" not in levels:
        fail("campaign must contain 60 levels")
    if tasks.count("T(\"") < 48:
        fail("lighthouse roadmap must contain at least 48 authored tasks")
    for token in ("SaveVersion = 3", "LighthouseTaskIndex", "MusicEnabled", "BackupKey"):
        if token not in save:
            fail(f"save migration token missing: {token}")


def check_art_and_audio_pipeline() -> None:
    art_loader = read("Assets/LumaBay/Runtime/Presentation/LumaBayArtPack.cs")
    art_generator = read("Assets/LumaBay/Editor/LumaBayArtPackGenerator.cs")
    audio_generator = read("Assets/LumaBay/Editor/LumaBayAudioPackGenerator.cs")
    audio_runtime = read("Assets/LumaBay/Runtime/Presentation/AudioSynth.cs")
    screens = read("Assets/LumaBay/Runtime/LumaBayGame.Screens.cs")
    gameplay = read("Assets/LumaBay/Runtime/LumaBayGame.Gameplay.cs")

    for token in ("LighthouseState", "TintedPanel", "ArtPack/backgrounds", "ArtPack/pieces"):
        if token not in art_loader:
            fail(f"art-pack loader token missing: {token}")
    for token in ("GenerateLighthouseStates", "GeneratePieces", "GenerateBoosters", "lighthouse_31.png"):
        if token not in art_generator:
            fail(f"art generator token missing: {token}")
    for token in ("ui_click", "swap", "invalid", "cascade", "restore", "ambience", "music"):
        if token not in audio_generator or token not in audio_runtime:
            fail(f"audio bank token missing: {token}")
    for token in ("CreateLighthouseVisual", "LighthouseTaskCatalog.Count", "columns = 3"):
        if token not in screens:
            fail(f"premium screen token missing: {token}")
    for token in ("BeginAnimatedMove", "PlayBooster", "PlayLose", "BoardFrame"):
        if token not in gameplay:
            fail(f"gameplay presentation token missing: {token}")


def check_gameplay_features() -> None:
    game_types = read("Assets/LumaBay/Runtime/Model/GameTypes.cs")
    gameplay = read("Assets/LumaBay/Runtime/LumaBayGame.Gameplay.cs")
    board = read("Assets/LumaBay/Runtime/Model/Match3Board.cs")
    localization = read("Assets/LumaBay/Runtime/Services/Localization.cs")
    goals = read("Assets/LumaBay/Runtime/LumaBayGame.Goals.cs")
    obstacles = read("Assets/LumaBay/Runtime/LumaBayGame.Obstacles.cs")

    for obstacle in ("Crate", "Ice", "Net"):
        if obstacle not in game_types or obstacle not in board:
            fail(f"obstacle is not fully declared: {obstacle}")
    for booster in ("UseLightningBolt", "UseAnchorBomb", "BuyShuffle", "BuyExtraMoves", "UseMagicHarpoon"):
        if booster not in gameplay:
            fail(f"booster handler missing: {booster}")
    for key in ("tutorial_swipe", "tutorial_boosters", "booster_lightning", "booster_harpoon"):
        if key not in localization:
            fail(f"localization key missing: {key}")
    if "TargetGoalIcon" not in goals or "FogGoalIcon" not in goals:
        fail("visual goal icons are incomplete")
    if "SpawnBoardBurst" not in obstacles or "UiBurstParticle" not in obstacles:
        fail("board match sparkle effect is incomplete")


def check_unity_lifecycle() -> None:
    sources = "\n".join(
        path.read_text(encoding="utf-8")
        for path in ROOT.glob("Assets/LumaBay/Runtime/LumaBayGame*.cs")
    )
    update_count = len(re.findall(r"\bprivate\s+void\s+Update\s*\(\s*\)", sources))
    late_update_count = len(re.findall(r"\bprivate\s+void\s+LateUpdate\s*\(\s*\)", sources))
    if update_count != 1:
        fail(f"LumaBayGame must define exactly one Update method, found {update_count}")
    if late_update_count != 0:
        fail(f"LumaBayGame must not define LateUpdate methods, found {late_update_count}")
    for helper in ("RefreshGoalPresentationIfNeeded", "RefreshBoardPresentationIfNeeded", "RefreshArtOverridesIfNeeded"):
        if helper not in sources:
            fail(f"central lifecycle helper missing: {helper}")


def check_meta_files() -> None:
    for source in REQUIRED:
        if not source.startswith("Assets/") or not source.endswith(".cs"):
            continue
        meta = ROOT / f"{source}.meta"
        if not meta.is_file():
            fail(f"missing Unity meta file: {meta.relative_to(ROOT)}")


def check_csharp_balance() -> None:
    for path in ROOT.glob("Assets/**/*.cs"):
        text = path.read_text(encoding="utf-8")
        stripped = re.sub(r'@?"(?:""|\\.|[^"\\])*"', '""', text)
        stripped = re.sub(r"//.*?$|/\*.*?\*/", "", stripped, flags=re.MULTILINE | re.DOTALL)
        if stripped.count("{") != stripped.count("}"):
            fail(f"unbalanced braces in {path.relative_to(ROOT)}")
        if "TODO_FATAL" in text:
            fail(f"fatal marker in {path.relative_to(ROOT)}")


def main() -> None:
    check_required()
    check_manifest()
    check_version()
    check_mobile_configuration()
    check_campaign_and_meta()
    check_art_and_audio_pipeline()
    check_gameplay_features()
    check_unity_lifecycle()
    check_meta_files()
    check_csharp_balance()
    print("Luma Bay 0.1.2 art rebuild static validation passed.")


if __name__ == "__main__":
    main()
