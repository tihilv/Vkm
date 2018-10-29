using System;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Layout;
using Vkm.TestProject.Entities;

namespace Vkm.TestProject
{
    [TestClass]
    public class BitmapTests
    {
        [TestMethod]
        public void TestBitmapAllocations()
        {
            Bitmap bmp1;
            using (Bitmap srcBmp = new Bitmap(100, 100))
            using (BitmapRepresentation br = new BitmapRepresentation(srcBmp))
            using (var bmpEx = br.CreateBitmap())
            {
                bmp1 = bmpEx.GetInternal();
            }

            Bitmap bmp2;
            using (Bitmap srcBmp = new Bitmap(101, 100))
            using (BitmapRepresentation br = new BitmapRepresentation(srcBmp))
            using (var bmpEx = br.CreateBitmap())
            {
                bmp2 = bmpEx.GetInternal();
            }

            Bitmap bmp3;
            Bitmap bmp4;
            using (Bitmap srcBmp = new Bitmap(100, 100))
            using (BitmapRepresentation br = new BitmapRepresentation(srcBmp))
            {
                using (var bmpEx1 = br.CreateBitmap())
                using (var bmpEx2 = br.CreateBitmap())
                {
                    bmp3 = bmpEx1.GetInternal();
                    bmp4 = bmpEx2.GetInternal();
                }
            }
            Assert.AreEqual(bmp1, bmp3, "Instances of bitmaps of the same size are different");
            Assert.AreNotEqual(bmp1, bmp2, "Instances of bitmaps of different size are the same");
            Assert.AreNotEqual(bmp3, bmp4, "Not disposed image is used twice");
        }

        [TestMethod]
        public void TestBitmapResize()
        {
            Color green = Color.FromArgb(0,200,0);
            Color red = Color.FromArgb(200,0,0);
            using (var bitmap = new Bitmap(20, 20))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                using (var gBrush = new SolidBrush(green))
                using (var rBrush = new SolidBrush(red))
                {
                    graphics.FillRectangle(gBrush, 0, 0, 10, 20);
                    graphics.FillRectangle(rBrush, 10, 0, 10, 20);
                }

                using (var sourceRepresentation = new BitmapRepresentation(bitmap))
                    using (var sourceBitmapEx = sourceRepresentation.CreateBitmap())
                    using (var destinationBitmapEx = new BitmapEx(10, 10))
                {
                    BitmapHelpers.ResizeBitmap(sourceBitmapEx, destinationBitmapEx);
                    var bmp = destinationBitmapEx.GetInternal();
                    {
                        var green2 = bmp.GetPixel(3, 5);
                        var red2 = bmp.GetPixel(7, 5);
                        Assert.AreEqual(true, CompareColors(green, green2, 5));
                        Assert.AreEqual(true, CompareColors(red, red2, 5));
                    }
                }
            }
        }

        [TestMethod]
        public void TestExtractLayoutDrawElements()
        {
            Color green = Color.FromArgb(0, 200, 0);
            Color red = Color.FromArgb(200, 0, 0);
            Color blue = Color.FromArgb(0, 0, 200);

            Color black = Color.FromArgb(0, 0, 0);
            Color gray = Color.FromArgb(100, 100, 100);
            Color white = Color.FromArgb(200, 200, 200);
            
            using (var bitmap = new Bitmap(30, 20))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                using (var rBrush = new SolidBrush(red))
                using (var gBrush = new SolidBrush(green))
                using (var bBrush = new SolidBrush(blue))
                using (var bkBrush = new SolidBrush(black))
                using (var grBrush = new SolidBrush(gray))
                using (var wtBrush = new SolidBrush(white))
                {
                    graphics.FillRectangle(rBrush, 0, 0, 10, 10);
                    graphics.FillRectangle(gBrush, 10, 0, 10, 10);
                    graphics.FillRectangle(bBrush, 20, 0, 10, 10);
                    graphics.FillRectangle(bkBrush, 0, 10, 10, 10);
                    graphics.FillRectangle(grBrush, 10, 10, 10, 10);
                    graphics.FillRectangle(wtBrush, 20, 10, 10, 10);
                }

                var initializer = new Initializer();
                
                using (var sourceRepresentation = new BitmapRepresentation(bitmap))
                using (var sourceBitmapEx = sourceRepresentation.CreateBitmap())
                {
                    var ldes = BitmapHelpers.ExtractLayoutDrawElements(sourceBitmapEx, new DeviceSize(3, 2), 5, 3, initializer.LayoutContext).ToArray();
                    
                    Assert.AreEqual(true, CheckColorOfElement(ldes, 5, 3, red));
                    Assert.AreEqual(true, CheckColorOfElement(ldes, 6, 3, green));
                    Assert.AreEqual(true, CheckColorOfElement(ldes, 7, 3, blue));
                    Assert.AreEqual(true, CheckColorOfElement(ldes, 5, 4, black));
                    Assert.AreEqual(true, CheckColorOfElement(ldes, 6, 4, gray));
                    Assert.AreEqual(true, CheckColorOfElement(ldes, 7, 4, white));
                }
            }
        }

        private bool CheckColorOfElement(LayoutDrawElement[] ldes, int x, int y, Color color)
        {
            var element = ldes.First(e => e.Location == new Location(x, y));
            using (var bitmap = element.BitmapRepresentation.CreateBitmap())
            {
                var bmp = bitmap.GetInternal();
                return CompareColors(bmp.GetPixel(4, 4), color, 5);
            }
        }


        private static bool CompareColors(Color c1, Color c2, byte delta)
        {
            return (Math.Abs(c1.R - c2.R) <= delta) && (Math.Abs(c1.G - c2.G) <= delta) && (Math.Abs(c1.B - c2.B) <= delta);
        }
    }
}