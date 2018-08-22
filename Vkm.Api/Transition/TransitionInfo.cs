using System;

namespace Vkm.Api.Transition
{
    public struct TransitionInfo
    {
        public readonly TransitionType Type;
        public readonly TimeSpan Duration;

        public TransitionInfo(TransitionType type, TimeSpan duration)
        {
            Type = type;
            Duration = duration;
        }
    }

    public enum TransitionType
    {
        Instant, 
        ElementUpdate,
        LayoutUpdate,
        LayoutChange
    }
}
