using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace StonewardsGoldRingScaling
{
    [BepInPlugin(PluginId, PluginName, PluginVersion)]
    public sealed class GoldRingScalingPlugin : BaseUnityPlugin
    {
        public const string PluginId = "stonewards.goldringscaling";
        public const string PluginName = "Stonewards Gold Ring Scaling";
        public const string PluginVersion = "0.1.0";
        private Harmony? _harmony;

        internal static ManualLogSource? Log { get; private set; }

        private void Awake()
        {
            Log = Logger;
            _harmony = new Harmony(PluginId);
            _harmony.PatchAll(typeof(GoldRingScalingPlugin).Assembly);
            Logger.LogInfo("Gold Ring Scaling loaded. Leaderboard detection is NOT modified.");
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }

    // Stonewards 0.2.3 uses LIMITED_TO_15 for the Gold Ring's income effect.
    // Stonewards 0.2.2 used compounding modifiers (PERCENT_MULT).
    // Only touch the specific Gold Ring income effect; all other stats stay vanilla.
    internal static class LegacyGoldRingScaling
    {
        private static bool _logged;

        internal static void Restore(AddMetaCurrencyMultiplierEffect effect)
        {
            if (effect == null)
                return;

            if (effect.statType == StatModType.PERCENT_MULT)
                return;

            if (effect.statType != StatModType.LIMITED_TO_15)
            {
                if (!_logged)
                {
                    GoldRingScalingPlugin.Log?.LogWarning(
                        "Unexpected Gold Ring stat type: " + effect.statType +
                        ". The mod left it unchanged for safety.");
                    _logged = true;
                }
                return;
            }

            effect.statType = StatModType.PERCENT_MULT;
            if (!_logged)
            {
                GoldRingScalingPlugin.Log?.LogInfo(
                    "Gold Ring stat type restored: LIMITED_TO_15 -> PERCENT_MULT.");
                _logged = true;
            }
        }
    }

    [HarmonyPatch(typeof(AddMetaCurrencyMultiplierEffect), nameof(AddMetaCurrencyMultiplierEffect.Apply))]
    internal static class GoldRingApplyPatch
    {
        [HarmonyPrefix]
        private static void Prefix(AddMetaCurrencyMultiplierEffect __instance)
        {
            LegacyGoldRingScaling.Restore(__instance);
        }
    }

    [HarmonyPatch(typeof(AddMetaCurrencyMultiplierEffect), "GetNextValue", new[] { typeof(float) })]
    internal static class GoldRingPreviewPatch
    {
        [HarmonyPrefix]
        private static void Prefix(AddMetaCurrencyMultiplierEffect __instance)
        {
            LegacyGoldRingScaling.Restore(__instance);
        }
    }

    // Hotfix 0.2.2 stated a maximum gold multiplier of x200.
    // Constrain ONLY the income stat, not other CharacterStats or saved currency.
    internal static class LegacyGoldCap
    {
        internal const float MaxMultiplier = 200f;

        internal static void Clamp(CharacterStat stat, ref float value)
        {
            ResourceManager manager = ResourceManager.Instance;
            if (manager == null)
                return;

            if (ReferenceEquals(stat, manager.MetaCurrencyBonusMultiplier) && value > MaxMultiplier)
                value = MaxMultiplier;
        }
    }

    [HarmonyPatch(typeof(CharacterStat), "get_Value")]
    internal static class GoldMultiplierCapPatch
    {
        [HarmonyPostfix]
        private static void Postfix(CharacterStat __instance, ref float __result)
        {
            LegacyGoldCap.Clamp(__instance, ref __result);
        }
    }

    [HarmonyPatch(typeof(CharacterStat), nameof(CharacterStat.GetValueWithModifierPreview))]
    internal static class GoldMultiplierPreviewCapPatch
    {
        [HarmonyPostfix]
        private static void Postfix(CharacterStat __instance, ref float __result)
        {
            LegacyGoldCap.Clamp(__instance, ref __result);
        }
    }
}
