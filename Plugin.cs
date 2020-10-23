using BeatSaberMarkupLanguage.GameplaySetup;
using HarmonyLib;
using IPA;
using System;
using System.Reflection;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace NalulunaModifier
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public const string HarmonyId = "com.github.nalulululuna.NalulunaModifier";
        internal static Harmony harmony = new Harmony(HarmonyId);

        internal static Plugin instance { get; private set; }
        internal static string Name => "NalulunaModifier";
        internal static string TabName => "Naluluna";
        internal static NalulunaModifierController PluginController { get { return NalulunaModifierController.instance; } }

        [Init]
        public Plugin(IPALogger logger)
        {
            instance = this;
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Config.Read();
            GameplaySetup.instance.AddTab(TabName, $"{Name}.UI.ModifierUI.bsml", ModifierUI.instance);
            new GameObject("NalulunaModifierController").AddComponent<NalulunaModifierController>();
            ApplyHarmonyPatches();
        }

        public static void ApplyHarmonyPatches()
        {
            try
            {
                Logger.log.Debug("Applying Harmony patches.");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Logger.log.Critical("Error applying Harmony patches: " + ex.Message);
                Logger.log.Debug(ex);
            }
        }

        public static void RemoveHarmonyPatches()
        {
            try
            {
                harmony.UnpatchAll(HarmonyId);
            }
            catch (Exception ex)
            {
                Logger.log.Critical("Error removing Harmony patches: " + ex.Message);
                Logger.log.Debug(ex);
            }
        }
    }
}
