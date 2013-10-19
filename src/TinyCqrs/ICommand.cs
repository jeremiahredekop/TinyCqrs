using System.Threading.Tasks;

namespace TinyCqrs
{
    public interface ICommand
    {
    }

    public interface ICommand<in TArgs> : ICommand
    {
        Task Invoke(TArgs command);
    }
}