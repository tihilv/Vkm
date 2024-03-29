﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Time;

namespace Vkm.Api.Element
{
    public abstract class ElementBase: IElement, IInitializable
    {
        private LayoutContext _layoutContext;
        private GlobalContext _globalContext;

        private readonly List<ITimerToken> _timers;

        public Identifier Id { get; private set; }
        public abstract DeviceSize ButtonCount { get; }

        protected LayoutContext LayoutContext => _layoutContext;

        protected GlobalContext GlobalContext => _globalContext;

        public event EventHandler<DrawEventArgs> DrawRequested;

        protected ElementBase(Identifier identifier)
        {
            Id = identifier;
            
            _timers = new List<ITimerToken>();
        }

        protected virtual void OnEnteringLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            
        }

        protected virtual void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            
        }
        
        public void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            OnEnteringLayout(layoutContext, previousLayout);
            
            _layoutContext = layoutContext;
            foreach (var timer in _timers)
                timer.Start();
            
            OnEnteredLayout(layoutContext, previousLayout);
        }

        protected virtual void OnLeavingLayout()
        {
            
        }

        protected virtual void OnLeavedLayout()
        {
            
        }
        
        public void LeaveLayout()
        {
            OnLeavingLayout();
            
            foreach (var timer in _timers)
                timer.Stop();
            
            OnLeavedLayout();
        }

        public virtual void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            
        }

        protected void DrawInvoke(IEnumerable<LayoutDrawElement> drawElements)
        {
            DrawRequested?.Invoke(this, new DrawEventArgs(drawElements.ToArray()));
        }

        protected void RegisterTimer(TimeSpan interval, Action action)
        {
            _timers.Add(GlobalContext.Services.TimerService.RegisterTimer(interval, action));
        }

        public void InitContext(GlobalContext context)
        {
            _globalContext = context;
        }

        public virtual void Init()
        {
            
        }
    }
}
