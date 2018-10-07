using System;
using System.IO;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vkm.Library.Interfaces.Service.Player;
using Vkm.Library.Interfaces.Services;

namespace Vkm.Library.Service.Player.Gpmdp.Providers
{
    public class JsonApi : IGpmdpProvider
    {
        private const int UpdateTimeoutMs = 1000;
        private const string JsonSubdirectory = @"Google Play Music Desktop Player\json_store";
        private const string JsonApiFile = "playback.json";
        
        private readonly IBitmapDownloadService _bitmapDownloadService;

        private readonly string _jsonApiDirectory;
        private readonly string _playbackFilePath;

        private Task _updateTask;
        private readonly AutoResetEvent _updateSync;
        
        internal JsonApi(IBitmapDownloadService bitmapDownloadService)
        {
            _bitmapDownloadService = bitmapDownloadService;
            _jsonApiDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), JsonSubdirectory);
            _playbackFilePath = Path.Combine(_jsonApiDirectory, JsonApiFile);
            
            _updateSync = new AutoResetEvent(false);

            _updateTask = UpdateAction();
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
            bool read = false;
            int tries = 0;
            do
            {
                try
                {
                    if (File.GetLastWriteTimeUtc(_playbackFilePath).AddMinutes(1) < DateTime.UtcNow)
                    {
                        _prevRootObject = null;
                        read = true;
                    }
                    else
                    {
                        string contents;

                        using (var fileStream = new FileStream(_playbackFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 10240, FileOptions.SequentialScan))
                        using (StreamReader reader = new StreamReader(fileStream))
                            contents = reader.ReadToEnd();

                        if (!string.IsNullOrEmpty(contents))
                            _prevRootObject = JObject.Parse(contents).ToObject<RootObject>();
                        read = true;
                    }
                }
                catch (JsonReaderException)
                {
                    tries++;
                    Thread.Sleep(50);
                }
                catch (IOException)
                {
                    tries++;
                    Thread.Sleep(50);
                }
            } while (tries < 3 && !read);


            if (_prevRootObject == null)
                return await Task.FromResult<PlayingInfo>(null);

            return await _prevRootObject.ToPlayingInfo(_bitmapDownloadService);
        }

        private PlayingInfo _prevInfo;
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            _updateSync.Set();
        }

        private async Task UpdateAction()
        {
            while (true)
            {
                try
                {
                    using (PlayingInfo currentInfo = await GetCurrent())
                        if (!Object.Equals(currentInfo, _prevInfo))
                        {
                            _prevInfo = currentInfo;
                            PlayingInfoChanged?.Invoke(this, new PlayingEventArgs(currentInfo));
                        }

                    await Task.Delay(UpdateTimeoutMs);

                    _updateSync.WaitOne();
                }
                catch
                {
                    // doesn't matter if we got an exception here.
                }
            }

        }

    }
}
