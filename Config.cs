using IPA.Utilities;
using System.IO;

namespace NalulunaModifier
{
    public static class Config
    {
        public static BS_Utils.Utilities.Config config = new BS_Utils.Utilities.Config(Plugin.Name);
        public static FileSystemWatcher watcher = new FileSystemWatcher(UnityGame.UserDataPath)
        {
            NotifyFilter = NotifyFilters.LastWrite,
            Filter = Plugin.Name + ".ini",
            EnableRaisingEvents = true
        };          

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

        public static void Read()
        {
            parabola = config.GetBool(Plugin.Name, "parabola", false, true);
            parabolaOffsetY = config.GetFloat(Plugin.Name, "parabolaOffsetY", 1.8f);
            noBlue = config.GetBool(Plugin.Name, "noBlue", false, true);
            noRed = config.GetBool(Plugin.Name, "noRed", false, true);
            redToBlue = config.GetBool(Plugin.Name, "redToBlue", false, true);
            blueToRed = config.GetBool(Plugin.Name, "blueToRed", false, true);

            boxing = config.GetBool(Plugin.Name, "boxing", false, true);
            hideSabers = config.GetBool(Plugin.Name, "hideSabers", false, true);
            hideSaberEffects = config.GetBool(Plugin.Name, "hideSaberEffects", false, true);
            centering = config.GetBool(Plugin.Name, "centering", false, true);
            centeringOffsetY = config.GetFloat(Plugin.Name, "centeringOffsetY", 0.3f, true);

            feet = config.GetBool(Plugin.Name, "feet", false, true);
            feetNotesY = config.GetFloat(Plugin.Name, "feetNotesY", 0.1f, true);
            noDirection = config.GetBool(Plugin.Name, "noDirection", false, true);
            ignoreBadColor = config.GetBool(Plugin.Name, "ignoreBadColor", false, true);
            flatNotes = config.GetBool(Plugin.Name, "flatNotes", false, true);
            feetAvatar = config.GetBool(Plugin.Name, "feetAvatar", false, true);
            feetTracker = config.GetBool(Plugin.Name, "feetTracker", false, true);
            ReadFeetPosRot();

            fourSabers = config.GetBool(Plugin.Name, "fourSabers", false, true);
            reverseGrip = config.GetBool(Plugin.Name, "reverseGrip", false, true);
            topNotesToFeet = config.GetBool(Plugin.Name, "topNotesToFeet", false, true);
            middleNotesToFeet = config.GetBool(Plugin.Name, "middleNotesToFeet", false, true);
            bottomNotesToFeet = config.GetBool(Plugin.Name, "bottomNotesToFeet", false, true);

            headbang = config.GetBool(Plugin.Name, "headbang", false, true);
            superhot = config.GetBool(Plugin.Name, "superhot", false, true);
            vacuum = config.GetBool(Plugin.Name, "vacuum", false, true);
            ninjaMaster = config.GetBool(Plugin.Name, "ninjaMaster", false, true);
            ninjaMasterSaberSeparation = config.GetFloat(Plugin.Name, "ninjaMasterSaberSeparation", 0.18f, true);
            ninjaMasterHideHand = config.GetBool(Plugin.Name, "ninjaMasterHideHand", false, true);
            ninjaMasterHideHandL = config.GetBool(Plugin.Name, "ninjaMasterHideHandL", false, true);
            ninjaMasterHideHandR = config.GetBool(Plugin.Name, "ninjaMasterHideHandR", false, true);
            ninjaMasterHideFoot = config.GetBool(Plugin.Name, "ninjaMasterHideFoot", false, true);
            ninjaMasterHideWaist = config.GetBool(Plugin.Name, "ninjaMasterHideWaist", false, true);
            ninjaMasterHideMouth = config.GetBool(Plugin.Name, "ninjaMasterHideMouth", false, true);
            ninjaMasterHideHead = config.GetBool(Plugin.Name, "ninjaMasterHideHead", false, true);
        }

        public static void ReadFeetPosRot()
        {
            vmcAvatarFootPosX = config.GetFloat(Plugin.Name, "vmcAvatarFootPosX", 0, true);
            vmcAvatarFootPosY = config.GetFloat(Plugin.Name, "vmcAvatarFootPosY", 0, true);
            vmcAvatarFootPosZ = config.GetFloat(Plugin.Name, "vmcAvatarFootPosZ", 0, true);

            vmcAvatarFootRotX = config.GetFloat(Plugin.Name, "vmcAvatarFootRotX", 0, true);
            vmcAvatarFootRotY = config.GetFloat(Plugin.Name, "vmcAvatarFootRotY", 0, true);
            vmcAvatarFootRotZ = config.GetFloat(Plugin.Name, "vmcAvatarFootRotZ", 0, true);

            customAvatarFootPosX = config.GetFloat(Plugin.Name, "customAvatarFootPosX", 0, true);
            customAvatarFootPosY = config.GetFloat(Plugin.Name, "customAvatarFootPosY", 0, true);
            customAvatarFootPosZ = config.GetFloat(Plugin.Name, "customAvatarFootPosZ", 0, true);

            customAvatarFootRotX = config.GetFloat(Plugin.Name, "customAvatarFootRotX", -135f, true);
            customAvatarFootRotY = config.GetFloat(Plugin.Name, "customAvatarFootRotY", 0, true);
            customAvatarFootRotZ = config.GetFloat(Plugin.Name, "customAvatarFootRotZ", 0, true);

            trackerFootPosX = config.GetFloat(Plugin.Name, "trackerFootPosX", 0, true);
            trackerFootPosY = config.GetFloat(Plugin.Name, "trackerFootPosY", 0, true);
            trackerFootPosZ = config.GetFloat(Plugin.Name, "trackerFootPosZ", 0, true);

            trackerFootRotX = config.GetFloat(Plugin.Name, "trackerFootRotX", 0, true);
            trackerFootRotY = config.GetFloat(Plugin.Name, "trackerFootRotY", 0, true);
            trackerFootRotZ = config.GetFloat(Plugin.Name, "trackerFootRotZ", 0, true);
        }

        public static void Write()
        {
            watcher.EnableRaisingEvents = false;

            config.SetBool(Plugin.Name, "parabola", parabola);
            config.SetFloat(Plugin.Name, "parabolaOffsetY", parabolaOffsetY);
            config.SetBool(Plugin.Name, "boxing", boxing);
            config.SetBool(Plugin.Name, "hideSabers", hideSabers);
            config.SetBool(Plugin.Name, "hideSaberEffects", hideSaberEffects);

            config.SetBool(Plugin.Name, "centering", centering);
            config.SetBool(Plugin.Name, "noBlue", noBlue);
            config.SetBool(Plugin.Name, "noRed", noRed);
            config.SetBool(Plugin.Name, "redToBlue", redToBlue);
            config.SetBool(Plugin.Name, "blueToRed", blueToRed);

            config.SetBool(Plugin.Name, "feet", feet);
            config.SetBool(Plugin.Name, "noDirection", noDirection);
            config.SetBool(Plugin.Name, "ignoreBadColor", ignoreBadColor);
            config.SetBool(Plugin.Name, "flatNotes", flatNotes);
            config.SetBool(Plugin.Name, "feetAvatar", feetAvatar);

            config.SetBool(Plugin.Name, "fourSabers", fourSabers);
            config.SetBool(Plugin.Name, "reverseGrip", reverseGrip);
            config.SetBool(Plugin.Name, "topNotesToFeet", topNotesToFeet);
            config.SetBool(Plugin.Name, "middleNotesToFeet", middleNotesToFeet);
            config.SetBool(Plugin.Name, "bottomNotesToFeet", bottomNotesToFeet);

            config.SetBool(Plugin.Name, "headbang", headbang);
            config.SetBool(Plugin.Name, "superhot", superhot);
            config.SetBool(Plugin.Name, "vacuum", vacuum);
            config.SetBool(Plugin.Name, "ninjaMaster", ninjaMaster);

            watcher.EnableRaisingEvents = true;
        }
    }
}
