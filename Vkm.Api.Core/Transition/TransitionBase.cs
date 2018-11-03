using System;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Options;

namespace Vkm.Api.Transition
{
    public abstract class TransitionBase<T> : ITransition, IOptionsProvider where T: TransitionOptions
    {
        private T _transitionOptions;

        private GlobalContext _globalContext;

        public Identifier Id { get; private set; }

        protected T TransitionOptions => _transitionOptions;

        protected GlobalContext GlobalContext => _globalContext;

        public event EventHandler<TransitionEventArgs> PerformTransition;

        protected TransitionBase(Identifier id)
        {
            Id = id;
        }

        public abstract IOptions GetDefaultOptions();

        public virtual void InitOptions(IOptions options)
        {
            _transitionOptions = (T) options;
        }

        protected void OnTransition()
        {
            PerformTransition?.Invoke(this, new TransitionEventArgs(_transitionOptions.DeviceId, _transitionOptions.LayoutId));
        }

        protected void OnTransitionBack()
        {
            PerformTransition?.Invoke(this, new TransitionEventArgs(_transitionOptions.DeviceId, _transitionOptions.LayoutId, true));
        }

        public virtual void InitContext(GlobalContext context)
        {
            _globalContext = context;
        }

        public abstract void Init();

        protected void RegisterTimer(TimeSpan interval, Action action)
        {
            GlobalContext.Services.TimerService.RegisterTimer(interval, action).Start();
        }
    }
}