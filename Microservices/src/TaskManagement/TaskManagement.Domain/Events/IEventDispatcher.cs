namespace TaskManagement.Domain.Events
{
    public interface IEventDispatcher
    {
        void Publish(IEvent e);
    }
}
