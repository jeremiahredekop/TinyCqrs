using System.Threading.Tasks;

namespace TinyCqrs
{
    public interface IQuery
    {
    }

    public interface IQuery<TArgs, TResult> : IQuery
    {
        Task<TResult> Perform(TArgs input);
    }
}