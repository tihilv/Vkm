using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private readonly ConcurrentDictionary<ITimerToken, ITimerToken> _timers;

        private bool _inLayout;

        public Identifier Id { get; private set; }
        private readonly ConcurrentDictionary<ElementPlacement, ElementPlacement> _elements;

        public virtual byte? PreferredBrightness => null;

        protected LayoutContext LayoutContext => _layoutContext;
        protected GlobalContext GlobalContext => _globalContext;

        public event EventHandler<DrawEventArgs> DrawLayout;

        protected LayoutBase(Identifier identifier)
        {
            Id = identifier;

            _elements = new ConcurrentDictionary<ElementPlacement, ElementPlacement>();
            _timers = new ConcurrentDictionary<ITimerToken, ITimerToken>();
        }

        public virtual void EnterLayout(LayoutContext layoutContext)
        {
            _inLayout = true;

            _layoutContext = layoutContext;

            foreach (var placement in _elements.Values)
                ConnectToElement(placement.Element);

            foreach (var timer in _timers.Values)
                timer.Start();
        }

        public virtual void LeaveLayout()
        {
            _inLayout = false;

            foreach (var timer in _timers.Values)
                timer.Stop();

            foreach (var placement in _elements.Values)
                DisconnectFromElement(placement);
        }

        private void ConnectToElement(IElement element)
        {
            element.DrawElement += ElementOnDrawElement;
            element.EnterLayout(_layoutContext);
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
                layoutDrawElements[index] = new LayoutDrawElement(new Location((byte) (placement.Location.X + x), (byte) (placement.Location.Y + y)), LayoutContext.CreateBitmap());
                index++;
            }

            DrawLayout?.Invoke(this, new DrawEventArgs(layoutDrawElements));
        }

        protected void AddElement(Location location, IElement element)
        {
            var placement = new ElementPlacement(location, element);
            _elements.TryAdd(placement, placement);

            if (_inLayout)
                ConnectToElement(element);
        }

        protected void RemoveElement(IElement element)
        {
            var placement = _elements.Values.Single(e => e.Element == element);

            _elements.TryRemove(placement, out _);

            if (_inLayout)
                DisconnectFromElement(placement);
        }

        protected void RemoveElement(Location location)
        {
            var placement = _elements.Values.Single(e => e.Location == location);

            _elements.TryRemove(placement, out _);

            if (_inLayout)
                DisconnectFromElement(placement);
        }


        private void ElementOnDrawElement(object sender, DrawEventArgs e)
        {
            var placedElement = _elements.Values.FirstOrDefault(el=> el.Element == sender);
            if (placedElement.Element != null)
                DrawLayout?.Invoke(this, new DrawEventArgs(e.Elements.Select(el => new LayoutDrawElement(placedElement.Location + el.Location, el.Bitmap)).ToArray()));
        }

        public virtual void ButtonPressed(Location location, bool isDown)
        {
            var elementsToLook = _elements.Values.ToArray();

            foreach (ElementPlacement placement in elementsToLook)
            {
                if (location.X >= placement.Location.X && location.X < placement.Location.X + placement.Element.ButtonCount.Width && location.Y >= placement.Location.Y && location.Y < placement.Location.Y + placement.Element.ButtonCount.Height)
                    placement.Element.ButtonPressed(new Location((byte) (location.X - placement.Location.X), (byte) (location.Y - placement.Location.Y)), isDown);
            }
        }

        public virtual void InitContext(GlobalContext context)
        {
            _globalContext = context;
        }

        public virtual void Init()
        {

        }

        protected void RegisterTimer(TimeSpan interval, Action action)
        {
            var timer = GlobalContext.Services.TimerService.RegisterTimer(interval, action);
            _timers.TryAdd(timer, timer);
        }
    }
}
