# Changelog

## 0.1.2-alpha — full art, audio and animation rebuild

### Added

- Reproducible premium PNG art-pack generator for UI, background, pieces, boosters and lighthouse scenes.
- Thirty-two illustrated lighthouse states with damage, scaffolding, repairs, light, pier, garden and observatory.
- Forty-eight authored lighthouse tasks across six chapters and sixty campaign levels.
- Original casual-game audio bank with click, swap, invalid move, match, cascade, booster, reward, win, lose and restoration cues.
- Original coastal ambience and tonal background music with independent music setting.
- Animated valid swaps and invalid-swap return.
- Idle move detection and non-destructive animated hints.
- Animated restored-lighthouse beam and subtle illustration motion.
- Rebuilt three-column level selector for all sixty levels.
- Save schema version 3 with backup recovery and migration from earlier alpha versions.
- Campaign and lighthouse-progression EditMode tests.

### Changed

- Rebuilt main menu, map, settings, gameplay HUD, modal windows and progression presentation.
- Replaced letter-like and runtime-only piece art with dedicated generated sprites.
- Removed heavy gold decoration from individual board cells; gold is now reserved for hierarchy and rewards.
- Expanded the board campaign from 30 to 60 progressively balanced levels.
- Changed lighthouse progression from six broad steps to 48 smaller rewards and 32 visible states.
- Moved the first full lighthouse launch into the early campaign, followed by upgrades and surrounding-area development.
- Replaced runtime noise synthesis with imported generated WAV assets.
- Kept generated binary art/audio out of Git while retaining deterministic source generators.

## 0.1.1-alpha — premium visual experiment

### Added

- First blue-and-gold nautical visual system.
- Five usable boosters and crate, ice and net obstacles.
- Guided swipe and booster tutorials.
- UI entrance motion, button feedback, piece drop animation and ambient sparkles.
- Save schema version 2.

### Changed

- Rebalanced 30 levels.
- Fixed portrait level-list sizing.
- Switched the control APK to non-development ARM64 IL2CPP.

## 0.1.0-alpha — tested vertical slice

### Added

- Runtime-generated portrait UI.
- Match-3 board with swaps, matches, cascades and reshuffling.
- Row, column, bomb and rainbow special pieces.
- Thirty levels with collection and fog goals.
- Local progression, stars, coins and lighthouse restoration.
- Russian and English localization.
- Unity editor bootstrap and Android build command.
