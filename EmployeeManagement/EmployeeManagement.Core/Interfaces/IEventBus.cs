namespace Internship.EmployeeManagement.Core.Interfaces
{
    public interface IEventBus
    {
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : class;
    }
}
