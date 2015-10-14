using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ObjRec.Core.Model;

namespace ObjRec.Core.Filters.Sobel
{
    public class SobelFilter : IFilter
    {
        public Task<Image> Apply(Image image)
        {
           var task =  Task.Run(() =>
            {
                var grayscale = true;
                var sourceBitmap = new Bitmap(image);
                BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
                byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

                //copy from source bitmap to pixel buffer
                Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
                sourceBitmap.UnlockBits(sourceData);

                if (grayscale == true)
                {
                    float rgb = 0;

                    for (int k = 0; k < pixelBuffer.Length; k += 4)
                    {
                        rgb = pixelBuffer[k] * 0.11f;
                        rgb += pixelBuffer[k + 1] * 0.59f;
                        rgb += pixelBuffer[k + 2] * 0.3f;

                        pixelBuffer[k] = (byte)rgb;
                        pixelBuffer[k + 1] = pixelBuffer[k];
                        pixelBuffer[k + 2] = pixelBuffer[k];
                        pixelBuffer[k + 3] = 255;
                    }
                }

                var pixelX = new Pixel();
                var pixelY = new Pixel();
                var pixelTotal = new Pixel();

                int filterOffset = 1;
                int calcOffset = 0;

                int byteOffset = 0;

                for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
                {
                    for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                    {
                        pixelX.Clear();
                        pixelY.Clear();

                        // mult width offser becaous ARGB
                        byteOffset = offsetY * sourceData.Stride + offsetX * 4;

                        for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                        {
                            for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                            {
                                calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);

                                pixelX.Blue += (double)(pixelBuffer[calcOffset]) * SobelMatrix.Horizontal[filterY + filterOffset, filterX + filterOffset];

                                pixelX.Green += (double)(pixelBuffer[calcOffset + 1]) * SobelMatrix.Horizontal[filterY + filterOffset, filterX + filterOffset];

                                pixelX.Red += (double)(pixelBuffer[calcOffset + 2]) * SobelMatrix.Horizontal[filterY + filterOffset, filterX + filterOffset];

                                pixelY.Blue += (double)(pixelBuffer[calcOffset]) * SobelMatrix.Vertical[filterY + filterOffset, filterX + filterOffset];

                                pixelY.Green += (double)(pixelBuffer[calcOffset + 1]) * SobelMatrix.Vertical[filterY + filterOffset, filterX + filterOffset];

                                pixelY.Red += (double)(pixelBuffer[calcOffset + 2]) * SobelMatrix.Vertical[filterY + filterOffset, filterX + filterOffset];
                            }
                        }

                        pixelTotal.Blue = Math.Sqrt(pixelX.Blue * pixelX.Blue + pixelY.Blue * pixelY.Blue);
                        pixelTotal.Green = Math.Sqrt(pixelX.Green * pixelX.Green + pixelY.Green * pixelY.Green);
                        pixelTotal.Red = Math.Sqrt(pixelX.Red * pixelX.Red + pixelY.Red * pixelY.Red);

                        pixelTotal.CorrectColor();

                        resultBuffer[byteOffset] = (byte)(pixelTotal.Blue);
                        resultBuffer[byteOffset + 1] = (byte)(pixelTotal.Green);
                        resultBuffer[byteOffset + 2] = (byte)(pixelTotal.Red);
                        resultBuffer[byteOffset + 3] = 255;
                    }
                }

                Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

                BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
                resultBitmap.UnlockBits(resultData);

                return (Image)resultBitmap;
            });
            return task;
        }
    }
}
