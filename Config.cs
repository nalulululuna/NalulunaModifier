using IPA.Utilities;
using System.Collections;
using System.IO;
using UnityEngine;

namespace NalulunaModifier
{
    public static class Config
    {
        private static BS_Utils.Utilities.Config _config = new BS_Utils.Utilities.Config(Plugin.Name);

        public static bool parabola = false;
        public static float parabolaOffsetY = 1.8f;
        public static bool noBlue = false;
        public static bool noRed = false;
        public static bool redToBlue = false;
        public static bool blueToRed = false;

        public static bool boxing = false;
        public static bool hideSabers = false;
        public static bool hideSaberEffects = false;
        public static bool centering = false;
        public static float centeringOffsetY = 0.3f;
        public static float centeringNotesScale = 0.75f;

        public static bool headbang = false;
        public static bool superhot = false;
        public static bool vacuum = false;
        public static bool ninjaMaster = false;
        public static float ninjaMasterSaberSeparation = 0.18f;
        public static bool ninjaMasterHideHand = false;
        public static bool ninjaMasterHideHandL = false;
        public static bool ninjaMasterHideHandR = false;
        public static bool ninjaMasterHideFoot = false;
        public static bool ninjaMasterHideWaist = false;
        public static bool ninjaMasterHideMouth = false;
        public static bool ninjaMasterHideHead = false;
        public static bool beatWalker = false;

        public static bool feetModifiers = false;
        public static bool feet = false;
        public static float feetNotesY = 0.1f;
        public static bool flatNotes = false;
        public static bool noDirection = false;
        public static bool ignoreBadColor = false;
        public static bool feetAvatar = false;
        
        public static bool feetTracker
        {
            get { return _feetTracker && feet; }
            set { _feetTracker = value; }
        }
        private static bool _feetTracker;

        public static bool ninjaModifiers = false;
        public static bool fourSabers = false;
        public static bool reverseGrip = false;
        public static bool topNotesToFeet = false;
        public static bool middleNotesToFeet = false;
        public static bool bottomNotesToFeet = false;

        public static float vmcAvatarFootPosX = 0;
        public static float vmcAvatarFootPosY = 0;
        public static float vmcAvatarFootPosZ = 0;
        public static float vmcAvatarFootRotX = 0;
        public static float vmcAvatarFootRotY = 0;
        public static float vmcAvatarFootRotZ = 0;
        public static float customAvatarFootPosX = 0;
        public static float customAvatarFootPosY = 0;
        public static float customAvatarFootPosZ = 0;
        public static float customAvatarFootRotX = -135f;
        public static float customAvatarFootRotY = 0;
        public static float customAvatarFootRotZ = 0;
        public static float trackerFootPosX = 0;
        public static float trackerFootPosY = 0;
        public static float trackerFootPosZ = 0;
        public static float trackerFootRotX = 0;
        public static float trackerFootRotY = 0;
        public static float trackerFootRotZ = 0;

        public static FileSystemWatcher watcher = new FileSystemWatcher(UnityGame.UserDataPath)
        {
            NotifyFilter = NotifyFilters.LastWrite,
            Filter = Plugin.Name + ".ini",
            EnableRaisingEvents = true
        };

        private static bool _init;
        private static bool _ignoreConfigChanged;

        private static void Init()
        {
            watcher.Changed += OnConfigChanged;
            _init = true;
        }

        private static void OnConfigChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            //Logger.log.Debug("OnConfigChanged");
            if (!_ignoreConfigChanged)
            {
                Config.Read();
                ModifierUI.instance.updateUI();
            }
        }

        public static void Read()
        {
            if (!_init)
            {
                Init();
            }

            parabola = _config.GetBool(Plugin.Name, "parabola", false, true);
            parabolaOffsetY = _config.GetFloat(Plugin.Name, "parabolaOffsetY", 1.8f);
            noBlue = _config.GetBool(Plugin.Name, "noBlue", false, true);
            noRed = _config.GetBool(Plugin.Name, "noRed", false, true);
            redToBlue = _config.GetBool(Plugin.Name, "redToBlue", false, true);
            blueToRed = _config.GetBool(Plugin.Name, "blueToRed", false, true);

            boxing = _config.GetBool(Plugin.Name, "boxing", false, true);
            hideSabers = _config.GetBool(Plugin.Name, "hideSabers", false, true);
            hideSaberEffects = _config.GetBool(Plugin.Name, "hideSaberEffects", false, true);
            centering = _config.GetBool(Plugin.Name, "centering", false, true);
            centeringOffsetY = _config.GetFloat(Plugin.Name, "centeringOffsetY", 0.3f, true);
            centeringNotesScale = _config.GetFloat(Plugin.Name, "centeringNotesScale", 0.75f, true);

            feetModifiers = _config.GetBool(Plugin.Name, "feetModifiers", false, true);
            feet = _config.GetBool(Plugin.Name, "feet", false, true);
            feetNotesY = _config.GetFloat(Plugin.Name, "feetNotesY", 0.1f, true);
            noDirection = _config.GetBool(Plugin.Name, "noDirection", false, true);
            ignoreBadColor = _config.GetBool(Plugin.Name, "ignoreBadColor", false, true);
            flatNotes = _config.GetBool(Plugin.Name, "flatNotes", false, true);
            feetAvatar = _config.GetBool(Plugin.Name, "feetAvatar", false, true);
            feetTracker = _config.GetBool(Plugin.Name, "feetTracker", false, true);
            ReadFeetPosRot();

            ninjaModifiers = _config.GetBool(Plugin.Name, "ninjaModifiers", false, true);
            fourSabers = _config.GetBool(Plugin.Name, "fourSabers", false, true);
            reverseGrip = _config.GetBool(Plugin.Name, "reverseGrip", false, true);
            topNotesToFeet = _config.GetBool(Plugin.Name, "topNotesToFeet", false, true);
            middleNotesToFeet = _config.GetBool(Plugin.Name, "middleNotesToFeet", false, true);
            bottomNotesToFeet = _config.GetBool(Plugin.Name, "bottomNotesToFeet", false, true);

            headbang = _config.GetBool(Plugin.Name, "headbang", false, true);
            superhot = _config.GetBool(Plugin.Name, "superhot", false, true);
            vacuum = _config.GetBool(Plugin.Name, "vacuum", false, true);
            ninjaMaster = _config.GetBool(Plugin.Name, "ninjaMaster", false, true);
            ninjaMasterSaberSeparation = _config.GetFloat(Plugin.Name, "ninjaMasterSaberSeparation", 0.18f, true);
            ninjaMasterHideHand = _config.GetBool(Plugin.Name, "ninjaMasterHideHand", false, true);
            ninjaMasterHideHandL = _config.GetBool(Plugin.Name, "ninjaMasterHideHandL", false, true);
            ninjaMasterHideHandR = _config.GetBool(Plugin.Name, "ninjaMasterHideHandR", false, true);
            ninjaMasterHideFoot = _config.GetBool(Plugin.Name, "ninjaMasterHideFoot", false, true);
            ninjaMasterHideWaist = _config.GetBool(Plugin.Name, "ninjaMasterHideWaist", false, true);
            ninjaMasterHideMouth = _config.GetBool(Plugin.Name, "ninjaMasterHideMouth", false, true);
            ninjaMasterHideHead = _config.GetBool(Plugin.Name, "ninjaMasterHideHead", false, true);
        }

        public static void ReadFeetPosRot()
        {
            vmcAvatarFootPosX = _config.GetFloat(Plugin.Name, "vmcAvatarFootPosX", 0, true);
            vmcAvatarFootPosY = _config.GetFloat(Plugin.Name, "vmcAvatarFootPosY", 0, true);
            vmcAvatarFootPosZ = _config.GetFloat(Plugin.Name, "vmcAvatarFootPosZ", 0, true);

            vmcAvatarFootRotX = _config.GetFloat(Plugin.Name, "vmcAvatarFootRotX", 0, true);
            vmcAvatarFootRotY = _config.GetFloat(Plugin.Name, "vmcAvatarFootRotY", 0, true);
            vmcAvatarFootRotZ = _config.GetFloat(Plugin.Name, "vmcAvatarFootRotZ", 0, true);

            customAvatarFootPosX = _config.GetFloat(Plugin.Name, "customAvatarFootPosX", 0, true);
            customAvatarFootPosY = _config.GetFloat(Plugin.Name, "customAvatarFootPosY", 0, true);
            customAvatarFootPosZ = _config.GetFloat(Plugin.Name, "customAvatarFootPosZ", 0, true);

            customAvatarFootRotX = _config.GetFloat(Plugin.Name, "customAvatarFootRotX", -135f, true);
            customAvatarFootRotY = _config.GetFloat(Plugin.Name, "customAvatarFootRotY", 0, true);
            customAvatarFootRotZ = _config.GetFloat(Plugin.Name, "customAvatarFootRotZ", 0, true);

            trackerFootPosX = _config.GetFloat(Plugin.Name, "trackerFootPosX", 0, true);
            trackerFootPosY = _config.GetFloat(Plugin.Name, "trackerFootPosY", 0, true);
            trackerFootPosZ = _config.GetFloat(Plugin.Name, "trackerFootPosZ", 0, true);

            trackerFootRotX = _config.GetFloat(Plugin.Name, "trackerFootRotX", 0, true);
            trackerFootRotY = _config.GetFloat(Plugin.Name, "trackerFootRotY", 0, true);
            trackerFootRotZ = _config.GetFloat(Plugin.Name, "trackerFootRotZ", 0, true);
        }

        public static void Write()
        {
            PersistentSingleton<SharedCoroutineStarter>.instance.StartCoroutine(DisableWatcherTemporaryCoroutine());

            _config.SetBool(Plugin.Name, "parabola", parabola);
            _config.SetFloat(Plugin.Name, "parabolaOffsetY", parabolaOffsetY);
            _config.SetBool(Plugin.Name, "boxing", boxing);
            _config.SetBool(Plugin.Name, "hideSabers", hideSabers);
            _config.SetBool(Plugin.Name, "hideSaberEffects", hideSaberEffects);

            _config.SetBool(Plugin.Name, "centering", centering);
            _config.SetBool(Plugin.Name, "noBlue", noBlue);
            _config.SetBool(Plugin.Name, "noRed", noRed);
            _config.SetBool(Plugin.Name, "redToBlue", redToBlue);
            _config.SetBool(Plugin.Name, "blueToRed", blueToRed);

            _config.SetBool(Plugin.Name, "feetModifiers", feetModifiers);
            _config.SetBool(Plugin.Name, "feet", feet);
            _config.SetBool(Plugin.Name, "noDirection", noDirection);
            _config.SetBool(Plugin.Name, "ignoreBadColor", ignoreBadColor);
            _config.SetBool(Plugin.Name, "flatNotes", flatNotes);
            _config.SetBool(Plugin.Name, "feetAvatar", feetAvatar);

            _config.SetBool(Plugin.Name, "ninjaModifiers", ninjaModifiers);
            _config.SetBool(Plugin.Name, "fourSabers", fourSabers);
            _config.SetBool(Plugin.Name, "reverseGrip", reverseGrip);
            _config.SetBool(Plugin.Name, "topNotesToFeet", topNotesToFeet);
            _config.SetBool(Plugin.Name, "middleNotesToFeet", middleNotesToFeet);
            _config.SetBool(Plugin.Name, "bottomNotesToFeet", bottomNotesToFeet);

            _config.SetBool(Plugin.Name, "headbang", headbang);
            _config.SetBool(Plugin.Name, "superhot", superhot);
            _config.SetBool(Plugin.Name, "vacuum", vacuum);
            _config.SetBool(Plugin.Name, "ninjaMaster", ninjaMaster);
        }

        private static IEnumerator DisableWatcherTemporaryCoroutine()
        {
            _ignoreConfigChanged = true;
            yield return new WaitForSecondsRealtime(1f);
            _ignoreConfigChanged = false;
        }
    }
}
