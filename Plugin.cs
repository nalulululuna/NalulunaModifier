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

        private static PlayerController _playerController;

        [Init]
        public Plugin(IPALogger logger)
        {
            instance = this;
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");
        }

        private void HandleGameSceneLoaded()
        {
            Config.Read();
            if (Config.parabola || Config.noBlue || Config.noRed || Config.redToBlue || Config.blueToRed)
            {
                ScoreSubmission.ProlongedDisableSubmission(Name);
            }
            else
            {
                ScoreSubmission.RemoveProlongedDisable(Name);
            }

            _playerController = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
            if (_playerController != null)
            {
                _playerController.leftSaber.gameObject.SetActive(Config.blueToRed || !(Config.noRed || Config.redToBlue));
                _playerController.rightSaber.gameObject.SetActive(Config.redToBlue || !(Config.noBlue || Config.blueToRed));
            }
        }

        [OnStart]
        public void OnApplicationStart()
        {
            OnEnable();
        }

        //[OnEnable]
        public void OnEnable()
        {
            ApplyHarmonyPatches();

            Config.Read();
            BSEvents.gameSceneLoaded += HandleGameSceneLoaded;
            GameplaySetup.instance.AddTab(TabName, $"{Name}.UI.BSML.ModifierUI.bsml", UI.ModifierUI.instance);
        }

        /*
        [OnDisable]
        public void OnDisable()
        {
            RemoveHarmonyPatches();

            BSEvents.gameSceneLoaded -= HandleGameSceneLoaded;
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
