using Internship.EmployeeManagement.Application.Features.Employees.Commands.UpdateEmployee;
using Internship.EmployeeManagement.Core.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Internship.EmployeeManagement.Api.IntegrationTests.ExternalServices
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Type>> _consumerTypes;

        private readonly IServiceProvider _serviceProvider;
        public List<Type> Consumed { get; }
        public bool UpdateEventOutdated { get; set; }
        public EventBus(IServiceProvider serviceProvider)
        {
            _consumerTypes = new();
            _serviceProvider = serviceProvider;
            Consumed = new();
        }

        public void Subscribe<TMessage, TConsumer>()
            where TMessage : class
            where TConsumer : IConsumer<TMessage>
        {
            var messageType = typeof(TMessage);
            var consumerType = typeof(TConsumer);

            if (!_consumerTypes.ContainsKey(messageType))
                _consumerTypes[messageType] = new List<Type>();

            if (!_consumerTypes[messageType].Contains(consumerType))
                _consumerTypes[messageType].Add(consumerType);
        }

        public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : class
        {
            var messageType = message.GetType();

            if(UpdateEventOutdated && messageType == typeof(EmployeeUpdatedEvent))
                OutdateUpdateEvent(message as EmployeeUpdatedEvent);

            var consumeContext = CreateConsumeContext(message);

            if (_consumerTypes.ContainsKey(messageType))
            {
                using var scope = _serviceProvider.CreateScope();

                foreach (var consumerType in _consumerTypes[messageType])
                {

                    var consumer = scope.ServiceProvider.GetService(consumerType);

                    if (consumer != null)
                    {

                        var method = consumerType.GetMethod("Consume");

                        await (Task?)method?.Invoke(consumer, [consumeContext]);

                        Consumed.Add(messageType);
                    }
                }
            }

        }

        private static void OutdateUpdateEvent(EmployeeUpdatedEvent employeeUpdatedEvent)
        {
            employeeUpdatedEvent.UpdateDateTime = DateTime.UtcNow.AddMinutes(-1);
        }

        private object CreateConsumeContext<T>(T message) where T : class
        {
            var mockContext = new Mock<ConsumeContext<T>>();
            mockContext.Setup(x => x.Message).Returns(message);
            mockContext.Setup(x => x.CancellationToken).Returns((CancellationToken)default);

            return mockContext.Object;
        }

    }
}
