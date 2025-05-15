using EmployeesPortal.Models;

namespace MeetingService.IServices
{
    public interface IScheduleService
    {
        Task<IEnumerable<Schedule>> GetALLAppointmentsForEmployeeAsync(int employeeId);
        Task<IEnumerable<Schedule>> GetAppointmentsForEmployeeInRangeAsync(int employeeId, DateTime startDate, DateTime endDate);
        Task<bool> CheckAvailabilityAsync(int employeeId, DateTime startDate, DateTime endDate);
        Task<bool> AddAppointmentAsync(Schedule appointment);
        Task<bool> RemoveAppointmentMeetingAsync(int employeeId, int meetingId);
        Task<bool> RemoveAppointmentAsync(int scheduleId);
        Task<IEnumerable<Schedule>> GetAppointmentsForEmployeesInRangeAsync(List<int> employeeIds, DateTime startDate, DateTime endDate);
        Task RemoveAppintmentsForEmployeeVacation(VacationRequest vs);

    }
}
