namespace TinyCqrs
{
    public interface IPublisher
    {
        void Publish(object message);
    }
}