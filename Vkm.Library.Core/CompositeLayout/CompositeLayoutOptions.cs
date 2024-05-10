using System;
using System.Collections.ObjectModel;
using Vkm.Api.Options;
using Vkm.Common;

namespace Vkm.Library.CompositeLayout
{
    [Serializable]
    public class CompositeLayoutOptions : IOptions
    {
        private readonly ObservableCollectionEx<CompositeLayoutElementInfo> _compositeLayoutElementInfos;

        public ObservableCollectionEx<CompositeLayoutElementInfo> CompositeLayoutElementInfos => _compositeLayoutElementInfos;

        public CompositeLayoutOptions()
        {
            _compositeLayoutElementInfos = new ObservableCollectionEx<CompositeLayoutElementInfo>();
        }
    }
}