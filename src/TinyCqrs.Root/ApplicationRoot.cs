using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using TinyCqrs.Root.Caliburn.Micro;

namespace TinyCqrs.Root
{
    public class ApplicationRoot : IQueryAgent, ICommandAgent, ISubscriptions, IPublisher
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IServiceLocator _locator;

        public ApplicationRoot(IServiceLocator locator, IEventAggregator eventAggregator)
        {
            _locator = locator;
            _eventAggregator = eventAggregator;
        }


        public Task PerformCommand<TArgs>(TArgs args) where TArgs : class
        {
            var command = _locator.GetInstance<ICommand<TArgs>>();
            return command.Invoke(args);
        }

        public void Publish(object message)
        {
            _eventAggregator.Publish(message);
        }

        public Task<TResult> PerformQuery<TResult>(IQueryArgs<TResult> args)
        {
            return (Task<TResult>) GetType()
                                       .GetMethod("PerformQueryInner", BindingFlags.Instance | BindingFlags.NonPublic)
                                       .MakeGenericMethod(args.GetType(), typeof (TResult))
                                       .Invoke(this, new object[] {args});
        }

        public void Subscribe(object subscriber)
        {
            _eventAggregator.Subscribe(subscriber);
        }

        public void Unsubscribe(object subscriber)
        {
            _eventAggregator.Unsubscribe(subscriber);
        }

        private Task<TResult> PerformQueryInner<TArgs, TResult>(TArgs args)
        {
            var query = _locator.GetInstance<IQuery<TArgs, TResult>>();
            return query.Perform(args);
        }
    }
}