using EmployeesPortal.Models;

namespace Attendance_Service.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceRecord> CheckInAsync(int employeeId);
        Task<AttendanceRecord> CheckOutAsync(int employeeId);
        Task<List<AttendanceRecord>> GetAttendanceRecordsAsync(int employeeId);
    }

}
