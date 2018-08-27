using System;

namespace Vkm.Library.Interfaces.Service.Player
{
    public class PlayingEventArgs : EventArgs
    {
        public PlayingInfo PlayingInfo { get; private set; }

        public PlayingEventArgs(PlayingInfo playingInfo)
        {
            PlayingInfo = playingInfo;
        }
    }
}