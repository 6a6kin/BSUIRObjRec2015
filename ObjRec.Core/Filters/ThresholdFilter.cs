using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using ObjRec.Core.Model;

namespace ObjRec.Core.Filters
{
    public class ThresholdFilter : IFilter
    {
        public int Threshold { get; set; } = 0;

        public virtual Task<Image> Apply(Image image)
        {
            return Task.Run(() =>
            {
                var srcBitmap = (Bitmap)image;
                var resultBitmap = new Bitmap(image);

                for (int x = 0; x < srcBitmap.Width; x++)
                {
                    for (int y = 0; y < srcBitmap.Height; y++)
                    {
                        var pixel = new Pixel(srcBitmap.GetPixel(x, y));

                        int insensity = pixel.Intensity;

                        var color = insensity > Threshold ? Color.White : Color.Black;

                        resultBitmap.SetPixel(x, y, color);
                    }
                }

                return (Image)resultBitmap;
            });
        }
    }
}
