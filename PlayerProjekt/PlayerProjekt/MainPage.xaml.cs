using FFImageLoading.Svg.Forms;
using Plugin.MaterialDesignControls.Material3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PlayerProjekt
{
    public partial class MainPage : ContentPage
    {
        public string currentlyPlaying;
        public MainPage()
        {
            InitializeComponent();
            currentlyPlaying = "";
        }

        private async void Play_Tapped(object sender, EventArgs e)
        {
            Grid button = sender as Grid;
            var play = button.Children[0] as SvgCachedImage;
            var card = button.Parent.Parent.Parent as MaterialCard;
            Console.WriteLine(card.ClassId);

            await Task.Delay(100);

            if(card.ClassId == "paused")
            {
                play.Source = "resource://PlayerProjekt.Resources.pause_circle_filled.svg";
                card.ClassId = "playing";
            }
            else
            {
                play.Source = "resource://PlayerProjekt.Resources.play_circle_filled.svg";
                card.ClassId = "paused";
            }

        }

        private void AddNewFileTapped(object sender, EventArgs e)
        {

        }
    }
}
