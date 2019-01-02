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

namespace Vkm.Kernel.VisualEffect
{
    internal class VisualEffectProcessor: IDisposable
    {
        private const float FPS = 25;
        private const double DefaultDuration = 0.5;

        private readonly IDevice _device;
        private readonly IVisualTransitionFactory _visualTransitionFactory;

        private readonly AutoResetEvent _transitionsAdded;
        private readonly ConcurrentDictionary<Location, Lazy<VisualEffectInfo>> _currentTransitions;

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
            _transitionsAdded = new AutoResetEvent(false);
            _currentTransitions = new ConcurrentDictionary<Location, Lazy<VisualEffectInfo>>();
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
            return _visualTransitionFactory.CreateVisualTransition(drawElement.TransitionInfo.Type);
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
                var value = _currentTransitions.AddOrUpdate(drawElement.Location, location =>
                    {
                        var transition = new InstantTransition();
                        return new Lazy<VisualEffectInfo>(() => new VisualEffectInfo(drawElement.Location, transition, null, drawElement.BitmapRepresentation, steps));
                    },
                    (location, info) => new Lazy<VisualEffectInfo>(()=>
                    {
                        if (info.Value.Transition.HasNext && drawElement.TransitionInfo.Type == TransitionType.Instant)
                        {
                            info.Value.ReplaceLastBitmap(drawElement.BitmapRepresentation);
                            return info.Value;
                        }
                        else
                        {
                            var transition = GetTransition(drawElement);
                            var result = new VisualEffectInfo(drawElement.Location, transition, info.Value.Transition?.Current.Clone(), drawElement.BitmapRepresentation, steps);
                            info.Value.Dispose();
                            return result;
                        }
                    })).Value;
            }

            var effectInfos = _currentTransitions.Values.Where(t => t.Value.Transition.HasNext);

            _currentDrawElementsCache.Clear();

            foreach (var effectInfo in effectInfos.Select(e=>e.Value))
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

                foreach (var effectInfo in _currentTransitions.Values.Select(v=>v.Value))
                    effectInfo.Dispose();

                _currentTransitions.Clear();
            }
        }
    }
}