using EventTypes;
using MassTransit;
using Microsoft.Extensions.Logging;
using RequestLogAPI.Data;
using System.Threading.Tasks;

namespace RequestLogAPI
{
    public class MessageConsumer :
        IConsumer<Message>
    {
        private readonly EFContext rep;
        readonly ILogger<MessageConsumer> _logger;

        public MessageConsumer(EFContext rep, ILogger<MessageConsumer> logger)
        {
            this.rep = rep;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Message> context)
        {
            string message = $"Received request: for city - {context.Message.City} at {context.Message.Time}";
            _logger.LogInformation(message);

            rep.RequestLogs.Add(new RequestLog
            {
                Text = message,
                Time = context.Message.Time
            });

            rep.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
