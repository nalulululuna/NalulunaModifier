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

        [UIAction("setHideSaberEffects")]
        void setHideSaberEffects(bool value)
        {
            hideSaberEffects = value;
        }

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

        [UIAction("setFeet")]
        void setFoot(bool value)
        {
            feet = value;
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

        [UIAction("setNoDirection")]
        void setNoDirection(bool value)
        {
            noDirection = value;
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

        [UIAction("setIgnoreBadColor")]
        void setIgnoreBadColor(bool value)
        {
            ignoreBadColor = value;
        }

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

        [UIAction("setFlatNotes")]
        void setFlatNotes(bool value)
        {
            flatNotes = value;
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

        [UIAction("setFeetAvatar")]
        void setFeetAvatar(bool value)
        {
            feetAvatar = value;
        }

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

        [UIAction("setFourSabers")]
        void setFourSabers(bool value)
        {
            fourSabers = value;
        }

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

        [UIAction("setReverseGrip")]
        void setReverseGrip(bool value)
        {
            reverseGrip = value;
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

        [UIAction("setTopNotesToFeet")]
        void setTopNotesToFeet(bool value)
        {
            topNotesToFeet = value;
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

        [UIAction("setMiddleNotesToFeet")]
        void setMiddleNotesToFeet(bool value)
        {
            middleNotesToFeet = value;
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

        [UIAction("setBottomNotesToFeet")]
        void setBottomNotesToFeet(bool value)
        {
            bottomNotesToFeet = value;
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

        [UIAction("setVacuum")]
        void setVacuum(bool value)
        {
            vacuum = value;
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

        [UIAction("setNinjaMaster")]
        void setNinjaMaster(bool value)
        {
            ninjaMaster = value;
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

        [UIAction("setBeatWalker")]
        void setBeatWalk(bool value)
        {
            beatWalker = value;
        }
        */
    }
}
