using System;
using Vkm.Api.Basic;

namespace Vkm.Library.Interfaces.Service.Player
{
    public class PlayingInfo: IDisposable
    {
        public string Title { get; private set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public BitmapRepresentation BitmapRepresentation { get; private set; }
        public bool IsPlaying {get; private set; }
        public TimeSpan DurationSpan { get; private set; }
        public TimeSpan CurrentPosition { get; private set; }

        public PlayingInfo(string title, string artist, string album, BitmapRepresentation bitmapRepresentation, bool isPlaying, TimeSpan durationSpan, TimeSpan currentPosition)
        {
            Title = title;
            Artist = artist;
            Album = album;
            BitmapRepresentation = bitmapRepresentation;
            IsPlaying = isPlaying;
            DurationSpan = durationSpan;
            CurrentPosition = currentPosition;
        }

        protected bool Equals(PlayingInfo other)
        {
            return string.Equals(Title, other.Title) && string.Equals(Artist, other.Artist) && string.Equals(Album, other.Album) && Equals(BitmapRepresentation, other.BitmapRepresentation) && IsPlaying == other.IsPlaying && DurationSpan.Equals(other.DurationSpan) && CurrentPosition.Equals(other.CurrentPosition);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PlayingInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Artist != null ? Artist.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Album != null ? Album.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BitmapRepresentation != null ? BitmapRepresentation.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsPlaying.GetHashCode();
                hashCode = (hashCode * 397) ^ DurationSpan.GetHashCode();
                hashCode = (hashCode * 397) ^ CurrentPosition.GetHashCode();
                return hashCode;
            }
        }

        public void Dispose()
        {
            BitmapRepresentation?.Dispose();
        }
    }
}