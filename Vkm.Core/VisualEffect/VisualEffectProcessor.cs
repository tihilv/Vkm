using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vkm.Api.Basic;
using Vkm.Api.Device;
using Vkm.Api.Layout;
using Vkm.Api.Transition;
using Vkm.Api.VisualEffect;
using Vkm.Core.VisualEffect;

namespace Vkm.Core
{
    internal class VisualEffectProcessor: IDisposable
    {
        private const float FPS = 20;
        private const int DefaultDuration = 1;

        private readonly IDevice _device;

        private static AutoResetEvent _transitionsAdded;
        private readonly ConcurrentDictionary<Location, VisualEffectInfo> _currentTransitions;

        private readonly ConcurrentQueue<LayoutDrawElement> _scheduledTransitions;

        private bool _disposed;

        private Task _drawTask;

        public VisualEffectProcessor(IDevice device)
        {
            _device = device;

            _transitionsAdded = new AutoResetEvent(false);
            _currentTransitions = new ConcurrentDictionary<Location, VisualEffectInfo>();
            _scheduledTransitions = new ConcurrentQueue<LayoutDrawElement>();

            _drawTask = Task.Run(() => DrawCycle());
        }

        public void Draw(IEnumerable<LayoutDrawElement> drawElements)
        {
            bool hasNewElement = false;

            foreach (var drawElement in drawElements)
            {
                _scheduledTransitions.Enqueue(drawElement);
                hasNewElement = true;
            }

            if (hasNewElement)
                _transitionsAdded.Set();
        }

        IVisualTransition GetTransition(LayoutDrawElement drawElement)
        {
            if (drawElement.TransitionInfo.Type == TransitionType.Instant)
                return new InstantTransition();

            if (drawElement.TransitionInfo.Type == TransitionType.ElementUpdate)
                return new FadeTransition();

            if (drawElement.TransitionInfo.Type == TransitionType.LayoutChange)
                return new MoveTransition();

            return new InstantTransition();
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
            while (_scheduledTransitions.TryDequeue(out var drawElement))
            {
                var secs = drawElement.TransitionInfo.Duration.TotalSeconds;
                if (secs == 0)
                    secs = DefaultDuration;

                int steps = (int) (secs * FPS);
                _currentTransitions.AddOrUpdate(drawElement.Location, location =>
                    {
                        var transition = new InstantTransition();
                        return new VisualEffectInfo(drawElement.Location, transition, null, drawElement.BitmapRepresentation, steps);
                    },
                    (location, info) =>
                    {
                        if (info.Transition.HasNext && drawElement.TransitionInfo.Type == TransitionType.Instant)
                        {
                            info.ReplaceLastBitmap(drawElement.BitmapRepresentation);
                            return info;
                        }
                        else
                        {
                            var transition = GetTransition(drawElement);
                            var result = new VisualEffectInfo(drawElement.Location, transition, info.Transition.Current.Clone(), drawElement.BitmapRepresentation, steps);
                            info.Dispose();
                            return result;
                        }
                    });
            }

            var effectInfos = _currentTransitions.Values.Where(t => t.Transition.HasNext);

            _currentDrawElementsCache.Clear();

            foreach (var effectInfo in effectInfos)
            {
                effectInfo.Transition.Next();
                _currentDrawElementsCache.Add(new LayoutDrawElement(effectInfo.Location, effectInfo.Transition.Current));
            }

            foreach (var currentElement in _currentDrawElementsCache)
                _device.SetBitmap(currentElement.Location, currentElement.BitmapRepresentation);

            Thread.Sleep((int) (1000 / FPS));

            return _currentDrawElementsCache.Any();
        }

        public void Dispose()
        {
            _disposed = true;
            DisposeHelper.DisposeAndNull(ref _drawTask);

            foreach (var effectInfo in _currentTransitions.Values)
                effectInfo.Dispose();

            _currentTransitions.Clear();
        }
    }
}