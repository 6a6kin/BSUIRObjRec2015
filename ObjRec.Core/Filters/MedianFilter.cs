using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ObjRec.Core.Model;

namespace ObjRec.Core.Filters
{
    public class MedianFilter : IFilter
    {
        public int WindowSize { get; set; }
        public int WindowHalf => (WindowSize - 1)/2;
        public int WindowCenter => (WindowSize*WindowSize - 1)/2;

        public Task<Image> Apply(Image image)
        {
            return Task.Run(() =>
            {
                var sourceBitmap = (Bitmap)image;
                BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
                byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

                Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
                sourceBitmap.UnlockBits(sourceData);

                var pixelTotal = new Pixel();
                WindowSize = 3;
                int window = WindowSize;
                int winSide = WindowHalf;
                int winCen = WindowCenter;
                byte[] pixelRectR = new byte[window * window];
                byte[] pixelRectG = new byte[window * window];
                byte[] pixelRectB = new byte[window * window];

                for (int height = winSide; height < sourceBitmap.Height- winSide; height++)
                {
                    for (int width = winSide; width < sourceBitmap.Width - winSide; width++)
                    {
                        int i = 0;
                        for (int windowX = width - winSide; windowX <= width+ winSide; windowX++)
                        {
                            for (int windowY = height - winSide; windowY <= height+ winSide; windowY++)
                            {
                                int sIndex = (windowX*4) + (windowY*sourceBitmap.Width*4);
                                pixelRectR[i] = pixelBuffer[sIndex];
                                pixelRectG[i] = pixelBuffer[sIndex + 1];
                                pixelRectB[i] = pixelBuffer[sIndex + 2];
                                i++;
                            }
                        }
                        Array.Sort(pixelRectR);
                        Array.Sort(pixelRectG);
                        Array.Sort(pixelRectB);
                        pixelTotal.Red = pixelRectR[winCen];
                        pixelTotal.Green = pixelRectG[winCen];
                        pixelTotal.Blue = pixelRectB[winCen];

                        int shortIndex = (width*4) + (height*sourceBitmap.Width*4);
                        resultBuffer[shortIndex] = (byte)(pixelTotal.Red);
                        resultBuffer[shortIndex + 1] = (byte)(pixelTotal.Green);
                        resultBuffer[shortIndex + 2] = (byte)(pixelTotal.Blue);
                        resultBuffer[shortIndex + 3] = 255;
                    }

                }

                Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

                BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                
                Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
                resultBitmap.UnlockBits(resultData);
                
                return (Image)resultBitmap;
            });
        }
    }
}
