using System;
using System.Collections.Generic;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service.Player;

namespace Vkm.Library.Media
{
    class MediaLayout: ILayout, IInitializable
    {
        private GlobalContext _globalContext;
        private LayoutContext _layoutContext;

        private IPlayerService[] _playerServices;

        public Identifier Id { get; }
        
        public byte? PreferredBrightness => null;

        public event EventHandler<DrawEventArgs> DrawLayout;
        
        public void InitContext(GlobalContext context)
        {
            _globalContext = context;
        }

        public void Init()
        {
            _playerServices = _globalContext.GetServices<IPlayerService>().ToArray();
        }
        
        public void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            _layoutContext = layoutContext;

            DrawNow();

            foreach (IPlayerService playerService in _playerServices)
                playerService.PlayingInfoChanged += PlayerServiceOnPlayingInfoChanged;
        }

        private void PlayerServiceOnPlayingInfoChanged(object sender, PlayingEventArgs e)
        {
            PerformDraw(e.PlayingInfo);
        }

        async void DrawNow()
        {
            var playingNow = _playerServices.Select(async s => await s.GetCurrent()).FirstOrDefault(c => c != null);
            if (playingNow != null)
                PerformDraw(await playingNow);
        }

        private BitmapRepresentation _previousRepresentation;
        private void PerformDraw(PlayingInfo playingInfo)
        {
            byte imageSize = 2;
            byte textSize = 3;
            var result = new List<LayoutDrawElement>();

            if (_previousRepresentation != playingInfo.BitmapRepresentation)
            {
                _previousRepresentation = playingInfo.BitmapRepresentation;
                
                using (var bitmap = new BitmapEx(_layoutContext.IconSize.Width * imageSize, _layoutContext.IconSize.Height * imageSize))
                {
                    bitmap.MakeTransparent();
                    if (playingInfo?.BitmapRepresentation != null)
                        using (BitmapEx coverBitmap = playingInfo.BitmapRepresentation.CreateBitmap())
                        {
                            BitmapHelpers.ResizeBitmap(coverBitmap, bitmap);
                        }

                    result.AddRange(BitmapHelpers.ExtractLayoutDrawElements(bitmap, new DeviceSize(imageSize, imageSize), 0, 0, _layoutContext));
                }
            }

            using (var bitmap = new BitmapEx(_layoutContext.IconSize.Width * textSize, _layoutContext.IconSize.Height))
            {
                bitmap.MakeTransparent();
                DefaultDrawingAlgs.DrawText(bitmap, _globalContext.Options.Theme.FontFamily, playingInfo.Title, playingInfo.Title, _globalContext.Options.Theme.ForegroundColor);
                result.AddRange(BitmapHelpers.ExtractLayoutDrawElements(bitmap, new DeviceSize(textSize, 1), imageSize, 0, _layoutContext));
            }
            
            using (var bitmap = new BitmapEx(_layoutContext.IconSize.Width * textSize, _layoutContext.IconSize.Height))
            {
                bitmap.MakeTransparent();
                var maxStr = (playingInfo.Artist.Length > playingInfo.Album.Length)?playingInfo.Artist: playingInfo.Album;
                DefaultDrawingAlgs.DrawTexts(bitmap, _globalContext.Options.Theme.FontFamily, playingInfo.Artist, playingInfo.Album, maxStr, _globalContext.Options.Theme.ForegroundColor);
                result.AddRange(BitmapHelpers.ExtractLayoutDrawElements(bitmap, new DeviceSize(textSize, 1), imageSize, 1, _layoutContext));
            }
            
            DrawLayout?.Invoke(this, new DrawEventArgs(result.ToArray()));
        }

        public void LeaveLayout()
        {
            foreach (IPlayerService playerService in _playerServices)
                playerService.PlayingInfoChanged -= PlayerServiceOnPlayingInfoChanged;
        }

        public void ButtonPressed(Location location, bool isDown)
        {
            if (isDown)
                _layoutContext.SetPreviousLayout();
        }
    }

}
