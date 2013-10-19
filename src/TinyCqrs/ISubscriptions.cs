namespace TinyCqrs
{
    public interface ISubscriptions
    {
        void Subscribe(object subscriber);
        void Unsubscribe(object subscriber);
    }
}