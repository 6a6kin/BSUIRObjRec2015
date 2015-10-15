using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ObjRec.Core.Model;

namespace ObjRec.Core.Algorithms
{
    public static class ThresholdOtsuAlgorithm
    {
        public static Task<int> ComputeThreshold(Image image)
        {
            return Task.Run(() =>
            {
                var hist = new int[256];
                var bitmap = (Bitmap) image;
                double n = bitmap.Width*bitmap.Height;

                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        var pixel = new Pixel(bitmap.GetPixel(x, y));
                        hist[pixel.Intensity]++;
                    }
                }

                double P1 = 0.0d;
                double P2 = 0.0d;
                double mu1;
                double mu2;
                double ip = 0.0d;
                double sumip = hist.Select((v, t) => t*v/n).Sum();
                
                double maxSigma = 0.0d;
                int maxT = 0;
                
                for (int t = 0; t < 256; t++)
                {
                    P1 += hist[t]/n;
                    P2 = 1 - P1;

                    ip += t* hist[t] / n;

                    mu1 = ip/P1;
                    mu2 = (sumip - ip)/P2;

                    var sigma = (int) (P1*P2*(mu1 - mu2)*(mu1 - mu2));
                    if (maxSigma < sigma)
                    {
                        maxSigma = sigma;
                        maxT = t;
                    }
                }

                return maxT;
            });
        }
    }
}
