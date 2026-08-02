# Changelog

## 0.1.1-alpha — premium visual update

### Added

- Premium blue-and-gold nautical visual system with pearl ornamentation.
- Illustrated sunset coast and lighthouse background generated at runtime.
- Redesigned main menu, map, level selection, settings, gameplay HUD and result windows.
- Glossy shell, starfish, lantern, compass, crystal and flower pieces without letter labels.
- Five usable boosters: Lightning Bolt, Anchor Bomb, Shuffle, Extra Moves and Magic Harpoon.
- Crate, ice and net obstacles introduced progressively across the 30 levels.
- Guided first-level swipe tutorial and booster tutorial with save migration.
- UI entrance motion, button press feedback, piece drop/bounce animation, glows and ambient sparkles.
- Procedural coastal ambience and upgraded interface, match, error and victory sounds.
- Generated lighthouse Android application icon.
- Save schema version 2 while preserving existing 0.1.0 progress.
- Obstacle EditMode tests and expanded static repository validation.

### Changed

- Rebalanced all 30 levels for a smoother difficulty curve.
- Fixed level-list sizing for portrait Android screens.
- Reworked victory presentation with three stars and a restoration reward.
- Optimized the control APK as a non-development ARM64 IL2CPP build.
- Enabled medium managed-code stripping to reduce package size.

## 0.1.0-alpha — tested vertical slice

### Added

- Complete runtime-generated portrait UI.
- Match-3 board with valid-board generation, swaps, matches, cascades and reshuffling.
- Row, column, bomb and rainbow special pieces.
- Thirty generated levels with collection and fog goals.
- Local progression, stars, coins and lighthouse restoration.
- Russian and English localization.
- Procedural piece textures and maritime interface artwork.
- Sound effects synthesized at runtime.
- Safe-area handling and color-independent piece shapes.
- Unity editor bootstrap and Android build command.
