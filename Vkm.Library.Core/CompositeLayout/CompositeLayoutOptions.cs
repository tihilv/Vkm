using System;
using System.Collections.ObjectModel;
using Vkm.Api.Options;

namespace Vkm.Library.CompositeLayout
{
    [Serializable]
    public class CompositeLayoutOptions : IOptions
    {
        private readonly ObservableCollection<CompositeLayoutElementInfo> _compositeLayoutElementInfos;

        public ObservableCollection<CompositeLayoutElementInfo> CompositeLayoutElementInfos => _compositeLayoutElementInfos;

        public CompositeLayoutOptions()
        {
            _compositeLayoutElementInfos = new ObservableCollection<CompositeLayoutElementInfo>();
        }
    }
}