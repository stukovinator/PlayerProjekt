using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using System.Threading.Tasks;
using System.IO;

namespace PlayerProjekt
{
    public class AudioManager
    {
        private readonly string audioFolderPath;
        private readonly string fileListPath;
        public string AudioFolderPath => audioFolderPath;

        public AudioManager()
        {
            audioFolderPath = Path.Combine(FileSystem.AppDataDirectory, "AudioFiles");
            fileListPath = Path.Combine(FileSystem.AppDataDirectory, "fileList.json");

            if (!Directory.Exists(audioFolderPath))
            {
                Directory.CreateDirectory(audioFolderPath);
            }
        }

        public List<string> GetAudioFiles()
        {
            if (!File.Exists(fileListPath))
            {
                return new List<string>();
            }

            return JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(fileListPath));
        }

        public void AddAudioFile(string filePath)
        {
            var fileList = GetAudioFiles();
            fileList.Add(filePath);
            File.WriteAllText(fileListPath, JsonConvert.SerializeObject(fileList));
        }

        public void RemoveAudioFile(string filePath)
        {
            var fileList = GetAudioFiles();
            if (fileList.Remove(filePath))
            {
                File.WriteAllText(fileListPath, JsonConvert.SerializeObject(fileList));
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
