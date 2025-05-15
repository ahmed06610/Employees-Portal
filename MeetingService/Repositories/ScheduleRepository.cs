using EmployeesPortal.Data;
using EmployeesPortal.Models;
using MeetingService.Enums;
using MeetingService.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace MeetingService.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public ScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }
       public async Task<IEnumerable<Schedule>> GetALLAppointmentsForEmployeeAsync(int employeeId)
        {
            var res = await _context.Schedules
               .Where(s => s.EmployeeID==employeeId)
               .ToListAsync();
            return res;
        }

        public async Task<IEnumerable<Schedule>> GetAppointmentsForEmployeesInInterval(List<int> employeeIds, DateTime start, DateTime end)
        {
            var res= await _context.Schedules
                .Where(s => employeeIds.Contains(s.EmployeeID) && s.AppointmentDate >= start && s.AppointmentDate <= end)
                .OrderBy(s => s.AppointmentDate)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
            return res;
        }

        public async Task<bool> CheckAvailabilityForEmployees(List<int> employeeIds, DateTime startDate, DateTime endDate)
        {
            foreach (var employeeId in employeeIds)
            {
                var isAvailable = await CheckAvailabilityForEmployee(employeeId, startDate, endDate);
                if (!isAvailable)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> CheckAvailabilityForEmployee(int employeeId, DateTime startDate, DateTime endDate)
        {
            var appointments = await _context.Schedules
     .Where(s => s.EmployeeID == employeeId)
     .ToListAsync();

            var conflictingAppointments = appointments
                .Where(s => ((s.AppointmentDate.Date.Add(s.StartTime)>startDate)
                          &&(s.AppointmentDate.Date.Add(s.StartTime)<endDate))||
                          (s.AppointmentDate.Date.Add(s.EndTime)>startDate)
                          &&(s.AppointmentDate.Date.Add(s.EndTime)<endDate))
                .ToList();



            return !conflictingAppointments.Any();
        }

        public async Task AddAppointmentAsync(Schedule appointment)
        {
            _context.Schedules.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAppointmentForMeetingAsync(int meetingId, int employeeId)
        {
            var appointment = await _context.Schedules.Include(s=>s.Status)
                .FirstOrDefaultAsync(s => s.EmployeeID == employeeId && s.Status.StatusID ==(int)ScheduleStatues.Meeting && s.MeetingID == meetingId);

            if (appointment != null)
            {
                _context.Schedules.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAppointmentsForMeetingAsync(int meetingId)
        {
            var appointments = await _context.Schedules
                .Where(s => s.MeetingID == meetingId)
                .ToListAsync();

            _context.Schedules.RemoveRange(appointments);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAppointmentsForEmployee(int scheduleId)
        {
            var appointments = await _context.Schedules
               .Where(s => s.ScheduleID == scheduleId)
               .ToListAsync();

            _context.Schedules.RemoveRange(appointments);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAppiontmentAsync(Schedule schedule)
        {
            if (schedule != null)
            {
                _context.Schedules.Update(schedule);
                await _context.SaveChangesAsync();
            }
        }
    }

}
