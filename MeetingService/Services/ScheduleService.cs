using EmployeesPortal.Models;
using MeetingService.Interfaces;
using MeetingService.IServices;
using MeetingService.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingService.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }
       public async Task<IEnumerable<Schedule>> GetALLAppointmentsForEmployeeAsync(int employeeId)
        {
            return await _scheduleRepository.GetALLAppointmentsForEmployeeAsync(employeeId );

        }


        // Get all appointments for an employee in a specific date range
        public async Task<IEnumerable<Schedule>> GetAppointmentsForEmployeeInRangeAsync(int employeeId, DateTime startDate, DateTime endDate)
        {
            return await _scheduleRepository.GetAppointmentsForEmployeesInInterval(new List<int> { employeeId }, startDate, endDate);
        }

        // Check availability for a specific employee
        public async Task<bool> CheckAvailabilityAsync(int employeeId, DateTime startDate, DateTime endDate)
        {
            return await _scheduleRepository.CheckAvailabilityForEmployee(employeeId, startDate, endDate);
        }

        // Add a new appointment for an employee
        public async Task<bool> AddAppointmentAsync(Schedule appointment)
        {
            await _scheduleRepository.AddAppointmentAsync(appointment);
            return true;
        }

        // Remove an appointment for an employee
        public async Task<bool> RemoveAppointmentMeetingAsync(int employeeId, int meetingId)
        {
            await _scheduleRepository.RemoveAppointmentForMeetingAsync(meetingId, employeeId);
            return true;
        }
        public async Task<bool> RemoveAppointmentAsync(int schudelId)
        {
            await _scheduleRepository.DeleteAppointmentsForEmployee(schudelId);
            return true;
        }

        // Get appointments for multiple employees within a specified date range
        public async Task<IEnumerable<Schedule>> GetAppointmentsForEmployeesInRangeAsync(List<int> employeeIds, DateTime startDate, DateTime endDate)
        {
            return await _scheduleRepository.GetAppointmentsForEmployeesInInterval(employeeIds, startDate, endDate);
        }
        public async Task RemoveAppintmentsForEmployeeVacation(VacationRequest vs)
        {
            var apps = await _scheduleRepository.GetALLAppointmentsForEmployeeAsync(vs.EmployeeID);
            foreach (var app in apps)
            {
                if ((vs.StartDateOfVacation <= app.AppointmentDate.Add(app.StartTime) && vs.EndDateOfVacation >= app.AppointmentDate.Add(app.StartTime))
                    || (vs.StartDateOfVacation <= app.AppointmentDate.Add(app.EndTime) && vs.EndDateOfVacation >= app.AppointmentDate.Add(app.EndTime)))
                {
                    app.IsOff = true;
                    await _scheduleRepository.UpdateAppiontmentAsync(app);
                }
            }
        }
    }
}
