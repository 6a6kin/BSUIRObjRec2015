using System.Drawing;
using System.Threading.Tasks;
using ObjRec.Core.Algorithms;

namespace ObjRec.Core.Filters
{
    public class ThresholdOtsuFilter : ThresholdFilter
    {
        public override Task<Image> Apply(Image image)
        {
            Threshold = ThresholdOtsuAlgorithm.ComputeThreshold(image);

            return base.Apply(image);
        }
    }
}
