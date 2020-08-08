namespace NalulunaModifier
{
    public static class Config
    {
        public static BS_Utils.Utilities.Config config = new BS_Utils.Utilities.Config(Plugin.Name);
        
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

        public static bool headbang = false;
        public static bool superhot = false;
        public static bool vacuum = false;

        public static bool feet = false;
        public static bool flatNotes = false;
        public static bool noDirection = false;
        public static bool ignoreBadColor = false;
        public static bool feetAvatar = false;
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

            feet = config.GetBool(Plugin.Name, "feet", false, true);
            noDirection = config.GetBool(Plugin.Name, "noDirection", false, true);
            ignoreBadColor = config.GetBool(Plugin.Name, "ignoreBadColor", false, true);
            flatNotes = config.GetBool(Plugin.Name, "flatNotes", false, true);
            feetAvatar = config.GetBool(Plugin.Name, "feetAvatar", false, true);
            ReadFeetPosRot();

            headbang = config.GetBool(Plugin.Name, "headbang", false, true);
            superhot = config.GetBool(Plugin.Name, "superhot", false, true);
            vacuum = config.GetBool(Plugin.Name, "vacuum", false, true);
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
        }

        public static void Write()
        {
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

            config.SetBool(Plugin.Name, "headbang", headbang);
            config.SetBool(Plugin.Name, "superhot", superhot);
            config.SetBool(Plugin.Name, "vacuum", vacuum);
        }
    }
}
