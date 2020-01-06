using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Options;
using Vkm.Library.Interfaces.Service.Player;

namespace Vkm.Library.Service.Player
{
    public class AmipPlayerService: IPlayerService, IInitializable, IOptionsProvider
    {
        public static Identifier Identifier = new Identifier("Vkm.AmipService");
        private const int UpdateTimeoutMs = 1000;
        public Identifier Id => Identifier;

        public string Name => "Amip Music Service";

        private AmipOptions _options;
        private IAlbumCoverService _albumCoverService;
        
        private Task _updateTask;
        private readonly AsyncAutoResetEvent _updateSync;
        
        public AmipPlayerService()
        {
            _updateSync = new AsyncAutoResetEvent();
        }

        public event EventHandler<PlayingEventArgs> PlayingInfoChanged;

        public void InitContext(GlobalContext context)
        {
            _albumCoverService = context.GetServices<IAlbumCoverService>().FirstOrDefault();
        }

        public void Init()
        {
            Run();
            _updateTask = UpdateAction();
        }

        public IOptions GetDefaultOptions()
        {
            return new AmipOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (AmipOptions) options;
        }
        
        private void Run()
        {
            FileSystemWatcher watcher = new FileSystemWatcher()
            {
                Path = Path.GetDirectoryName(_options.Filename),
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite,
                Filter = Path.GetFileName(_options.Filename)
            };
            watcher.Changed += OnChanged;
            watcher.EnableRaisingEvents = true;
        }

        private string _prevContents;
        public async Task<PlayingInfo> GetCurrent()
        {
            bool read = false;
            int tries = 0;
            do
            {
                try
                {
                    if (File.GetLastWriteTimeUtc(_options.Filename).AddMinutes(10) < DateTime.UtcNow)
                    {
                        _prevContents = null;
                        read = true;
                    }
                    else
                    {
                        using (var fileStream = new FileStream(_options.Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 10240, FileOptions.SequentialScan))
                        using (StreamReader reader = new StreamReader(fileStream))
                            _prevContents = reader.ReadToEnd();

                        read = true;
                    }
                }
                catch (IOException)
                {
                    tries++;
                    Thread.Sleep(50);
                }
            } while (tries < 3 && !read);


            if (_prevContents == null)
                return await Task.FromResult<PlayingInfo>(null);

            var splitted = _prevContents.Split(_options.Separator, StringSplitOptions.None);

            var artist = splitted[0];
            var album = splitted[1];
            var title = splitted[2];
            return new PlayingInfo(title, artist, album, await _albumCoverService.GetCover(artist, album), splitted[3] == "P", TimeSpan.FromSeconds(1), TimeSpan.Zero);
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

                    await _updateSync.WaitAsync();
                }
                catch
                {
                    // doesn't matter if we got an exception here.
                }
            }

        }
    }

    [Serializable]
    public class AmipOptions : IOptions
    {
        private string _filename = "e:\\1.txt";
        private string[] _separator = new []{ " ||| "};

        public string Filename
        {
            get => _filename;
            set => _filename = value;
        }

        public string[] Separator
        {
            get => _separator;
            set => _separator = value;
        }
    }
}