using System.Drawing;
using System.Threading.Tasks;

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
                        var pixel = srcBitmap.GetPixel(x, y);

                        int insensity = (int) (0.2126*pixel.R + 0.7152*pixel.G + 0.0722*pixel.B);

                        insensity = insensity > Threshold ? 255 : 0;

                        resultBitmap.SetPixel(x, y, Color.FromArgb(insensity, insensity, insensity));
                    }
                }

                return (Image) resultBitmap;
            });
        }
    }
}
