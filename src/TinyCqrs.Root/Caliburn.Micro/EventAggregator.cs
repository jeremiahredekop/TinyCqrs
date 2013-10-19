using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

#if NETFX_CORE && !WinRT
#define WinRT
#endif

namespace TinyCqrs.Root.Caliburn.Micro
{
    /// <summary>
    ///     Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        ///     Gets or sets the default publication thread marshaller.
        /// </summary>
        /// <value>
        ///     The default publication thread marshaller.
        /// </value>
        Action<Action> PublicationThreadMarshaller { get; set; }

        /// <summary>
        ///     Searches the subscribed handlers to check if we have a handler for
        ///     the message type supplied.
        /// </summary>
        /// <param name="messageType">The message type to check with</param>
        /// <returns>True if any handler is found, false if not.</returns>
        bool HandlerExistsFor(Type messageType);

        /// <summary>
        ///     Subscribes an instance to all events declared through implementations of <see cref="IHandle{TMessage}" />
        /// </summary>
        /// <param name="subscriber">The instance to subscribe for event publication.</param>
        void Subscribe(object subscriber);

        /// <summary>
        ///     Unsubscribes the instance from all events.
        /// </summary>
        /// <param name="subscriber">The instance to unsubscribe.</param>
        void Unsubscribe(object subscriber);

        /// <summary>
        ///     Publishes a message.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <remarks>
        ///     Uses the default thread marshaller during publication.
        /// </remarks>
        void Publish(object message);

        /// <summary>
        ///     Publishes a message.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <param name="marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        void Publish(object message, Action<Action> marshal);
    }

    /// <summary>
    ///     Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        /// <summary>
        ///     The default thread marshaller used for publication;
        /// </summary>
        public static Action<Action> DefaultPublicationThreadMarshaller = action => action();

        /// <summary>
        ///     Processing of handler results on publication thread.
        /// </summary>
        public static Action<object, object> HandlerResultProcessing = (target, result) => { };

        private readonly List<Handler> handlers = new List<Handler>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventAggregator" /> class.
        /// </summary>
        public EventAggregator()
        {
            PublicationThreadMarshaller = DefaultPublicationThreadMarshaller;
        }

        /// <summary>
        ///     Gets or sets the default publication thread marshaller.
        /// </summary>
        /// <value>
        ///     The default publication thread marshaller.
        /// </value>
        public Action<Action> PublicationThreadMarshaller { get; set; }

        /// <summary>
        ///     Searches the subscribed handlers to check if we have a handler for
        ///     the message type supplied.
        /// </summary>
        /// <param name="messageType">The message type to check with</param>
        /// <returns>True if any handler is found, false if not.</returns>
        public bool HandlerExistsFor(Type messageType)
        {
            return handlers.Any(handler => handler.Handles(messageType) & !handler.IsDead);
        }

        /// <summary>
        ///     Subscribes an instance to all events declared through implementations of <see cref="IHandle{T}" />
        /// </summary>
        /// <param name="subscriber">The instance to subscribe for event publication.</param>
        public virtual void Subscribe(object subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }
            lock (handlers)
            {
                if (handlers.Any(x => x.Matches(subscriber)))
                {
                    return;
                }

                handlers.Add(new Handler(subscriber));
            }
        }

        /// <summary>
        ///     Unsubscribes the instance from all events.
        /// </summary>
        /// <param name="subscriber">The instance to unsubscribe.</param>
        public virtual void Unsubscribe(object subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }
            lock (handlers)
            {
                Handler found = handlers.FirstOrDefault(x => x.Matches(subscriber));

                if (found != null)
                {
                    handlers.Remove(found);
                }
            }
        }

        /// <summary>
        ///     Publishes a message.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <remarks>
        ///     Does not marshall the the publication to any special thread by default.
        /// </remarks>
        public virtual void Publish(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            Publish(message, PublicationThreadMarshaller);
        }

        /// <summary>
        ///     Publishes a message.
        /// </summary>
        /// <param name="message">The message instance.</param>
        /// <param name="marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        public virtual void Publish(object message, Action<Action> marshal)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (marshal == null)
            {
                throw new ArgumentNullException("marshal");
            }

            Handler[] toNotify;
            lock (handlers)
            {
                toNotify = handlers.ToArray();
            }

            marshal(() =>
                {
                    Type messageType = message.GetType();

                    List<Handler> dead = toNotify
                        .Where(handler => !handler.Handle(messageType, message))
                        .ToList();

                    if (dead.Any())
                    {
                        lock (handlers)
                        {
                            dead.Apply(x => handlers.Remove(x));
                        }
                    }
                });
        }

        private class Handler
        {
            private readonly WeakReference reference;
            private readonly Dictionary<Type, MethodInfo> supportedHandlers = new Dictionary<Type, MethodInfo>();

            public Handler(object handler)
            {
                reference = new WeakReference(handler);

#if WinRT
                var handlerInfo = typeof(IHandle).GetTypeInfo();
                var interfaces = handler.GetType().GetTypeInfo().ImplementedInterfaces
                    .Where(x => handlerInfo.IsAssignableFrom(x.GetTypeInfo()) && x.GetTypeInfo().IsGenericType);

                foreach (var @interface in interfaces) {
                    var type = @interface.GenericTypeArguments[0];
                    var method = @interface.GetTypeInfo().DeclaredMethods.First(x => x.Name == "Handle");
                    supportedHandlers[type] = method;
                }
#else
                IEnumerable<Type> interfaces = handler.GetType().GetInterfaces()
                                                      .Where(
                                                          x => typeof (IHandle).IsAssignableFrom(x) && x.IsGenericType);

                foreach (Type @interface in interfaces)
                {
                    Type type = @interface.GetGenericArguments()[0];
                    MethodInfo method = @interface.GetMethod("Handle");
                    supportedHandlers[type] = method;
                }
#endif
            }

            public bool IsDead
            {
                get { return reference.Target == null; }
            }

            public bool Matches(object instance)
            {
                return reference.Target == instance;
            }

            public bool Handle(Type messageType, object message)
            {
                object target = reference.Target;
                if (target == null)
                {
                    return false;
                }

                foreach (var pair in supportedHandlers)
                {
                    if (pair.Key.IsAssignableFrom(messageType))
                    {
                        object result = pair.Value.Invoke(target, new[] {message});
                        if (result != null)
                        {
                            HandlerResultProcessing(target, result);
                        }
                    }
                }

                return true;
            }

            public bool Handles(Type messageType)
            {
                return supportedHandlers.Any(pair => pair.Key.IsAssignableFrom(messageType));
            }
        }
    }
}