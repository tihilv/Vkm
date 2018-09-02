using System;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Transition;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service.Player;

namespace Vkm.Library.Media
{
    internal class MediaElement: ElementBase
    {
        private IPlayerService[] _playerServices;

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public MediaElement(Identifier id) : base(id)
        {
            
        }

        public override void Init()
        {
            base.Init();
            _playerServices = GlobalContext.GetServices<IPlayerService>().ToArray();
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            DrawNow();

            foreach (IPlayerService playerService in _playerServices)
                playerService.PlayingInfoChanged += PlayerServiceOnPlayingInfoChanged;
        }

        async void DrawNow()
        {
            var playingNow = _playerServices.Select(async s => await s.GetCurrent()).FirstOrDefault(c => c != null);
            if (playingNow != null)
                PerformDraw(await playingNow);
        }

        private void PlayerServiceOnPlayingInfoChanged(object sender, PlayingEventArgs e)
        {
            PerformDraw(e.PlayingInfo);
        }

        private BitmapRepresentation _previousRepresentation;
        private bool _prevPlaying;

        private void PerformDraw(PlayingInfo playingInfo)
        {
            if (_previousRepresentation != playingInfo?.BitmapRepresentation || _prevPlaying != playingInfo?.IsPlaying)
            {
                _previousRepresentation = playingInfo?.BitmapRepresentation;
                _prevPlaying = playingInfo?.IsPlaying ?? false;
                
                var bitmap = LayoutContext.CreateBitmap();

                if (playingInfo?.BitmapRepresentation != null)
                {
                    using (var coverBitmap = playingInfo.BitmapRepresentation.CreateBitmap())
                    {
                        BitmapHelpers.ResizeBitmap(coverBitmap, bitmap, _prevPlaying ? null : BitmapHelpers.GrayColorMatrix);
                    }
                }

                DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), bitmap, new TransitionInfo(TransitionType.ElementUpdate, TimeSpan.FromSeconds(1)))});
            }
        }

        public override void LeaveLayout()
        {
            foreach (IPlayerService playerService in _playerServices)
                playerService.PlayingInfoChanged -= PlayerServiceOnPlayingInfoChanged;

            _previousRepresentation = null;
            
            base.LeaveLayout();
        }

        public override bool ButtonPressed(Location location, bool isDown)
        {
            if (isDown)
            {
                var mediaLayout = new MediaLayout(new Identifier(Id.Value + ".Layout"));
                GlobalContext.InitializeEntity(mediaLayout);

                LayoutContext.SetLayout(mediaLayout);
            }
            return base.ButtonPressed(location, isDown);
        }

    }
}
