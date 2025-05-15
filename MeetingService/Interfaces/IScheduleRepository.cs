using EmployeesPortal.Models;

namespace MeetingService.Interfaces
{
    public interface IScheduleRepository
    {
        Task<IEnumerable<Schedule>> GetALLAppointmentsForEmployeeAsync(int employeeId);
        Task<IEnumerable<Schedule>> GetAppointmentsForEmployeesInInterval(List<int> employeeIds, DateTime start, DateTime end);
        Task<bool> CheckAvailabilityForEmployees(List<int> employeeIds, DateTime startDate, DateTime endDate);
        Task<bool> CheckAvailabilityForEmployee(int employeeId, DateTime startDate, DateTime endDate);
        Task AddAppointmentAsync(Schedule appointment);
        Task RemoveAppointmentForMeetingAsync(int meetingId, int employeeId);
        Task DeleteAppointmentsForMeetingAsync(int meetingId);
        Task DeleteAppointmentsForEmployee(int scheduleId);
        Task UpdateAppiontmentAsync(Schedule schedule);

    }

}
