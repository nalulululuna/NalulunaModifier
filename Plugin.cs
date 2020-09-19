using BeatSaberMarkupLanguage.GameplaySetup;
using BS_Utils.Gameplay;
using BS_Utils.Utilities;
using HarmonyLib;
using IPA;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace NalulunaModifier
{
    //[Plugin(RuntimeOptions.DynamicInit)]
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public const string HarmonyId = "com.twitter.nalulululuna.NalulunaModifier";
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
            OnEnable();
        }

        //[OnEnable]
        public void OnEnable()
        {
            Config.Read();
            GameplaySetup.instance.AddTab(TabName, $"{Name}.UI.ModifierUI.bsml", UI.ModifierUI.instance);
            new GameObject("NalulunaModifierController").AddComponent<NalulunaModifierController>();
            ApplyHarmonyPatches();
        }

        /*
        [OnDisable]
        public void OnDisable()
        {
            if (PluginController != null)
                GameObject.Destroy(PluginController);
            RemoveHarmonyPatches();
            GameplaySetup.instance.RemoveTab(TabName);
        }
        */

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
