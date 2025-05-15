using EmployeesPortal.Data;
using EmployeesPortal.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Vacation_Service.DTOs;
using Vacation_Service.Enums;
using Vacation_Service.Interfaces;

namespace Vacation_Service.Repositories
{
    public class VacationRepository : IVacationRepository
    {
        private readonly ApplicationDbContext _context;

        public VacationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VacationRequest> CreateVacationRequestAsync(VacationRequest request)
        {
         
            _context.VacationRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<VacationRequest> UpdateVacationRequestAsync(VacationRequest request)
        {
           
            _context.VacationRequests.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> DeleteVacationRequestAsync(int requestId)
        {
            var request = await _context.VacationRequests.FindAsync(requestId);
            if (request == null) return false;
            request.IsDeleted= true;
            _context.VacationRequests.Update(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<VacationRequest> GetVacationRequestByIdAsync(int requestId)
        {
            return await _context.VacationRequests.FindAsync(requestId);
        }

        public async Task<List<VacationRequest>> GetVacationRequestsByEmployeeAsync(int employeeId)
        {
            return await _context.VacationRequests
                .Where(v => v.EmployeeID == employeeId)
                .ToListAsync();
        }

        public async Task<bool> ApproveVacationRequestAsync(int requestId)
        {
            var request = await _context.VacationRequests.FindAsync(requestId);
            if (request == null) return false;

            request.StatusID = (int)VacationStatus.Approved; 
            request.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectVacationRequestAsync(int requestId)
        {
            var request = await _context.VacationRequests.FindAsync(requestId);
            if (request == null) return false;

            request.StatusID = (int)VacationStatus.Rejected;
            request.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<VacationRequest>> GetVacationRequestsByManagerIdAsync(int employeeId)
        {
            return await _context.VacationRequests
                .Where(vr => vr.ManagerIdOfRequest == employeeId)
                .ToListAsync();
        }
        public async Task<List<VacationRequest>> GetVacationRequests(params Expression<Func<VacationRequest, bool>>[] expressions)
        {
            IQueryable<VacationRequest> query = _context.Set<VacationRequest>();

            foreach (var expression in expressions)
            {
                query = query.Where(expression);
            }

            return await query.ToListAsync();
        }
        public async Task<int?> GetHeadManagerIdAsync(int employeeId)
        {
            var manager = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

            return manager?.ManagerID; // Assuming Employee table has ManagerID for the head manager
        }
    }

}
