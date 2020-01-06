using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Device;
using Vkm.Api.Layout;
using Vkm.Api.Transition;
using Vkm.Api.VisualEffect;

namespace Vkm.Kernel.VisualEffect
{
    internal class VisualEffectProcessor: IDisposable
    {
        private const float FPS = 25;
        private const double DefaultDuration = 0.5;

        private readonly IDevice _device;
        private readonly IVisualTransitionFactory _visualTransitionFactory;

        private readonly AsyncAutoResetEvent _transitionsAdded;
        private readonly LazyDictionary<Location, VisualEffectInfo> _currentTransitions;

        private readonly ConcurrentQueue<LayoutDrawElement> _scheduledTransitions;

        private readonly CancellationTokenSource _cts;

        private readonly Task _drawTask;

        private readonly object _disposeLock;

        internal bool IsAllDrawn => _scheduledTransitions.IsEmpty && !_currentDrawElementsCache.Any();
        
        public VisualEffectProcessor(IDevice device, IVisualTransitionFactory visualTransitionFactory)
        {
            _device = device;
            _visualTransitionFactory = visualTransitionFactory;

            _disposeLock = new object();
            _cts = new CancellationTokenSource();
            _transitionsAdded = new AsyncAutoResetEvent();
            _currentTransitions = new LazyDictionary<Location, VisualEffectInfo>();
            _scheduledTransitions = new ConcurrentQueue<LayoutDrawElement>();

            _drawTask = DrawCycle(_cts.Token);
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
            return _visualTransitionFactory.CreateVisualTransition(drawElement.TransitionInfo.Type);
        }

        async Task DrawCycle(CancellationToken ctsToken)
        {
            do
            {
                await _transitionsAdded.WaitAsync();

                bool hasTransitions;
                do
                {
                    lock (_disposeLock)
                        hasTransitions = PerformDraw();

                    await Task.Delay((int) (1000 / FPS), ctsToken);

                } while (hasTransitions && !ctsToken.IsCancellationRequested);

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
                var localElement = drawElement;
                var value = _currentTransitions.AddOrUpdate(drawElement.Location, location =>
                    {
                        var transition = new InstantTransition();
                        return new VisualEffectInfo(localElement.Location, transition, null, localElement.BitmapRepresentation, steps);
                    },
                    (location, info) => 
                    {
                        if (info.Transition.HasNext && localElement.TransitionInfo.Type == TransitionType.Instant)
                        {
                            info.ReplaceLastBitmap(localElement.BitmapRepresentation);
                            return info;
                        }
                        else
                        {
                            var transition = GetTransition(localElement);
                            var result = new VisualEffectInfo(localElement.Location, transition, info.Transition?.Current.Clone(), localElement.BitmapRepresentation, steps);
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

            if (_currentDrawElementsCache.Any())
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