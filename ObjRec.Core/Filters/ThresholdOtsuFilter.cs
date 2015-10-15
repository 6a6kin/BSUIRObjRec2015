using System.Drawing;
using System.Threading.Tasks;
using ObjRec.Core.Algorithms;

namespace ObjRec.Core.Filters
{
    public class ThresholdOtsuFilter : ThresholdFilter
    {
        public async override Task<Image> Apply(Image image)
        {
            Threshold = await ThresholdOtsuAlgorithm.ComputeThreshold(image);

            return await base.Apply(image);
        }
    }
}
