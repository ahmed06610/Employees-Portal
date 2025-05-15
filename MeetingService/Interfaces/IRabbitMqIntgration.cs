using EmployeesPortal.Models;
using EmployeesPortal.Shared.Models;
using System.Collections.Concurrent;

namespace MeetingService.Interfaces
{
    public interface IRabbitMqIntgration
    {
        public void StartConsumerMeeting(IServiceProvider serviceProvider);
    }
}
