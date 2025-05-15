using EmployeesPortal.Models;
using Vacation_Service.DTOs;
using Vacation_Service.Enums;
using Vacation_Service.Interfaces;

namespace Vacation_Service.Services
{
    public class VacationEscalationJob
    {
        private readonly IVacationService _vacationService;

        public VacationEscalationJob(IVacationService vacationService)
        {
            _vacationService = vacationService;
        }

        public async Task ExecuteAsync()
        {
            var requestsToEscalate = await _vacationService.GetRequestsNeedingEscalationAsync();

            foreach (var request in requestsToEscalate)
            {
                var managerId = request.ManagerIdOfRequest;
                var headManagerId = await _vacationService.GetHeadManagerIdAsync(managerId);

                if (headManagerId != null)
                {
                    request.ManagerIdOfRequest = headManagerId.Value;
                    request.Escalation += 1;
                    request.EscalationAt= DateTime.UtcNow;
                    request.StatusID =(int)VacationStatus.Escalated;
                    await _vacationService.UpdateVacationRequestAsync(request);

                    await NotifyManagerAsync(request, headManagerId.Value);
                }
            }
        }

        private async Task NotifyManagerAsync(VacationRequest request, int headManagerId)
        {
            // Notify the manager through SignalR or other mechanisms
        }
    }

}
