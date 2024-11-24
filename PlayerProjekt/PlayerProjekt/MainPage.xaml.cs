using FFImageLoading.Svg.Forms;
using MediaManager;
using Plugin.MaterialDesignControls.Material3;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PlayerProjekt
{
    public partial class MainPage : ContentPage
    {
        private readonly AudioManager audioManager;
        private string currentlyPlayingCardId;
        private SvgCachedImage currentlyPlayingIcon;
        

        public MainPage()
        {
            InitializeComponent();
            audioManager = new AudioManager();
            currentlyPlayingCardId = null;

            CrossMediaManager.Current.MediaItemFinished += OnMediaItemFinished;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadAudioFiles();
        }

        private void LoadAudioFiles()
        {
            var fileList = audioManager.GetAudioFiles();
            AddedFiles.Children.Clear();

            foreach (var file in fileList)
            {
                AddAudioCard(file, Path.GetFileNameWithoutExtension(file));
            }
        }

        private async Task<string> PickAudioFileAsync()
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "audio/mpeg", "audio/mp3" } }
            });

            var fileResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = customFileType,
                PickerTitle = "Wybierz plik audio"
            });

            return fileResult?.FullPath;
        }

        private void AddAudioCard(string filePath, string title)
        {
            var playCommand = new Command(() => PlayCardTapped(filePath));
            var deleteCommand = new Command(() => DeleteCardTapped(filePath));

            var card = AudioCard.CreateAudioCard(filePath, title, playCommand, deleteCommand);
            AddedFiles.Children.Add(card);
        }

        private async void AddNewFileTapped(object sender, EventArgs e)
        {
            var filePath = await PickAudioFileAsync();
            if (filePath == null) return;

            var destinationPath = Path.Combine(audioManager.AudioFolderPath, Path.GetFileName(filePath));
            File.Copy(filePath, destinationPath);

            audioManager.AddAudioFile(destinationPath);
            AddAudioCard(destinationPath, Path.GetFileNameWithoutExtension(filePath));
        }

        private void DeleteCardTapped(string filePath)
        {
            if (currentlyPlayingCardId == filePath)
            {
                CrossMediaManager.Current.Stop();
                currentlyPlayingCardId = null;
                currentlyPlayingIcon.Source = "resource://PlayerProjekt.Resources.play_circle_filled.svg";
            }

            audioManager.RemoveAudioFile(filePath);
            LoadAudioFiles();
        }

        private MaterialCard FindCardByFilePath(string filePath)
        {
            foreach (var child in AddedFiles.Children)
            {
                if (child is MaterialCard card && card.ClassId == filePath)
                {
                    return card;
                }
            }
            return null;
        }

        private async void PlayCardTapped(string filePath)
        {
            var card = FindCardByFilePath(filePath);
            if (card == null) return;

            var playIconGrid = ((card.Content as StackLayout).Children[3] as MaterialIconButton).Content as Grid;
            var playIcon = playIconGrid.Children[0] as SvgCachedImage;

            if (currentlyPlayingCardId == filePath)
            {
                if (CrossMediaManager.Current.IsPlaying())
                {
                    await CrossMediaManager.Current.Pause();
                    playIcon.Source = "resource://PlayerProjekt.Resources.play_circle_filled.svg";
                }
                else
                {
                    await CrossMediaManager.Current.Play();
                    playIcon.Source = "resource://PlayerProjekt.Resources.pause_circle_filled.svg";
                }
                return;
            }

            if (!string.IsNullOrEmpty(currentlyPlayingCardId))
            {
                var previousCard = FindCardByFilePath(currentlyPlayingCardId);
                if (previousCard != null)
                {
                    var previousIconGrid = ((previousCard.Content as StackLayout).Children[3] as MaterialIconButton).Content as Grid;
                    var previousIcon = previousIconGrid.Children[0] as SvgCachedImage;
                    previousIcon.Source = "resource://PlayerProjekt.Resources.play_circle_filled.svg";
                }

                await CrossMediaManager.Current.Stop();
            }

            currentlyPlayingCardId = filePath;
            currentlyPlayingIcon = playIcon;

            playIcon.Source = "resource://PlayerProjekt.Resources.pause_circle_filled.svg";
            await CrossMediaManager.Current.Play(filePath);
        }


        private void OnMediaItemFinished(object sender, MediaManager.Media.MediaItemEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (!string.IsNullOrEmpty(currentlyPlayingCardId))
                {
                    var card = FindCardByFilePath(currentlyPlayingCardId);
                    if (card != null)
                    {
                        var playIconGrid = ((card.Content as StackLayout).Children[3] as MaterialIconButton).Content as Grid;
                        var playIcon = playIconGrid.Children[0] as SvgCachedImage;
                        playIcon.Source = "resource://PlayerProjekt.Resources.play_circle_filled.svg";
                    }
                }

                currentlyPlayingCardId = null;
                currentlyPlayingIcon = null;
            });
        }
    }
}