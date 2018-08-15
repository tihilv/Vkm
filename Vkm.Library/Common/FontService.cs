﻿using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Vkm.Library.Common
{
    internal class FontService
    {
        public static FontService Instance = new FontService();

        private readonly PrivateFontCollection _fontCollection;

        private readonly FontFamily _awesomeFontFamily;

        private readonly ConcurrentDictionary<string, FontFamily> _fontFamilies;

        public FontFamily AwesomeFontFamily => _awesomeFontFamily;

        private FontService()
        {
            _fontCollection = new PrivateFontCollection();
            _fontFamilies = new ConcurrentDictionary<string, FontFamily>();

            _awesomeFontFamily = GetFontFamilyByResourceName("Vkm.Library.Resources.FontAwesome.ttf");
        }

        internal FontFamily GetFontFamilyByResourceName(string resourceName)
        {
            return _fontFamilies.GetOrAdd(resourceName, s =>
            {
                using (Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    IntPtr data = Marshal.AllocCoTaskMem((int) fontStream.Length);
                    byte[] fontdata = new byte[fontStream.Length];
                    fontStream.Read(fontdata, 0, (int) fontStream.Length);
                    Marshal.Copy(fontdata, 0, data, (int) fontStream.Length);
                    _fontCollection.AddMemoryFont(data, (int) fontStream.Length);
                    Marshal.FreeCoTaskMem(data);
                }

                return _fontCollection.Families.Last();
            });
        }
    }
}
