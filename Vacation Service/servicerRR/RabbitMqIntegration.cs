using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using EmployeesPortal.Models;
using Vacation_Service.Interfaces;


namespace Vacation_Service.servicerRR
{
    public class RabbitMqIntegration : IRabbitMqIntgration
    {
        public void Publish(VacationRequest vacationRequest)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare the exchange
            channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

            // Declare the queues and bind them to the exchange
            channel.QueueDeclare(queue: "MeetingvacationRequest",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: new Dictionary<string, object> { { "x-queue-type", "quorum" } });
            channel.QueueBind(queue: "MeetingvacationRequest",
                              exchange: "logs",
                              routingKey: string.Empty);  // No routing key needed for fanout

            channel.QueueDeclare(queue: "SchedulevacationRequest",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: new Dictionary<string, object> { { "x-queue-type", "quorum" } });
            channel.QueueBind(queue: "SchedulevacationRequest",
                              exchange: "logs",
                              routingKey: string.Empty);  // No routing key needed for fanout

            // Serialize the VacationRequest object to a JSON string
            string message = JsonSerializer.Serialize(vacationRequest);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true; // Ensure the message is marked as persistent

            // Publish the message to the fanout exchange (no routing key for fanout)
            channel.BasicPublish(exchange: "logs",
                                 routingKey: string.Empty, // No routing key for fanout
                                 basicProperties: properties,
                                 body: body);

            Console.WriteLine("Sent VacationRequest: {0}", message);
        }


    }
}
