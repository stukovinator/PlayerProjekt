using FFImageLoading.Svg.Forms;
using Plugin.MaterialDesignControls.Material3;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PlayerProjekt
{
    public class AudioCard
    {
        public static MaterialCard CreateAudioCard(string filePath, string title, Command playCommand, Command deleteCommand)
        {
            var card = new MaterialCard
            {
                Type = MaterialCardType.Filled,
                BackgroundColor = Color.FromHex("#eef6ff"),
                Padding = 5,
                ClassId = filePath
            };

            var stackLayout = new StackLayout { Orientation = StackOrientation.Horizontal };

            var frame = new Frame
            {
                CornerRadius = 6,
                BackgroundColor = Color.White,
                Margin = new Thickness(0, 0, 5, 0),
                Content = new SvgCachedImage { Source = "resource://PlayerProjekt.Resources.audio.svg", Scale = 3 }
            };

            var textStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Children =
                {
                    new Label { Text = title, TextColor = Color.FromHex("#142357"), LineBreakMode = LineBreakMode.TailTruncation, MaxLines = 1, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), FontFamily = "JosefinSansBold" }
                }
            };

            var playButton = new MaterialIconButton
            {
                Content = new Grid
                {
                    GestureRecognizers = {
                        new TapGestureRecognizer { Command = playCommand }
                    },
                    Children = { new SvgCachedImage { Source = "resource://PlayerProjekt.Resources.play_circle_filled.svg", HeightRequest = 24, WidthRequest = 24 } }
                }
            };

            var deleteButton = new MaterialIconButton
            {
                WidthRequest = 24,
                HeightRequest = 24,
                Margin = new Thickness(0, 0, 15, 0),
                Content = new Grid
                {
                    GestureRecognizers = {
                        new TapGestureRecognizer { Command = deleteCommand }
                    },
                    Children = { new SvgCachedImage { Source = "resource://PlayerProjekt.Resources.delete_forever.svg", HeightRequest = 12, WidthRequest = 12 } }
                }
            };

            stackLayout.Children.Add(frame);
            stackLayout.Children.Add(textStack);
            stackLayout.Children.Add(deleteButton);
            stackLayout.Children.Add(playButton);
            card.Content = stackLayout;

            return card;
        }
    }
}
