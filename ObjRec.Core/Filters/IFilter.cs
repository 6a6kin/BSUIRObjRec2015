using System.Drawing;
using System.Threading.Tasks;

namespace ObjRec.Core.Filters
{
    public interface IFilter
    {
        Task<Image> Apply(Image image);
    }
}
