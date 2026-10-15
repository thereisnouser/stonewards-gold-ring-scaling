# Stonewards Gold Ring Scaling

A small **BepInEx 5 + Harmony** mod for **Stonewards 0.2.3** that restores the gold ring's pre-0.2.3 multiplicative scaling, with a **200× cap** on the final gold-income multiplier.

> **Experimental:** Developed against the game's 0.2.3 `Assembly-CSharp.dll`. The 0.2.2 assembly has not been compared byte-for-byte, and the mod has **not yet been validated in a running game**. Make a backup of your save data before testing.

## What it changes

- Changes `AddMetaCurrencyMultiplierEffect` from `StatModType.LIMITED_TO_15` to `StatModType.PERCENT_MULT` when the effect is applied or previewed.
- Caps `ResourceManager.MetaCurrencyBonusMultiplier` at **200×**, including upgrade previews.
- Leaves other stats, rings, currency balances, and save files untouched by the mod itself.
- Works independently of [Stonewards Party HUD](https://github.com/thereisnouser/stonewards-party-hud).

The original 0.2.2 behavior has not been independently reproduced in-game. This mod restores the known modifier type and cap; it may not reproduce every historical detail.

## Requirements

- Stonewards **0.2.3** (later versions are not tested)
- [BepInEx 5](https://github.com/BepInEx/BepInEx)
- .NET SDK with support for `netstandard2.1` (to build from source)
- Windows PowerShell (for the installation scripts)

The project references game assemblies and BepInEx from your **local** installation; copyrighted game DLLs and mod-loader binaries are not included.

## Build and install

1. Close Stonewards.
2. Back up the entire directory:

   ```text
   %USERPROFILE%\AppData\LocalLow\Banana Tiger Studio\Stonewards\Saves
   ```

3. For initial testing, disable Steam Cloud for Stonewards temporarily to avoid overwriting your backup.
4. Open PowerShell in this repository and run (adjust the game path):

   ```powershell
   powershell -ExecutionPolicy Bypass -File .\build-and-install.ps1 -GameDir "F:\Steam\steamapps\common\Stonewards"
   ```

If you use a mod manager profile containing BepInEx, specify its path:

```powershell
powershell -ExecutionPolicy Bypass -File .\build-and-install.ps1 -GameDir "F:\Steam\steamapps\common\Stonewards" -ProfileDir "PATH_TO_PROFILE"
```

The script builds the DLL and installs only `StonewardsGoldRingScaling.dll` into `BepInEx\plugins\StonewardsGoldRingScaling`. It does not replace game DLLs or other mods.

## Verify

1. Launch the **modded** game and check `BepInEx\LogOutput.log` (or the mod-manager log).
2. Look for `Gold Ring Scaling loaded`.
3. After applying the gold ring, look for `Gold Ring stat type restored`.
4. Perform a small test run, verify the earned currency, exit, then relaunch and recheck your balance.
5. Test multiplayer separately before relying on it. Because the multiplier is evaluated client-side, each participating player may need the mod for consistent results; this has **not** been verified.

If the log contains `Unexpected Gold Ring stat type`, the mod will leave that effect unchanged. Stop and investigate before proceeding.

## Uninstall

Close the game and run:

```powershell
powershell -ExecutionPolicy Bypass -File .\uninstall.ps1 -GameDir "F:\Steam\steamapps\common\Stonewards"
```

Use `-ProfileDir` if you installed into a mod-manager profile. Restart the game completely after uninstalling.

## Leaderboards and save safety

Stonewards 0.2.3 introduced mod-loader detection which disables leaderboard score submissions when a mod loader is detected. **This mod does not alter, suppress, or bypass mod detection or leaderboard checks.**

Although the mod does not directly edit save files, normal gameplay with increased gold earnings can change saved balances and progression statistics. The developers may introduce additional checks in future updates. There is no guarantee against save incompatibility, account-related sanctions, or changes to game behavior.

## Project status

**0.1.0 — experimental source release.** Build and in-game testing are the next milestones. No prebuilt DLL or official support is provided.
