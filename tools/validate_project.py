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
    "Assets/LumaBay/Runtime/Model/Match3Board.cs",
    "Assets/LumaBay/Editor/LumaBayProjectBootstrap.cs",
    "Assets/LumaBay/Editor/LumaBayBuildScript.cs",
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


def check_version() -> None:
    text = (ROOT / "ProjectSettings/ProjectVersion.txt").read_text(encoding="utf-8")
    if "6000.3.18f1" not in text:
        fail("unexpected Unity version")


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
    check_csharp_balance()
    print("Luma Bay static validation passed.")


if __name__ == "__main__":
    main()
