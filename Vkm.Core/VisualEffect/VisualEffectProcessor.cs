using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vkm.Api.Basic;
using Vkm.Api.Device;
using Vkm.Api.Layout;
using Vkm.Api.VisualEffect;
using Vkm.Core.VisualEffect;

namespace Vkm.Core
{
    internal class VisualEffectProcessor: IDisposable
    {
        private readonly IDevice _device;

        private static AutoResetEvent _transitionsAdded;
        private readonly ConcurrentDictionary<Location, VisualEffectInfo> _currentTransitions;

        private bool _disposed;

        private Task _drawTask;

        public VisualEffectProcessor(IDevice device)
        {
            _device = device;

            _transitionsAdded = new AutoResetEvent(false);
            _currentTransitions = new ConcurrentDictionary<Location, VisualEffectInfo>();

            _drawTask = Task.Run(() => DrawCycle());
        }

        public void Draw(IEnumerable<LayoutDrawElement> drawElements)
        {
            bool hasNewElement = false;
            foreach (var drawElement in drawElements)
            {
                var transition = GetTransition(drawElement);

                _currentTransitions.AddOrUpdate(drawElement.Location, location =>
                    {
                        return new VisualEffectInfo(drawElement.Location, transition, null, drawElement.BitmapRepresentation);
                    },
                    (location, info) =>
                    {
                        var result = new VisualEffectInfo(drawElement.Location, transition, info.Transition.Current.Clone(), drawElement.BitmapRepresentation);
                        info.Dispose();
                        return result;
                    });
                hasNewElement = true;
            }

            if (hasNewElement)
                _transitionsAdded.Set();
        }

        IVisualTransition GetTransition(LayoutDrawElement drawElement)
        {
            return new ImmediateTransition();
        }

        void DrawCycle()
        {
            do
            {
                _transitionsAdded.WaitOne();

                bool hasTransitions;
                do
                {
                    hasTransitions = PerformDraw();

                } while (hasTransitions);

            } while (!_disposed);
        }

        readonly List<LayoutDrawElement> _currentDrawElementsCache = new List<LayoutDrawElement>();
        bool PerformDraw()
        {
            var effectInfos = _currentTransitions.Values.Where(t => t.Transition.HasNext);
            
            _currentDrawElementsCache.Clear();
            foreach (var effectInfo in effectInfos)
            {
                effectInfo.Transition.Next();
                _currentDrawElementsCache.Add(new LayoutDrawElement(effectInfo.Location, effectInfo.Transition.Current));
            }

            foreach (var drawElement in _currentDrawElementsCache)
                _device.SetBitmap(drawElement.Location, drawElement.BitmapRepresentation);

            return _currentDrawElementsCache.Any();
        }

        public void Dispose()
        {
            _disposed = true;
            _drawTask.Dispose();

            foreach (var effectInfo in _currentTransitions.Values)
                effectInfo.Dispose();
        }
    }

    internal class ImmediateTransition : IVisualTransition
    {
        private bool _first = true;
        private BitmapRepresentation _current;

        public BitmapRepresentation Current => _current;

        public bool HasNext => _first;

        public void Init(BitmapRepresentation first, BitmapRepresentation last)
        {
            _current = last.Clone();
        }

        public void Next()
        {
            _first = false;
        }

        public void Dispose()
        {
            _current.Dispose();
        }

    }
}