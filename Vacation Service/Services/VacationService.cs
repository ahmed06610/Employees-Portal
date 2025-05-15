using EmployeesPortal.Models;
using Microsoft.EntityFrameworkCore;
using Vacation_Service.DTOs;
using Vacation_Service.Enums;
using Vacation_Service.Interfaces;
using Vacation_Service.Repositories;

namespace Vacation_Service.Services
{
    public class VacationService : IVacationService
    {
        private readonly IVacationRepository _repository;

        public VacationService(IVacationRepository repository)
        {
            _repository = repository;
        }

        public async Task<VacationRequest> CreateVacationRequestAsync(VacationRequestCreateDTO request)
        {
            var managerid = (int)await _repository.GetHeadManagerIdAsync(request.EmployeeID);
            var req = new VacationRequest
            {
                EmployeeID = request.EmployeeID,
                StartDateOfVacation = request.StartDateOfVacation,
                EndDateOfVacation = request.EndDateOfVacation,
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow,
                EscalationAt = DateTime.UtcNow,
                StatusID = (int)VacationStatus.Pending,
                Escalation = 0,
                ManagerIdOfRequest =managerid,
                IsDeleted=false
            };
            return await _repository.CreateVacationRequestAsync(req);
        }
        public async Task<VacationRequest> UpdateVacationRequestAsync(VacationRequest request)
        {

            return await _repository.UpdateVacationRequestAsync(request);
        }
        public async Task<VacationRequest> UpdateVacationRequestDTOAsync( VacationRequestUpdateDTO updatedRequest)
        {
            var request = await _repository.GetVacationRequestByIdAsync(updatedRequest.RequestID);
            if (request == null) return null;

            request.StartDateOfVacation = updatedRequest.StartDateOfVacation;
            request.EndDateOfVacation = updatedRequest.EndDateOfVacation;
            request.Reason = updatedRequest.Reason;
            request.ModifiedAt = DateTime.UtcNow; 
            return await _repository.UpdateVacationRequestAsync(request);
        }

        public async Task<bool> DeleteVacationRequestAsync(int requestId)
        {
            return await _repository.DeleteVacationRequestAsync(requestId);
        }

        public async Task<VacationRequest> GetVacationRequestByIdAsync(int requestId)
        {
            return await _repository.GetVacationRequestByIdAsync(requestId);
        }

        public async Task<List<VacationRequest>> GetVacationRequestsByEmployeeAsync(int employeeId)
        {
            return await _repository.GetVacationRequestsByEmployeeAsync(employeeId);
        }

        public async Task<bool> ApproveVacationRequestAsync(int requestId)
        {
            return await _repository.ApproveVacationRequestAsync(requestId);
        }

        public async Task<bool> RejectVacationRequestAsync(int requestId)
        {
            return await _repository.RejectVacationRequestAsync(requestId);
        }

        public async Task<List<VacationRequest>> GetVacationRequestsByManagerIdAsync(int employeeId)
        {
           var result=await _repository.GetVacationRequestsByManagerIdAsync(employeeId);
            if (result == null) return null;
           return result;
        }
        public async Task<List<VacationRequest>> GetRequestsNeedingEscalationAsync()
        {
            var twoDaysAgo = DateTime.UtcNow.AddDays(-2);

            return await _repository.GetVacationRequests(v => v.EscalationAt <= twoDaysAgo
                         && (v.StatusID == (int)VacationStatus.Pending||v.StatusID== (int)VacationStatus.Escalated));
        }
        public async Task<int?> GetHeadManagerIdAsync(int managerId)
        {
           return await _repository.GetHeadManagerIdAsync(managerId);
        }


    }

}
