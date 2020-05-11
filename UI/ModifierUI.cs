using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
namespace NalulunaModifier.UI
{
    public class ModifierUI : NotifiableSingleton<ModifierUI>
    {
        [UIValue("parabola")]
        public bool parabola
        {
            get => Config.parabola;
            set
            {
                Config.parabola = value;
                Config.Write();
            }
        }

        [UIValue("duration")]
        public float duration
        {
            get { return 1.8f; }
            set
            {
            }
        }

        [UIAction("setParabola")]
        void setParabola(bool value)
        {
            parabola = value;
        }

        [UIValue("noBlue")]
        public bool noBlue
        {
            get => Config.noBlue;
            set
            {
                Config.noBlue = value;
                Config.Write();
            }
        }

        [UIAction("setNoBlue")]
        void setNoBlue(bool value)
        {
            noBlue = value;
        }

        [UIValue("noRed")]
        public bool noRed
        {
            get => Config.noRed;
            set
            {
                Config.noRed = value;
                Config.Write();
            }
        }

        [UIAction("setNoRed")]
        void setNoRed(bool value)
        {
            noRed = value;
        }

        [UIValue("redToBlue")]
        public bool redToBlue
        {
            get => Config.redToBlue;
            set
            {
                Config.redToBlue = value;
                Config.Write();
            }
        }

        [UIAction("setRedToBlue")]
        void setRedToBlue(bool value)
        {
            redToBlue = value;
        }

        [UIValue("blueToRed")]
        public bool blueToRed
        {
            get => Config.blueToRed;
            set
            {
                Config.blueToRed = value;
                Config.Write();
            }
        }

        [UIAction("setBlueToRed")]
        void setBlueToRed(bool value)
        {
            blueToRed = value;
        }

        [UIAction("modLinkClick")]
        void modLinkClick()
        {
            System.Diagnostics.Process.Start("https://youtu.be/hWmCNc5rLEI");
        }
    }
}
