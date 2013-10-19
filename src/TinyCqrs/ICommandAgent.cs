using System.Threading.Tasks;

namespace TinyCqrs
{
    public interface ICommandAgent
    {
        Task PerformCommand<TArgs>(TArgs args) where TArgs : class;
    }
}