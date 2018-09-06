using System;
using System.Threading.Tasks;
using Vkm.Library.Interfaces.Service.Player;
using Vkm.Library.Service.Player.Gpmdp.Providers;

namespace Vkm.Library.Service.Player.Gpmdp
{
    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string AlbumArt { get; set; }
    }

    public class Rating
    {
        public bool liked { get; set; }
        public bool disliked { get; set; }
    }

    public class Time
    {
        public int Current { get; set; }
        public int Total { get; set; }
    }

    public class RootObject
    {
        public bool Playing { get; set; }
        public Song Song { get; set; }
        public Rating rating { get; set; }
        public Time Time { get; set; }
        public string songLyrics { get; set; }
        public string shuffle { get; set; }
        public string repeat { get; set; }
        public int volume { get; set; }

        internal async Task<PlayingInfo> ToPlayingInfo(IBitmapDownloader bitmapDownloader)
        {
                return new PlayingInfo(Song.Title, Song.Artist, Song.Album, await bitmapDownloader.DownloadBitmap(Song.AlbumArt), Playing, TimeSpan.FromSeconds(Time.Total), TimeSpan.FromSeconds(Time.Current));
        }
    }
}