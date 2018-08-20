using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace Vkm.Api.Basic
{
    public class BitmapEx: IDisposable
    {
        private static readonly ConcurrentDictionary<Tuple<int, int, PixelFormat>, ConcurrentStack<Bitmap>> _bitmapPool = new ConcurrentDictionary<Tuple<int, int, PixelFormat>, ConcurrentStack<Bitmap>>();

        private readonly Bitmap _internal;

        private bool _disposed;

        public BitmapEx(int width, int height, PixelFormat pixelFormat)
        {
            _internal = GetNewBitmap(width, height, pixelFormat);
        }

        public int Width => _internal.Width;
        public int Height => _internal.Height;
        public PixelFormat PixelFormat => _internal.PixelFormat;

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

                var key = Tuple.Create(_internal.Width, _internal.Height, _internal.PixelFormat);
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
        }
    }
}
