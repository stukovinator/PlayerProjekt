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
        private string currentlyPlayingCardId;
        private SvgCachedImage currentlyPlayingIcon;
        private readonly string audioFolderPath;
        private readonly string fileListPath;

        public MainPage()
        {
            InitializeComponent();
            currentlyPlayingCardId = null;

            audioFolderPath = Path.Combine(FileSystem.AppDataDirectory, "AudioFiles");
            fileListPath = Path.Combine(FileSystem.AppDataDirectory, "fileList.json");

            Console.WriteLine($"Ścieżka folderu AudioFiles: {audioFolderPath}");

            CrossMediaManager.Current.MediaItemFinished += OnMediaItemFinished;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadAudioFiles();
        }

        private void LoadAudioFiles()
        {
            if (File.Exists(fileListPath))
            {
                var fileList = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(fileListPath));
                AddedFiles.Children.Clear();
                foreach (var file in fileList)
                {
                    AddAudioCard(file, Path.GetFileNameWithoutExtension(file));
                }
            }
        }

        public async void AddNewFileTapped(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(audioFolderPath))
                {
                    Directory.CreateDirectory(audioFolderPath);
                }

                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "audio/mpeg", "audio/mp3" } }
                });

                var fileResult = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = customFileType,
                    PickerTitle = "Wybierz plik audio"
                });

                if (fileResult == null) return; // Anulowanie wyboru pliku

                var destinationPath = Path.Combine(audioFolderPath, fileResult.FileName);

                using (var sourceStream = await fileResult.OpenReadAsync())
                using (var destinationStream = File.Create(destinationPath))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }

                // Zapisz do listy i dodaj kartę
                SaveAudioFile(destinationPath);
                AddAudioCard(destinationPath, Path.GetFileNameWithoutExtension(fileResult.FileName));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd dodawania pliku: {ex.Message}");
            }
        }

        private void SaveAudioFile(string filePath)
        {
            List<string> fileList = new List<string>();

            if (File.Exists(fileListPath))
            {
                fileList = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(fileListPath));
            }

            fileList.Add(filePath);

            File.WriteAllText(fileListPath, JsonConvert.SerializeObject(fileList));
        }

        private void AddAudioCard(string filePath, string title)
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
                WidthRequest = 24,
                HeightRequest = 24,
                Content = new SvgCachedImage { Source = "resource://PlayerProjekt.Resources.audio.svg" }
            };

            var textStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(5, 0, 0, 0),
                Children =
                {
                    new Label { Text = title, TextColor = Color.FromHex("#142357"), FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), FontFamily = "JosefinSansBold" }
                }
            };

            var playButton = new MaterialIconButton
            {
                Content = new Grid
                {
                    GestureRecognizers = {
                        new TapGestureRecognizer { Command = new Command(() => PlayCardTapped(card)) }
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
                        new TapGestureRecognizer { Command = new Command(() => DeleteCardTapped(card)) }
                    },
                    Children = { new SvgCachedImage { Source = "resource://PlayerProjekt.Resources.delete_forever.svg", HeightRequest = 12, WidthRequest = 12 } }
                }
            };

            stackLayout.Children.Add(frame);
            stackLayout.Children.Add(textStack);
            stackLayout.Children.Add(deleteButton);
            stackLayout.Children.Add(playButton);
            card.Content = stackLayout;

            AddedFiles.Children.Add(card);
        }

        private async void DeleteCardTapped(MaterialCard card)
        {
            try
            {
                var filePath = card.ClassId;
                if (string.IsNullOrEmpty(filePath))
                    return;

                if (currentlyPlayingCardId == filePath)
                {
                    await CrossMediaManager.Current.Stop();
                    currentlyPlayingCardId = null;
                    currentlyPlayingIcon.Source = "resource://PlayerProjekt.Resources.play_circle_filled.svg";
                    Console.WriteLine($"Odtwarzanie pliku {filePath} zostało zatrzymane przed usunięciem.");
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Console.WriteLine($"Plik usunięty z systemu plików: {filePath}");
                }
                else
                {
                    Console.WriteLine($"Plik nie znaleziony w systemie plików: {filePath}");
                }

                if (File.Exists(fileListPath))
                {
                    List<string> fileList = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(fileListPath));

                    bool removed = fileList.Remove(filePath);
                    if (removed)
                    {
                        Console.WriteLine($"Ścieżka pliku usunięta z listy JSON: {filePath}");
                        File.WriteAllText(fileListPath, JsonConvert.SerializeObject(fileList));
                    }
                    else
                    {
                        Console.WriteLine($"Ścieżka pliku nie została znaleziona w liście JSON: {filePath}");
                    }
                }
                else
                {
                    Console.WriteLine($"Plik JSON nie istnieje: {fileListPath}");
                }

                AddedFiles.Children.Remove(card);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd przy usuwaniu pliku: {ex.Message}");
            }
        }


        private async void PlayCardTapped(MaterialCard card)
        {
            var playIconGrid = ((card.Content as StackLayout).Children[3] as MaterialIconButton).Content as Grid;
            var playIcon = playIconGrid.Children[0] as SvgCachedImage;

            if (currentlyPlayingCardId == card.ClassId)
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

            if (!string.IsNullOrEmpty(currentlyPlayingCardId) && currentlyPlayingIcon != null)
            {
                currentlyPlayingIcon.Source = "resource://PlayerProjekt.Resources.play_circle_filled.svg";
                await CrossMediaManager.Current.Stop();
            }

            currentlyPlayingCardId = card.ClassId;
            currentlyPlayingIcon = playIcon;

            playIcon.Source = "resource://PlayerProjekt.Resources.pause_circle_filled.svg";
            await CrossMediaManager.Current.Play(card.ClassId);
        }

        private void OnMediaItemFinished(object sender, MediaManager.Media.MediaItemEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                // Zmień ikonę na "play" dla aktualnie odtwarzanej karty
                if (currentlyPlayingIcon != null)
                {
                    currentlyPlayingIcon.Source = "resource://PlayerProjekt.Resources.play_circle_filled.svg";
                }

                // Wyczyść stan
                currentlyPlayingCardId = null;
                currentlyPlayingIcon = null;
            });
        }
    }
}