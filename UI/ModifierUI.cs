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

        [UIAction("twitterLinkClick")]
        void twitterLinkClick()
        {
            System.Diagnostics.Process.Start("https://twitter.com/nalulululuna");
        }

        [UIAction("badmintonLinkClick")]
        void badmintonLinkClick()
        {
            System.Diagnostics.Process.Start("https://youtu.be/hWmCNc5rLEI");
        }

        [UIAction("boxingLinkClick")]
        void boxingLinkClick()
        {
            System.Diagnostics.Process.Start("https://youtu.be/JCZbdFYst5E");
        }

        [UIAction("jokeLinkClick")]
        void jokeLinkClick()
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/playlist?list=PLCy2c6iKFgOGhgJnusH8AM0fPcAEpEAJL");
        }

        [UIValue("boxing")]
        public bool boxing
        {
            get => Config.boxing;
            set
            {
                Config.boxing = value;
                Config.Write();
            }
        }

        [UIAction("setBoxing")]
        void setBoxing(bool value)
        {
            boxing = value;
        }

        [UIValue("centering")]
        public bool centering
        {
            get => Config.centering;
            set
            {
                Config.centering = value;
                Config.Write();
            }
        }

        [UIAction("setCentering")]
        void setCentering(bool value)
        {
            centering = value;
        }


        [UIValue("hideSabers")]
        public bool hideSabers
        {
            get => Config.hideSabers;
            set
            {
                Config.hideSabers = value;
                Config.Write();
            }
        }

        [UIAction("setHideSabers")]
        void setHideSabers(bool value)
        {
            hideSabers = value;
        }

        [UIValue("headbang")]
        public bool headbang
        {
            get => Config.headbang;
            set
            {
                Config.headbang = value;
                Config.Write();
            }
        }

        [UIAction("setHeadbang")]
        void setHeadbang(bool value)
        {
            headbang = value;
        }

        [UIValue("superhot")]
        public bool superhot
        {
            get => Config.superhot;
            set
            {
                Config.superhot = value;
                Config.Write();
            }
        }

        [UIAction("setSuperhot")]
        void setSuperhot(bool value)
        {
            superhot = value;
        }
    }
}
