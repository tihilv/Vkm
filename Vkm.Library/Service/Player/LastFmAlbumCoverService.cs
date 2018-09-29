using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Options;
using Vkm.Library.Interfaces.Service.Player;
using Vkm.Library.Interfaces.Services;

namespace Vkm.Library.Service.Player
{
    public class LastFmAlbumCoverService: IAlbumCoverService, IInitializable, IOptionsProvider
    {
        public static readonly Identifier Identifier = new Identifier("Vkm.LastFmCoverService");
        
        private LastFmOptions _options;
        private IBitmapDownloadService _bitmapDownloadService;

        public Identifier Id => Identifier;
        public string Name => "LastFM Album Cover Image Service";

        public void InitContext(GlobalContext context)
        {
            _bitmapDownloadService = context.GetServices<IBitmapDownloadService>().FirstOrDefault();
        }

        public void Init()
        {
            
        }
        
        public IOptions GetDefaultOptions()
        {
            return new LastFmOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (LastFmOptions) options;
        }
        
        public async Task<BitmapRepresentation> GetCover(string artist, string album)
        {
            using (HttpClient client = new HttpClient())
            {
                using (var http = await client.GetAsync($"{_options.Domain}/2.0/?method=album.search&album={album}&api_key={_options.ApiKey}&format=json"))
                {
                    if (!http.IsSuccessStatusCode)
                        return null;

                    var result = JsonConvert.DeserializeObject<RootObject>(await http.Content.ReadAsStringAsync());

                    var bestValue = int.MinValue;
                    Album bestAlbum = null;
                    if (result.results.albummatches.album != null)
                    foreach (var alb in result?.results?.albummatches?.album)
                    {
                        var value = WeightFunction(alb, artist, album);
                        if (value > bestValue)
                        {
                            bestValue = value;
                            bestAlbum = alb;
                        }
                    }

                    if (bestAlbum != null)
                    {
                        return await _bitmapDownloadService.GetBitmap(bestAlbum.image.Last().text);
                    }
                }
            }

            return null;
        }

        private int WeightFunction(Album alb, string artist, string album)
        {
            artist = artist.ToLower();
            var result = -Math.Abs(alb.name.Length - album.Length);
            result -= Math.Abs(alb.artist.Length - artist.Length);


            var albArtist = alb.artist.ToLower();
            foreach (var artName in artist.Split(' '))
            {
                if (albArtist.Contains(artName))
                    result += artName.Length;
            }

            return result;
        }


        private class RootObject
        {
            public Results results { get; set; }
        }

        public class Results
        {
            public OpensearchQuery opensearchQuery { get; set; }
            public string opensearchtotalResults { get; set; }
            public string opensearchstartIndex { get; set; }
            public string opensearchitemsPerPage { get; set; }
            public Albummatches albummatches { get; set; }
            public Attr attr { get; set; }
        }

        public class OpensearchQuery
        {
            public string text { get; set; }
            public string role { get; set; }
            public string searchTerms { get; set; }
            public string startPage { get; set; }
        }

        public class Albummatches
        {
            public Album[] album { get; set; }
        }

        public class Album
        {
            public string name { get; set; }
            public string artist { get; set; }
            public string url { get; set; }
            public ImageDescription[] image { get; set; }
            public string streamable { get; set; }
            public string mbid { get; set; }
        }

        public class ImageDescription
        {
            [JsonProperty("#text")]
            public string text { get; set; }
            public string size { get; set; }
        }

        public class Attr
        {
            public string _for { get; set; }
        }

    }

    [Serializable]
    public class LastFmOptions : IOptions
    {
        private string _domain;
        private string _apiKey;

        public string Domain
        {
            get => _domain;
            set => _domain = value;
        }

        public string ApiKey
        {
            get => _apiKey;
            set => _apiKey = value;
        }
    }
}