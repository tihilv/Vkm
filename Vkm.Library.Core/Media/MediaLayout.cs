using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Common;
using Vkm.Library.Buttons;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service;
using Vkm.Library.Interfaces.Service.Player;

namespace Vkm.Library.Media
{
    class MediaLayout: LayoutBase
    {
        private IPlayerService[] _playerServices;

        private ButtonElement _playPauseButton;
        
        public MediaLayout(Identifier identifier) : base(identifier)
        {
        }
        
        public override void Init()
        {
            base.Init();

            var keyboardService = GlobalContext.GetServices<IKeyboardService>().First();

            _playerServices = GlobalContext.GetServices<IPlayerService>().ToArray();
            
            _playPauseButton = GlobalContext.InitializeEntity(new ButtonElement(FontAwesomeRes.fa_play, FontService.Instance.AwesomeFontFamily, keyboardService.PlayPause));
            
            AddElement(new Location(1, 2), GlobalContext.InitializeEntity(new ButtonElement(FontAwesomeRes.fa_backward, FontService.Instance.AwesomeFontFamily, keyboardService.PreviousTrack)));
            AddElement(new Location(2, 2), _playPauseButton);
            AddElement(new Location(3, 2), GlobalContext.InitializeEntity(new ButtonElement(FontAwesomeRes.fa_forward, FontService.Instance.AwesomeFontFamily, keyboardService.NextTrack)));
        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            DrawNow();

            foreach (IPlayerService playerService in _playerServices)
                playerService.PlayingInfoChanged += PlayerServiceOnPlayingInfoChanged;
        }

        protected override void OnLeavedLayout()
        {
            foreach (IPlayerService playerService in _playerServices)
                playerService.PlayingInfoChanged -= PlayerServiceOnPlayingInfoChanged;

            _previousRepresentation = null;
        }

        private void PlayerServiceOnPlayingInfoChanged(object sender, PlayingEventArgs e)
        {
            PerformDraw(e.PlayingInfo);
        }

        async void DrawNow()
        {
            bool drawn = false;
            bool isPlaying = false;
            foreach (var playingNowTask in _playerServices.Select(async s => await s.GetCurrent()))
            {
                if (playingNowTask != null)
                {
                    var playingNow = await playingNowTask;
                    if (playingNow != null)
                    {
                        if (!drawn || !isPlaying)
                        {
                            PerformDraw(playingNow);
                            drawn = true;
                            isPlaying = playingNow.IsPlaying;
                        }

                        playingNow.Dispose();
                    }
                }
            }
        }

        private BitmapRepresentation _previousRepresentation;
        private bool? _isPlaying;
        
        private void PerformDraw(PlayingInfo playingInfo)
        {
            byte imageSize = 2;
            byte textSize = 3;
            var result = new List<LayoutDrawElement>();

            if (playingInfo.IsPlaying != _isPlaying)
            {
                _isPlaying = playingInfo.IsPlaying;
                _playPauseButton.ReplaceText(_isPlaying.Value?FontAwesomeRes.fa_pause:FontAwesomeRes.fa_play);
            }
            
            if (_previousRepresentation != playingInfo?.BitmapRepresentation)
            {
                _previousRepresentation = playingInfo?.BitmapRepresentation;
                
                using (var bitmap = new BitmapEx(LayoutContext.IconSize.Width * imageSize, LayoutContext.IconSize.Height * imageSize))
                {
                    bitmap.MakeTransparent();
                    if (playingInfo?.BitmapRepresentation != null)
                        using (BitmapEx coverBitmap = playingInfo.BitmapRepresentation.CreateBitmap())
                        {
                            BitmapHelpers.ResizeBitmap(coverBitmap, bitmap);
                        }

                    result.AddRange(BitmapHelpers.ExtractLayoutDrawElements(bitmap, new DeviceSize(imageSize, imageSize), 0, 0, LayoutContext));
                }
            }

            using (var bitmap = new BitmapEx(LayoutContext.IconSize.Width * textSize, LayoutContext.IconSize.Height))
            {
                var lineWidth = 0.05;
                
                bitmap.MakeTransparent();
                var title = playingInfo?.Title;
                DefaultDrawingAlgs.DrawText(bitmap, GlobalContext.Options.Theme.FontFamily, title, LayoutContext.Options.Theme.ForegroundColor);

                if (playingInfo != null)
                {
                    using (var graphics = bitmap.CreateGraphics())
                    using (var brush = new SolidBrush(GlobalContext.Options.Theme.ForegroundColor))
                    {
                        var rect = new Rectangle(0, (int)(bitmap.Height * (1 - lineWidth)), (int) (bitmap.Width * playingInfo.CurrentPosition.TotalMilliseconds / playingInfo.DurationSpan.TotalMilliseconds), (int)(bitmap.Height * lineWidth));
                        graphics.FillRectangle(brush, rect);
                    }
                }

                result.AddRange(BitmapHelpers.ExtractLayoutDrawElements(bitmap, new DeviceSize(textSize, 1), imageSize, 0, LayoutContext));
            }
            
            using (var bitmap = new BitmapEx(LayoutContext.IconSize.Width * textSize, LayoutContext.IconSize.Height))
            {
                bitmap.MakeTransparent();
                var artist = playingInfo?.Artist ?? string.Empty;
                var album = playingInfo?.Album ?? string.Empty;
                var text = $"{artist}\n{album}";
                DefaultDrawingAlgs.DrawText(bitmap, GlobalContext.Options.Theme.FontFamily, text, GlobalContext.Options.Theme.ForegroundColor);
                result.AddRange(BitmapHelpers.ExtractLayoutDrawElements(bitmap, new DeviceSize(textSize, 1), imageSize, 1, LayoutContext));
            }
            
            DrawInvoke(result);
        }

        public override void ButtonPressed(Location location, ButtonEvent buttonEvent)
        {
            if (location.Y != 2)
                if (buttonEvent == ButtonEvent.Down)
                    LayoutContext.SetPreviousLayout();
            
            base.ButtonPressed(location, buttonEvent);
        }
    }
}
