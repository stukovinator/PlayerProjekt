using Plugin.MaterialDesignControls.Styles;
using Plugin.MaterialDesignControls;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlayerProjekt
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            SetMaterialStyles();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private void SetMaterialStyles()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var currentTheme = Application.Current.RequestedTheme;

                MaterialColor.Primary = currentTheme == OSAppTheme.Dark ? Color.FromHex("#303F9F") : Color.FromHex("#303F9F");
                MaterialColor.OnPrimary = currentTheme == OSAppTheme.Dark ? Color.FromHex("#1A1A1A") : Color.FromHex("#FFFFFF");
                MaterialColor.DisableContainer = currentTheme == OSAppTheme.Dark ? Color.FromHex("#1A1A1A") : Color.FromHex("#FFFFFF");
            });

            MaterialAnimation.Parameter = 0.7;
            MaterialAnimation.Type = AnimationTypes.Fade;
            MaterialAnimation.AnimateOnError = true;

            MaterialFontFamily.Default = "RegularFont";
            MaterialFontFamily.Regular = "MediumFont";
            MaterialFontFamily.Medium = "BoldFont";

            MaterialFontSize.DisplayLarge = 70;
            MaterialFontSize.DisplayMedium = 50;
            MaterialFontSize.BodySmall = 15;
        }
    }
}
