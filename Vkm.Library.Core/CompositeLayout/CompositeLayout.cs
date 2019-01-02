using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;

namespace Vkm.Library.CompositeLayout
{
    internal class CompositeLayout: LayoutBase, IOptionsProvider
    {
        private CompositeLayoutOptions _compositeLayoutOptions;

        private readonly ConcurrentDictionary<CompositeLayoutElementInfo, Lazy<IElement>> _initedElements;

        readonly object _layoutSwitchLock = new object();

        public CompositeLayout(Identifier identifier) : base(identifier)
        {
            _initedElements = new ConcurrentDictionary<CompositeLayoutElementInfo, Lazy<IElement>>();
        }

        public IOptions GetDefaultOptions()
        {
            return new CompositeLayoutOptions();
        }

        public void InitOptions(IOptions options)
        {
            _compositeLayoutOptions = (CompositeLayoutOptions)options;
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            lock (_layoutSwitchLock)
            {
                Debug.Assert(!base.Elements.Any(), "Has elements before initialization");

                base.EnterLayout(layoutContext, previousLayout);

                _compositeLayoutOptions.CompositeLayoutElementInfos.CollectionChanged += CompositeLayoutElementInfosOnCollectionChanged;

                Parallel.ForEach(_compositeLayoutOptions.CompositeLayoutElementInfos, PlaceElement);

                Debug.Assert(base.Elements.Count() == _compositeLayoutOptions.CompositeLayoutElementInfos.Count, "Wrong number of elements after initialization");
            }
        }

        public override void LeaveLayout()
        {
            lock (_layoutSwitchLock)
            {
                Debug.Assert(base.Elements.Count() == _compositeLayoutOptions.CompositeLayoutElementInfos.Count, "Wrong number of elements before leaving");

                _compositeLayoutOptions.CompositeLayoutElementInfos.CollectionChanged -= CompositeLayoutElementInfosOnCollectionChanged;

                foreach (var item in _compositeLayoutOptions.CompositeLayoutElementInfos)
                    DeleteElement(item, false);

                base.LeaveLayout();

                Debug.Assert(!base.Elements.Any(), "Has elements after leaving");
            }
        }

        private void CompositeLayoutElementInfosOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var newItems = e.NewItems.OfType<CompositeLayoutElementInfo>();
            var oldItems = e.OldItems.OfType<CompositeLayoutElementInfo>();

            switch (e.Action)
            {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in newItems)
                            PlaceElement(item);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (var item in oldItems)
                            DeleteElement(item, true);
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        foreach (var item in oldItems)
                            DeleteElement(item, true);
                        foreach (var item in newItems)
                            PlaceElement(item);
                        break;
            }
        }

        private void PlaceElement(CompositeLayoutElementInfo elementInfo)
        {
            AddElement(elementInfo.Location, _initedElements.GetOrAdd(elementInfo, info =>  new Lazy<IElement>(()=>GlobalContext.CreateElement(info.ModuleInfo))).Value);
        }

        private void DeleteElement(CompositeLayoutElementInfo elementInfo, bool forever)
        {
            RemoveElement(elementInfo.Location);
            if (forever)
            {
                _initedElements.TryRemove(elementInfo, out _);
            }
        }

    }
}
