using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Time;

namespace Vkm.Api.Layout
{
    public abstract class LayoutBase: ILayout, IInitializable
    {
        private GlobalContext _globalContext;
        private LayoutContext _layoutContext;
        
        readonly object _layoutSwitchLock = new object();

        private readonly ConcurrentDictionary<ITimerToken, ITimerToken> _timers;

        private bool _inLayout;

        private ILayout _previousLayout;

        public Identifier Id { get; private set; }
        private readonly ElementKeeper _elements;

        public virtual byte? PreferredBrightness => null;

        protected GlobalContext GlobalContext => _globalContext;

        public event EventHandler<DrawEventArgs> DrawLayout;

        protected IEnumerable<ElementPlacement> Elements => _elements.Values;

        protected LayoutBase(Identifier identifier)
        {
            Id = identifier;

            _elements = new ElementKeeper();
            _timers = new ConcurrentDictionary<ITimerToken, ITimerToken>();
        }

        public virtual void InitContext(GlobalContext context)
        {
            _globalContext = context;
        }

        public virtual void Init()
        {

        }

        protected virtual void OnEnteringLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            
        }
        
        protected virtual void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            
        }
        
        public void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            lock (_layoutSwitchLock)
            {
                OnEnteringLayout(layoutContext, previousLayout);

                Debug.Assert(!_inLayout, "Already in layout");

                _previousLayout = previousLayout;
                _inLayout = true;

                _layoutContext = layoutContext;

                foreach (var placement in _elements.Values)
                    ConnectToElement(placement, previousLayout);

                foreach (var timer in _timers.Values)
                    timer.Start();

                OnEnteredLayout(layoutContext, previousLayout);
            }
        }

        protected virtual void OnLeavingLayout()
        {
            
        }

        protected virtual void OnLeavedLayout()
        {
            
        }
        
        public void LeaveLayout()
        {
            lock (_layoutSwitchLock)
            {
                OnLeavingLayout();
                
                Debug.Assert(_inLayout, "Not in layout");
                _inLayout = false;

                foreach (var timer in _timers.Values)
                    timer.Stop();

                foreach (var placement in _elements.Values)
                    DisconnectFromElement(placement);
                
                OnLeavedLayout();
            }
        }

        private void ConnectToElement(ElementPlacement placement, ILayout previousLayout)
        {
            if ((placement.Location.X + placement.Element.ButtonCount.Width > _layoutContext.ButtonCount.Width) ||(placement.Location.Y + placement.Element.ButtonCount.Height > _layoutContext.ButtonCount.Height))
                throw new ArgumentException($"Element {placement.Element.Id.Value} of type {placement.Element.GetType().FullName} attempted to put out of the borders.");

            placement.Element.DrawElement += ElementOnDrawElement;
            placement.Element.EnterLayout(_layoutContext, previousLayout);
        }

        private void DisconnectFromElement(ElementPlacement placement)
        {
            placement.Element.LeaveLayout();
            placement.Element.DrawElement -= ElementOnDrawElement;

            LayoutDrawElement[] layoutDrawElements = new LayoutDrawElement[placement.Element.ButtonCount.Height*placement.Element.ButtonCount.Width];
            int index = 0;
            for (byte x = 0; x < placement.Element.ButtonCount.Width; x++)
            for (byte y = 0; y < placement.Element.ButtonCount.Height; y++)
            {
                layoutDrawElements[index] = new LayoutDrawElement(new Location((byte) (placement.Location.X + x), (byte) (placement.Location.Y + y)), _layoutContext.CreateBitmap());
                index++;
            }

            DrawLayout?.Invoke(this, new DrawEventArgs(layoutDrawElements));
        }

        protected void AddElement(Location location, IElement element)
        {
            var placement = new ElementPlacement(location, element);
            _elements.Add(placement);

            if (_inLayout)
                ConnectToElement(placement, _previousLayout);
        }

        protected void RemoveElement(IElement element)
        {
            var placement = _elements.Remove(element);

            if (_inLayout)
                DisconnectFromElement(placement);
        }

        protected void RemoveElement(Location location)
        {
            var placement = _elements.Remove(location);

            if (_inLayout)
                DisconnectFromElement(placement);
        }
        
        private void ElementOnDrawElement(object sender, DrawEventArgs e)
        {
            var placedElement = _elements.Values.FirstOrDefault(el=> el.Element == sender);
            if (placedElement.Element != null)
            {
                if (e.Elements.Any(el => el.Location.X >= placedElement.Element.ButtonCount.Width || el.Location.Y >= placedElement.Element.ButtonCount.Height))
                    throw new ArgumentException($"Element {placedElement.Element.Id.Value} of type {placedElement.Element.GetType().FullName} attempted to draw out of the borders.");

                var drawEventArgs = new DrawEventArgs(e.Elements.Select(el => new LayoutDrawElement(placedElement.Location + el.Location, el.BitmapRepresentation, el.TransitionInfo)).ToArray());
                DrawLayout?.Invoke(this, drawEventArgs);
            }
        }

        protected void AddElementsInRectangle(IEnumerable<IElement> elements)
        {
            AddElementsInRectangle(elements, 0, 0, (byte) (_layoutContext.ButtonCount.Width - 1), (byte) (_layoutContext.ButtonCount.Height - 1));
        }

        protected void AddElementsInRectangle(IEnumerable<IElement> elements, byte fromX, byte fromY, byte toX, byte toY)
        {
            using (_layoutContext.PauseDrawing())
            {
                byte width = (byte) (toX - fromX + 1);

                int i = 0;

                foreach (var element in elements)
                {
                    AddElement(new Location(fromX + i % width, fromY + i / width), element);
                    i++;
                }
            }
        }


        public virtual void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            var elementsToLook = _elements.Values.ToArray();

            foreach (ElementPlacement placement in elementsToLook)
            {
                if (location.X >= placement.Location.X && location.X < placement.Location.X + placement.Element.ButtonCount.Width && location.Y >= placement.Location.Y && location.Y < placement.Location.Y + placement.Element.ButtonCount.Height)
                    placement.Element.ButtonPressed(new Location((byte) (location.X - placement.Location.X), (byte) (location.Y - placement.Location.Y)), buttonEvent, _layoutContext);
            }
        }

        protected void RegisterTimer(TimeSpan interval, Action action)
        {
            var timer = GlobalContext.Services.TimerService.RegisterTimer(interval, action);
            _timers.TryAdd(timer, timer);
        }
        
        protected void WithLayout(Action<LayoutContext> action)
        {
            var lc = _layoutContext;
            if (lc != null)
                action(lc);
        }

        protected void DrawInvoke(IEnumerable<LayoutDrawElement> drawElements)
        {
            DrawLayout?.Invoke(this, new DrawEventArgs(drawElements.ToArray()));
        }

        class ElementKeeper
        {
            private readonly ConcurrentDictionary<Location, IElement> _elementPlacements;
            private readonly ConcurrentDictionary<IElement, Location> _elements;

            public ElementKeeper()
            {
                _elementPlacements = new ConcurrentDictionary<Location, IElement>();
                _elements = new ConcurrentDictionary<IElement, Location>();
            }

            public IEnumerable<ElementPlacement> Values
            {
                get
                {
                    foreach (var pair in _elementPlacements)
                    {
                        yield return new ElementPlacement(pair.Key, pair.Value);
                    }
                }
            }

            public void Add(ElementPlacement placement)
            {
                _elementPlacements[placement.Location] = placement.Element;
                _elements[placement.Element] = placement.Location;
            }

            public ElementPlacement Remove(IElement element)
            {
                if (_elements.TryRemove(element, out var location))
                    _elementPlacements.TryRemove(location, out _);

                return new ElementPlacement(location, element);
            }

            public ElementPlacement Remove(Location location)
            {
                if (_elementPlacements.TryRemove(location, out var element))
                    _elements.TryRemove(element, out _);

                return new ElementPlacement(location, element);

            }
        }
    }
}
