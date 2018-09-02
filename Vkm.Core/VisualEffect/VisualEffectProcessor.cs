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
        private const float FPS = 25;
        private const double DefaultDuration = 0.5;

        private readonly IDevice _device;

        private static AutoResetEvent _transitionsAdded;
        private readonly ConcurrentDictionary<Location, VisualEffectInfo> _currentTransitions;

        private readonly ConcurrentQueue<LayoutDrawElement> _scheduledTransitions;

        private readonly CancellationTokenSource _cts;

        private readonly Task _drawTask;

        private readonly object _disposeLock;

        public VisualEffectProcessor(IDevice device)
        {
            _device = device;

            _disposeLock = new object();
            _cts = new CancellationTokenSource();
            _transitionsAdded = new AutoResetEvent(false);
            _currentTransitions = new ConcurrentDictionary<Location, VisualEffectInfo>();
            _scheduledTransitions = new ConcurrentQueue<LayoutDrawElement>();

            _drawTask = Task.Run(() => DrawCycle(_cts.Token), _cts.Token);
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

        void DrawCycle(CancellationToken ctsToken)
        {
            do
            {
                _transitionsAdded.WaitOne();

                bool hasTransitions;
                do
                {
                    lock (_disposeLock)
                        hasTransitions = PerformDraw();

                    Thread.Sleep((int) (1000 / FPS));

                } while (hasTransitions);

            } while (!ctsToken.IsCancellationRequested);
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
                            var result = new VisualEffectInfo(drawElement.Location, transition, info.Transition?.Current.Clone(), drawElement.BitmapRepresentation, steps);
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

            _device.SetBitmaps(_currentDrawElementsCache);

            return _currentDrawElementsCache.Any();
        }

        public void Dispose()
        {
            _cts.Cancel();

            lock (_disposeLock)
            {
                while (_scheduledTransitions.TryDequeue(out var layoutDrawElement))
                    layoutDrawElement.BitmapRepresentation.Dispose();

                foreach (var effectInfo in _currentTransitions.Values)
                    effectInfo.Dispose();

                _currentTransitions.Clear();
            }
        }
    }
}