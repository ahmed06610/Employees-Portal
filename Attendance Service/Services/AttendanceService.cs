using Attendance_Service.Interfaces;
using EmployeesPortal.Data;
using EmployeesPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Service.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceRecord> CheckInAsync(int employeeId)
        {
            var attendanceRecord = new AttendanceRecord
            {
                EmployeeID = employeeId,
                CheckInTime = DateTime.UtcNow,
            };
            _context.AttendanceRecords.Add(attendanceRecord);
            await _context.SaveChangesAsync();
            return attendanceRecord;
        }

        public async Task<AttendanceRecord> CheckOutAsync(int employeeId)
        {
            var attendanceRecord = await _context.AttendanceRecords
                .Where(a => a.EmployeeID == employeeId && a.CheckOutTime == null)
                .OrderByDescending(a => a.CheckInTime)
                .FirstOrDefaultAsync();

            if (attendanceRecord != null)
            {
                attendanceRecord.CheckOutTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return attendanceRecord;

        }

        public async Task<List<AttendanceRecord>> GetAttendanceRecordsAsync(int employeeId)
        {
            var res= await _context.AttendanceRecords
                .Where(a => a.EmployeeID == employeeId)
                .ToListAsync();
            return res;
        }
    }

}
