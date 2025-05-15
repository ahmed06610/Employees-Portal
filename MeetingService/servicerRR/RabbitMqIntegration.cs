using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using EmployeesPortal.Models;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using MeetingService.Interfaces;
using MeetingService.IServices;
using System;

namespace MeetingService.servicerRR
{
/*    public class RabbitMqIntegration : IRabbitMqIntgration
    {
        IServiceProvider serviceProvider;

        public void StartConsumerMeeting(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

            channel.QueueDeclare(queue: "MeetingvacationRequest",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: new Dictionary<string, object> { { "x-queue-type", "quorum" } });

            channel.QueueBind(queue: "MeetingvacationRequest",
                              exchange: "logs",
                              routingKey: string.Empty);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += processMeetingService;
            consumer.Received += processSchudleService;

            // Start consuming messages
            channel.BasicConsume(queue: "MeetingvacationRequest",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Waiting for messages...");
        }

        private void processSchudleService(object? model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Deserialize the message back to a VacationRequest object
            var vacationRequest = JsonSerializer.Deserialize<VacationRequest>(message);

            // Use a new scope to get the required service for processing
            using (var scope = serviceProvider.CreateScope())
            {


                var ScheduleService = scope.ServiceProvider.GetRequiredService<IScheduleService>();

                // Handle the vacation request: find affected meetings and mark participants as "isOff"
                ScheduleService.RemoveAppintmentsForEmployeeVacation(vacationRequest).Wait();
            }

            Console.WriteLine($"Processed by meetingService vacationRequest ID: {vacationRequest.RequestID}");
        }
        private void processMeetingService(object? model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Deserialize the message back to a VacationRequest object
            var vacationRequest = JsonSerializer.Deserialize<VacationRequest>(message);

            // Use a new scope to get the required service for processing
            using (var scope = serviceProvider.CreateScope())
            {


                var meetingService = scope.ServiceProvider.GetRequiredService<IMeetingService>();

                // Handle the vacation request: find affected meetings and mark participants as "isOff"
                meetingService.RemoveEmpsGetVacation(vacationRequest).Wait();
            }

            Console.WriteLine($"Processed by meetingService vacationRequest ID: {vacationRequest.RequestID}");
        }



    }
*/}
