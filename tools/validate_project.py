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
    "Assets/LumaBay/Runtime/Model/Match3Board.cs",
    "Assets/LumaBay/Runtime/Model/BoardBoosters.cs",
    "Assets/LumaBay/Runtime/Presentation/NauticalTheme.cs",
    "Assets/LumaBay/Runtime/Presentation/ProceduralArt.cs",
    "Assets/LumaBay/Runtime/Presentation/CoastalBackdropArt.cs",
    "Assets/LumaBay/Runtime/Presentation/UiMotion.cs",
    "Assets/LumaBay/Runtime/Presentation/UiBurstParticle.cs",
    "Assets/LumaBay/Runtime/Presentation/PieceDropMotion.cs",
    "Assets/LumaBay/Runtime/Services/SaveService.cs",
    "Assets/LumaBay/Editor/LumaBayProjectBootstrap.cs",
    "Assets/LumaBay/Editor/LumaBayBuildScript.cs",
    "Assets/LumaBay/Editor/LumaBayBrandingGenerator.cs",
]


def fail(message: str) -> None:
    print(f"ERROR: {message}", file=sys.stderr)
    raise SystemExit(1)


def check_required() -> None:
    for relative in REQUIRED:
        if not (ROOT / relative).is_file():
            fail(f"missing required file: {relative}")


def check_manifest() -> None:
    manifest = json.loads((ROOT / "Packages/manifest.json").read_text(encoding="utf-8"))
    dependencies = manifest.get("dependencies", {})
    if "com.unity.ugui" not in dependencies:
        fail("uGUI package is not declared")
    if "com.unity.test-framework" not in dependencies:
        fail("Unity Test Framework package is not declared")


def check_version() -> None:
    text = (ROOT / "ProjectSettings/ProjectVersion.txt").read_text(encoding="utf-8")
    if "6000.3.18f1" not in text:
        fail("unexpected Unity version")

    runtime = (ROOT / "Assets/LumaBay/Runtime/LumaBayGame.cs").read_text(encoding="utf-8")
    bootstrap = (ROOT / "Assets/LumaBay/Editor/LumaBayProjectBootstrap.cs").read_text(encoding="utf-8")
    build = (ROOT / "Assets/LumaBay/Editor/LumaBayBuildScript.cs").read_text(encoding="utf-8")
    for name, source in (("runtime", runtime), ("bootstrap", bootstrap), ("build", build)):
        if "0.1.1-alpha" not in source:
            fail(f"0.1.1-alpha version missing from {name}")


def check_mobile_configuration() -> None:
    bootstrap = (ROOT / "Assets/LumaBay/Editor/LumaBayProjectBootstrap.cs").read_text(encoding="utf-8")
    build = (ROOT / "Assets/LumaBay/Editor/LumaBayBuildScript.cs").read_text(encoding="utf-8")
    required_bootstrap_tokens = [
        "com.norvexa.lumabay",
        "AndroidArchitecture.ARM64",
        "ScriptingImplementation.IL2CPP",
        "ManagedStrippingLevel.Medium",
        "UIOrientation.Portrait",
    ]
    for token in required_bootstrap_tokens:
        if token not in bootstrap:
            fail(f"Android configuration token missing: {token}")
    if "BuildOptions.Development" in build or "development = true" in build:
        fail("0.1.1 control build must not be a development build")


def check_gameplay_features() -> None:
    game_types = (ROOT / "Assets/LumaBay/Runtime/Model/GameTypes.cs").read_text(encoding="utf-8")
    gameplay = (ROOT / "Assets/LumaBay/Runtime/LumaBayGame.Gameplay.cs").read_text(encoding="utf-8")
    board = (ROOT / "Assets/LumaBay/Runtime/Model/Match3Board.cs").read_text(encoding="utf-8")
    localization = (ROOT / "Assets/LumaBay/Runtime/Services/Localization.cs").read_text(encoding="utf-8")
    goals = (ROOT / "Assets/LumaBay/Runtime/LumaBayGame.Goals.cs").read_text(encoding="utf-8")
    obstacles = (ROOT / "Assets/LumaBay/Runtime/LumaBayGame.Obstacles.cs").read_text(encoding="utf-8")

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
    check_gameplay_features()
    check_csharp_balance()
    print("Luma Bay 0.1.1 static validation passed.")


if __name__ == "__main__":
    main()
