using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Rectangle = System.Drawing.Rectangle;

namespace Vkm.Api.Basic
{
    [Serializable]
    public class BitmapRepresentation : IDisposable
    {
        private static readonly ConcurrentDictionary<int, ConcurrentStack<byte[]>> _pool = new ConcurrentDictionary<int, ConcurrentStack<byte[]>>();

        private readonly byte[] _bitmapInternal;

        private readonly PixelFormat _pixelFormat;
        private readonly ushort _width;
        private readonly ushort _height;

        private bool _disposed;

        public int Width => _width;

        public int Height => _height;

        public byte[] Bytes => _bitmapInternal;

        private BitmapRepresentation(byte[] bitmapInternal, ushort width, ushort height, PixelFormat pixelFormat)
        {
            _bitmapInternal = bitmapInternal;
            _pixelFormat = pixelFormat;
            _width = width;
            _height = height;
        }

        public BitmapRepresentation(BitmapEx bitmap) : this(bitmap?.GetInternal())
        {

        }

        public BitmapRepresentation(Bitmap bitmap)
        {
            _bitmapInternal = Array1DFromBitmap(bitmap);

            _width = (ushort)bitmap.Width;
            _height = (ushort)bitmap.Height;
            _pixelFormat = bitmap.PixelFormat;
        }

        public BitmapRepresentation(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var bitmap = (Bitmap) Bitmap.FromStream(stream))
            {
                _bitmapInternal = Array1DFromBitmap(bitmap);

                _width = (ushort)bitmap.Width;
                _height = (ushort)bitmap.Height;
                _pixelFormat = bitmap.PixelFormat;
            }
        }


        public BitmapRepresentation Clone()
        {
            var array = GetNewArray(_bitmapInternal.Length);
            Array.Copy(_bitmapInternal, array, _bitmapInternal.Length);
            return new BitmapRepresentation(array, _width, _height, _pixelFormat);
        }

        private static byte[] GetNewArray(int length)
        {
            var pool = _pool.GetOrAdd(length, i => new ConcurrentStack<byte[]>());

            if (pool.TryPop(out var v))
                return v;


            return new byte[length];
        }

        public BitmapEx CreateBitmap()
        {
            BitmapEx bitmap = new BitmapEx(_width, _height, _pixelFormat);
            Rectangle grayRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData grayData = bitmap.LockBits(grayRect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr grayPtr = grayData.Scan0;

            int grayBytes = grayData.Stride * bitmap.Height;

            System.Runtime.InteropServices.Marshal.Copy(_bitmapInternal, 0, grayPtr, grayBytes);

            bitmap.UnlockBits(grayData);
            return bitmap;
        }

        private static byte[] Array1DFromBitmap(Bitmap bmp)
        {
            if (bmp == null) throw new NullReferenceException("Bitmap is null");

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData data = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = data.Scan0;

            //declare an array to hold the bytes of the bitmap
            int numBytes = data.Stride * bmp.Height;
            byte[] bytes = GetNewArray(numBytes);

            //copy the RGB values into the array
            System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, numBytes);

            bmp.UnlockBits(data);

            return bytes;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _pool[_bitmapInternal.Length].Push(_bitmapInternal);
            }
            else
            {
                Debug.Assert(false, "Double dispose of Bitmap representation");
            }
        }
    }
}