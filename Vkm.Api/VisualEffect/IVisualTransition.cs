using System;
using Vkm.Api.Basic;

namespace Vkm.Api.VisualEffect
{
    public interface IVisualTransition: IDisposable
    {
        BitmapRepresentation Current { get; }
        
        bool HasNext { get; }

        void Init(BitmapRepresentation first, BitmapRepresentation last);

        void Next();
    }
}