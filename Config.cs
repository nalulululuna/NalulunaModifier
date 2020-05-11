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

        public static void Read()
        {
            parabola = config.GetBool(Plugin.Name, "parabola", false, true);
            parabolaOffsetY = config.GetFloat(Plugin.Name, "parabolaOffsetY", 1.8f);
            noBlue = config.GetBool(Plugin.Name, "noBlue", false, true);
            noRed = config.GetBool(Plugin.Name, "noRed", false, true);
            redToBlue = config.GetBool(Plugin.Name, "redToBlue", false, true);
            blueToRed = config.GetBool(Plugin.Name, "blueToRed", false, true);
        }

        public static void Write()
        {
            config.SetBool(Plugin.Name, "parabola", parabola);
            config.SetFloat(Plugin.Name, "parabolaOffsetY", parabolaOffsetY);
            config.SetBool(Plugin.Name, "noBlue", noBlue);
            config.SetBool(Plugin.Name, "noRed", noRed);
            config.SetBool(Plugin.Name, "redToBlue", redToBlue);
            config.SetBool(Plugin.Name, "blueToRed", blueToRed);
        }
    }
}
