using System.Linq;
using System.Threading.Tasks;

namespace TinyCqrs
{
    public interface IQueryAgent
    {
        Task<TResult> PerformQuery<TResult>(IQueryArgs<TResult> args);
    }
}