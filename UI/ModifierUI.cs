using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;

namespace NalulunaModifier
{
    public class ModifierUI : NotifiableSingleton<ModifierUI>
    {
        [UIParams]
        BSMLParserParams parserParams;

        public void updateUI()
        {
            parserParams.EmitEvent("cancel");
        }

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

        [UIAction("footLinkClick")]
        void footLinkClick()
        {
            System.Diagnostics.Process.Start("https://youtu.be/QtLNweiiQPU");
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

        [UIValue("hideSabers")]
        public bool hideSabers
        {
            get => Config.hideSabers;
            set
            {
                Config.hideSabers = value;
                Config.Write();

                updateUI();
            }
        }

        [UIValue("hideSaberEffects")]
        public bool hideSaberEffects
        {
            get => Config.hideSaberEffects;
            set
            {
                Config.hideSaberEffects = value;
                Config.Write();
            }
        }

        [UIValue("feetModifiers")]
        public bool feetModifiers
        {
            get => Config.feetModifiers;
            set
            {
                Config.feetModifiers = value;
                Config.feet = value;
                Config.noDirection = value;
                Config.ignoreBadColor = value;
                Config.Write();
            }
        }

        /*
        [UIValue("feet")]
        public bool feet
        {
            get => Config.feet;
            set
            {
                Config.feet = value;
                Config.Write();
            }
        }

        [UIValue("noDirection")]
        public bool noDirection
        {
            get => Config.noDirection;
            set
            {
                Config.noDirection = value;
                Config.Write();
            }
        }

        [UIValue("ignoreBadColor")]
        public bool ignoreBadColor
        {
            get => Config.ignoreBadColor;
            set
            {
                Config.ignoreBadColor = value;
                Config.Write();
            }
        }
        */

        [UIValue("flatNotes")]
        public bool flatNotes
        {
            get => Config.flatNotes;
            set
            {
                Config.flatNotes = value;
                Config.Write();
            }
        }

        [UIValue("feetAvatar")]
        public bool feetAvatar
        {
            get => Config.feetAvatar;
            set
            {
                Config.feetAvatar = value;
                Config.Write();
            }
        }

        [UIValue("disableStatistics")]
        public bool disableStatistics
        {
            get => Config.disableStatistics;
            set
            {
                Config.disableStatistics = value;
                Config.Write();
            }
        }

        [UIValue("ninjaModifiers")]
        public bool ninjaModifiers
        {
            get => Config.ninjaModifiers;
            set
            {
                Config.ninjaModifiers = value;
                Config.fourSabers = value;
                Config.Write();
            }
        }

        /*
        [UIValue("fourSabers")]
        public bool fourSabers
        {
            get => Config.fourSabers;
            set
            {
                Config.fourSabers = value;
                Config.Write();
            }
        }
        */

        [UIValue("reverseGrip")]
        public bool reverseGrip
        {
            get => Config.reverseGrip;
            set
            {
                Config.reverseGrip = value;
                Config.Write();
            }
        }

        [UIValue("topNotesToFeet")]
        public bool topNotesToFeet
        {
            get => Config.topNotesToFeet;
            set
            {
                Config.topNotesToFeet = value;
                Config.Write();
            }
        }

        [UIValue("middleNotesToFeet")]
        public bool middleNotesToFeet
        {
            get => Config.middleNotesToFeet;
            set
            {
                Config.middleNotesToFeet = value;
                Config.Write();
            }
        }

        [UIValue("bottomNotesToFeet")]
        public bool bottomNotesToFeet
        {
            get => Config.bottomNotesToFeet;
            set
            {
                Config.bottomNotesToFeet = value;
                Config.Write();
            }
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

        [UIValue("vacuum")]
        public bool vacuum
        {
            get => Config.vacuum;
            set
            {
                Config.vacuum = value;
                Config.Write();
            }
        }

        [UIValue("ninjaMaster")]
        public bool ninjaMaster
        {
            get => Config.ninjaMaster;
            set
            {
                Config.ninjaMaster = value;
                Config.Write();
            }
        }

        /*
        [UIValue("beatWalker")]
        public bool beatWalker
        {
            get => Config.beatWalker;
            set
            {
                Config.beatWalker = value;
                Config.Write();
            }
        }
        */
    }
}
