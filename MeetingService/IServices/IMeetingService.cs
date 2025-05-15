using EmployeesPortal.Models;
using MeetingService.DTOs;

namespace MeetingService.IServices
{
    public interface IMeetingService
    {
        public Task<List<(DateTime Start, DateTime End)>> GetAvailableTimeSlots(List<int> employeeIds, DateTime intervalStart, DateTime intervalEnd, TimeSpan duration);
        public Task<bool> CreateMeetingAsync(MeetingCreateDTO dto);
        public  Task<bool> AddEmployeeToMeetingAsync(int meetingId, int employeeId);
        public Task RemoveEmployeeFromMeetingAsync(int meetingId, int employeeId);
        public  Task CancelMeetingAsync(int meetingId);
        public Task<List<MeetingDTO>> GetAllMeetingsAsync();
        public Task<Meeting> GetMeetingByIdAsync(int meetingId);
        public Task RemoveEmpsGetVacation(VacationRequest vs);





    }
}
