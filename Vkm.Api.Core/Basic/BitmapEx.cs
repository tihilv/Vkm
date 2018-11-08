using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Vkm.Api.Basic
{
    public class BitmapEx: IDisposable
    {
        private static readonly ConcurrentDictionary<Tuple<int, int, PixelFormat>, ConcurrentStack<Bitmap>> _bitmapPool = new ConcurrentDictionary<Tuple<int, int, PixelFormat>, ConcurrentStack<Bitmap>>();

        private readonly int _width;
        private readonly int _height;
        private readonly PixelFormat _pixelFormat;

        private readonly Bitmap _internal;

        private bool _disposed;

        public BitmapEx(int width, int height, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
        {
            _internal = GetNewBitmap(width, height, pixelFormat);
            _width = _internal.Width;
            _height = _internal.Height;
            _pixelFormat = _internal.PixelFormat;
        }

        public int Width => _width;
        public int Height => _height;
        public PixelFormat PixelFormat => _pixelFormat;

        private static Bitmap GetNewBitmap(int width, int height, PixelFormat pixelFormat)
        {
            var key = Tuple.Create(width, height, pixelFormat);

            var pool = _bitmapPool.GetOrAdd(key, i => new ConcurrentStack<Bitmap>());

            if (pool.TryPop(out var v))
                return v;

            
            return new Bitmap(width, height, pixelFormat);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                var key = Tuple.Create(Width, Height, PixelFormat);
                _bitmapPool[key].Push(_internal);
            }
            else
            {
                Debug.Assert(false, "Double dispose of Bitmap representation");
            }
        }

        public BitmapData LockBits(Rectangle rect, ImageLockMode readWrite, PixelFormat pixelFormat)
        {
            return _internal.LockBits(rect, readWrite, pixelFormat);
        }

        public void UnlockBits(BitmapData data)
        {
            _internal.UnlockBits(data);
        }

        public Graphics CreateGraphics()
        {
            return Graphics.FromImage(_internal);
        }

        public void Save(Stream stream, ImageFormat imageFormat)
        {
            _internal.Save(stream, imageFormat);
        }

        public Bitmap GetInternal()
        {
            return _internal;
        }

        public void MakeTransparent()
        {
            _internal.MakeTransparent();
            
            using (Graphics g = CreateGraphics()) {
                g.Clip = new Region(new Rectangle(0, 0, Width, Height));
                g.Clear(Color.FromArgb(0, Color.White));
            }
        }

        public BitmapEx Clone()
        {
            using (var rep = new BitmapRepresentation(this))
                return rep.CreateBitmap();
        }
    }
}
