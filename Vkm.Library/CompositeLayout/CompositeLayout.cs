using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Module;
using Vkm.Api.Options;
using Vkm.Common;
using Vkm.Library.Clock;
using Vkm.Library.Heartbeat;
using Vkm.Library.Mail;
using Vkm.Library.Run;
using Vkm.Library.Volume;
using Vkm.Library.Weather;

namespace Vkm.Library.CompositeLayout
{
    internal class CompositeLayout: LayoutBase, IOptionsProvider
    {
        private static readonly Identifier ClockIdentifier = new Identifier("Vkm.DesktopDefaults.Clock");
        private static readonly Identifier VolumeIdentifier = new Identifier("Vkm.DesktopDefaults.Volume");
        private static readonly Identifier WeatherIdentifier = new Identifier("Vkm.DesktopDefaults.Weather");
        private static readonly Identifier MailIdentifier = new Identifier("Vkm.DesktopDefaults.Mail");
        public static readonly Identifier CalcIdentifier = new Identifier("Vkm.DesktopDefaults.Calc");
        public static readonly Identifier HeartbeatIdentifier = new Identifier("Vkm.DesktopDefaults.Heartbeat");

        private CompositeLayoutOptions _compositeLayoutOptions;

        private readonly ConcurrentDictionary<CompositeLayoutElementInfo, IElement> _initedElements;

        readonly object _layoutSwitchLock = new object();

        public CompositeLayout(Identifier identifier) : base(identifier)
        {
            _initedElements = new ConcurrentDictionary<CompositeLayoutElementInfo, IElement>();
        }

        public IOptions GetDefaultOptions()
        {
            var result = new CompositeLayoutOptions();

            if (Id == Identifiers.DefaultCompositeLayout) // desktop
            {
                result.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
                {
                    Location = new Location(0, 0),
                    ModuleInfo = new ModuleInitializationInfo(ClockElementFactory.Identifier, ClockIdentifier)
                });

                result.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
                {
                    Location = new Location(4, 0),
                    ModuleInfo = new ModuleInitializationInfo(VolumeElementFactory.Identifier, VolumeIdentifier)
                });
                result.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
                {
                    Location = new Location(3, 2),
                    ModuleInfo = new ModuleInitializationInfo(WeatherElementFactory.Identifier, WeatherIdentifier)
                });

                result.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
                {
                    Location = new Location(2, 2),
                    ModuleInfo = new ModuleInitializationInfo(MailElementFactory.Identifier, MailIdentifier)
                });

                result.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
                {
                    Location = new Location(0, 2),
                    ModuleInfo = new ModuleInitializationInfo(RunElementFactory.Identifier, CalcIdentifier)
                });

                result.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
                {
                    Location = new Location(4, 2),
                    ModuleInfo = new ModuleInitializationInfo(HeartbeatFactory.Identifier, HeartbeatIdentifier)
                });
            }
            
            return result;
        }

        public void InitOptions(IOptions options)
        {
            _compositeLayoutOptions = (CompositeLayoutOptions)options;
        }

        public override void EnterLayout(LayoutContext layoutContext)
        {
            lock (_layoutSwitchLock)
            {
                base.EnterLayout(layoutContext);

                _compositeLayoutOptions.CompositeLayoutElementInfos.CollectionChanged += CompositeLayoutElementInfosOnCollectionChanged;

                Parallel.ForEach(_compositeLayoutOptions.CompositeLayoutElementInfos, PlaceElement);
            }
        }

        public override void LeaveLayout()
        {
            lock (_layoutSwitchLock)
            {
                _compositeLayoutOptions.CompositeLayoutElementInfos.CollectionChanged -= CompositeLayoutElementInfosOnCollectionChanged;

                foreach (var item in _compositeLayoutOptions.CompositeLayoutElementInfos)
                    DeleteElement(item, false);

                base.LeaveLayout();
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
            AddElement(elementInfo.Location,  
                _initedElements.GetOrAdd(elementInfo, info => GlobalContext.CreateElement(info.ModuleInfo)));
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
