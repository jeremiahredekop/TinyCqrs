using System.Linq;
using System.Threading.Tasks;

namespace TinyCqrs
{
    public interface IQueryAgent
    {
        Task<TResult> PerformQuery<TResult>(IQueryArgs<TResult> args);
    }

    public interface ICommandAgent
    {
        Task PerformCommand<TArgs>(TArgs args) where TArgs : class;
    }

    public interface IPublisher
    {
        void Publish(object message);
    }

    public interface ISubscriptions
    {
        void Subscribe(object subscriber);
        void Unsubscribe(object subscriber);
    }
}