using Internship.EmployeeManagement.Core.Interfaces;
using MassTransit;

namespace Internship.EmployeeManagement.Infrastructure.MessageBroker
{
    public class EventBus(IPublishEndpoint publishEndpoint) : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : class
        {
            await _publishEndpoint.Publish(message, cancellationToken);
        }
    }
}
