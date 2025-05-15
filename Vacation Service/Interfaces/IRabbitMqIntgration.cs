using EmployeesPortal.Models;
using EmployeesPortal.Shared.Models;
using System.Collections.Concurrent;

namespace Vacation_Service.Interfaces
{
    public interface IRabbitMqIntgration
    {
        public void Publish(VacationRequest vacationRequest);
      
    }
}
