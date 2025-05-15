using EmployeesPortal.Models;
using Vacation_Service.DTOs;

namespace Vacation_Service.Interfaces
{
    public interface IVacationService
    {
        Task<VacationRequest> CreateVacationRequestAsync(VacationRequestCreateDTO request);
        Task<VacationRequest> UpdateVacationRequestAsync( VacationRequest request);
        Task<VacationRequest> UpdateVacationRequestDTOAsync( VacationRequestUpdateDTO request);
        Task<bool> DeleteVacationRequestAsync(int requestId);
        Task<VacationRequest> GetVacationRequestByIdAsync(int requestId);
        Task<List<VacationRequest>> GetVacationRequestsByEmployeeAsync(int employeeId);
        Task<bool> ApproveVacationRequestAsync(int requestId);
        Task<bool> RejectVacationRequestAsync(int requestId);
        Task<List<VacationRequest>> GetVacationRequestsByManagerIdAsync(int employeeId);
        public  Task<List<VacationRequest>> GetRequestsNeedingEscalationAsync();
        public Task<int?> GetHeadManagerIdAsync(int managerId);



    }
}
