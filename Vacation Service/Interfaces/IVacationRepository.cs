using EmployeesPortal.Models;
using System.Linq.Expressions;
using Vacation_Service.DTOs;

namespace Vacation_Service.Interfaces
{
    public interface IVacationRepository
    {
        Task<VacationRequest> CreateVacationRequestAsync(VacationRequest request);
        Task<VacationRequest> UpdateVacationRequestAsync(VacationRequest request);
        Task<bool> DeleteVacationRequestAsync(int requestId);
        Task<VacationRequest> GetVacationRequestByIdAsync(int requestId);
        Task<List<VacationRequest>> GetVacationRequestsByEmployeeAsync(int employeeId);
        Task<bool> ApproveVacationRequestAsync(int requestId);
        Task<bool> RejectVacationRequestAsync(int requestId);
        Task<List<VacationRequest>> GetVacationRequestsByManagerIdAsync(int employeeId);
        public  Task<List<VacationRequest>> GetVacationRequests(params Expression<Func<VacationRequest, bool>>[] expressions);
        public  Task<int?> GetHeadManagerIdAsync(int managerId);

    }
}
