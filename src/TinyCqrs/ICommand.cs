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

    /// <summary>
    ///     A marker interface for classes that subscribe to messages.
    /// </summary>
    public interface IHandle
    {
    }

    /// <summary>
    ///     Denotes a class which can handle a particular type of message.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to handle.</typeparam>
    public interface IHandle<TMessage> : IHandle
    {
        //don't use contravariance here
        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Handle(TMessage message);
    }
}