# Luma Bay 0.1.1 Alpha scope

## Player-facing completion criteria

- The game starts on Android and in the Unity Editor without manual scene construction.
- Existing 0.1.0 progress migrates without being erased.
- A new player receives a one-time swipe tutorial and later a booster tutorial.
- Swapping adjacent pieces works by tap-tap and swipe.
- Invalid swaps do not consume a move.
- Matches, cascades, fog clearing, obstacles and special pieces resolve without deadlocks.
- Crates, ice and nets appear progressively rather than all at once.
- Thirty levels are unlockable in sequence with a smoother difficulty curve.
- Winning awards stars and coins; losing allows retry.
- Five nautical boosters can be purchased using soft currency.
- Stars can be spent on six lighthouse restoration steps.
- Russian and English can be switched in Settings.
- The UI respects Android safe areas and portrait aspect ratios.
- The level list displays all 30 level cards and scrolls correctly.
- The visual language is consistent across menus, map, gameplay and result windows.
- The Android launcher uses the generated Luma Bay lighthouse icon.
- The control APK does not display Unity's `Development Build` watermark.

## Visual target

- Deep blue maritime glass panels.
- Gold borders, pearl details and warm lighthouse lighting.
- Glossy shell, starfish, lantern, compass, crystal and flower pieces.
- Sunset coast and lighthouse background.
- Smooth entrance, press, piece-drop, glow and match-sparkle effects.
- Readable interface at 720×1280 and taller portrait aspect ratios.

## Technical target

- Unity 6000.3.18f1.
- ARM64 Android build with IL2CPP.
- Medium managed-code stripping.
- Version `0.1.1-alpha`, Android `versionCode` 2.
- Package ID `com.norvexa.lumabay`.
- Local save schema version 2.
- Static repository validation and Unity EditMode tests.

## Explicitly deferred after 0.1.1 Alpha

- Advertising and in-app purchases.
- Cloud Save and account linking.
- Remote Config and server-hosted levels.
- Push notifications.
- Timed events, clans and leaderboards.
- Full narrative cut-scenes and character animation.
- Voice-over.
- Google Play release signing and production Android App Bundle.
