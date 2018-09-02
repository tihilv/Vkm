using System;
using System.IO;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Vkm.Library.Interfaces.Service.Player;

namespace Vkm.Library.Service.Player.Gpmdp.Providers
{
    public class JsonApi : IGpmdpProvider
    {
        private const string JsonSubdirectory = @"Google Play Music Desktop Player\json_store";
        private const string JsonApiFile = "playback.json";
        
        private readonly IBitmapDownloader _bitmapDownloader;

        private readonly string _jsonApiDirectory;
        private readonly string _playbackFilePath;
        
        internal JsonApi(IBitmapDownloader bitmapDownloader)
        {
            _bitmapDownloader = bitmapDownloader;
            _jsonApiDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), JsonSubdirectory);
            _playbackFilePath = Path.Combine(_jsonApiDirectory, JsonApiFile);
        }

        public event EventHandler<PlayingEventArgs> PlayingInfoChanged;

        public void Start()
        {
            Run(_jsonApiDirectory);
        }

        public bool IsUseable()
        {
            if (!Directory.Exists(_jsonApiDirectory))
                return false;

            if (!File.Exists(_playbackFilePath))
                return false;

            return true;
        }

        private void Run(string jsonStoreDirectory)
        {
            FileSystemWatcher watcher = new FileSystemWatcher()
            {
                Path = jsonStoreDirectory,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite,
                Filter = JsonApiFile
            };
            watcher.Changed += OnChanged;
            watcher.EnableRaisingEvents = true;
        }

        private RootObject _prevRootObject;
        public async Task<PlayingInfo> GetCurrent()
        {
            string contents = null;
            int tries = 0;
            do
            {
                try
                {
                    using (var fileStream = new FileStream(_playbackFilePath, FileMode.Open, FileSystemRights.Read, FileShare.ReadWrite, 1024, FileOptions.SequentialScan))
                    using (StreamReader reader = new StreamReader(fileStream))
                        contents = reader.ReadToEnd();
                }
                catch (IOException)
                {
                    tries++;
                    Thread.Sleep(50);
                }
            } while (tries < 3 && contents == null);

            if (!string.IsNullOrEmpty(contents))
                _prevRootObject = JObject.Parse(contents).ToObject<RootObject>();

            return await _prevRootObject.ToPlayingInfo(_bitmapDownloader);
        }

        private PlayingInfo _prevInfo;
        private async void OnChanged(object source, FileSystemEventArgs e)
        {
            PlayingInfo currentInfo = await GetCurrent();
            if (!Object.Equals(currentInfo, _prevInfo))
            {
                _prevInfo = currentInfo;
                PlayingInfoChanged?.Invoke(this, new PlayingEventArgs(currentInfo));
            }
        }
    }
}
