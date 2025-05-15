using EmployeesPortal.Models;
using MeetingService.DTOs;
using MeetingService.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Employee")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }
        [HttpGet("employee/{employeeId}/All")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GeALLtAppointmentsForEmployee(int employeeId)
        {
            var appointments = await _scheduleService.GetALLAppointmentsForEmployeeAsync(employeeId);
            return Ok(appointments);
        }

        // Get all appointments for an employee in a given date range
        [HttpGet("employee/{employeeId}/range")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetAppointmentsForEmployeeInRange(int employeeId, DateTime startDate, DateTime endDate)
        {
            var appointments = await _scheduleService.GetAppointmentsForEmployeeInRangeAsync(employeeId, startDate, endDate);
            return Ok(appointments);
        }

        // Check availability for an employee
        [HttpGet("employee/{employeeId}/availability")]
        public async Task<ActionResult<bool>> CheckAvailabilityForEmployee(int employeeId, DateTime startDate, DateTime endDate)
        {
            var isAvailable = await _scheduleService.CheckAvailabilityAsync(employeeId, startDate, endDate);
            return Ok(isAvailable);
        }

        // Add an appointment for an employee
        [HttpPost("add")]
        public async Task<IActionResult> AddAppointment(ScheduleCreateDTO dto)
        {
            var appointment = new Schedule
            {
                AppointmentDate = dto.AppointmentDate,
                Description = dto.Description,
                EmployeeID = dto.EmployeeID,
                EndTime=dto.EndTime,
                StartTime=dto.StartTime,
                ScheduleStatusId=dto.ScheduleStatusId,
            };
            await _scheduleService.AddAppointmentAsync(appointment);
            return CreatedAtAction(nameof(GetAppointmentsForEmployeeInRange), new { employeeId = appointment.EmployeeID }, appointment);
        }

        // Remove an appointment
        [HttpDelete("meeting/{meetingId}/employee/{employeeId}")]
        public async Task<IActionResult> RemoveAppointment(int meetingId, int employeeId)
        {
            await _scheduleService.RemoveAppointmentMeetingAsync(employeeId, meetingId);
            return NoContent();
        }

        // Get appointments for multiple employees in a range
        [HttpGet("employees/range")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetAppointmentsForEmployeesInRange([FromQuery] List<int> employeeIds, DateTime startDate, DateTime endDate)
        {
            var appointments = await _scheduleService.GetAppointmentsForEmployeesInRangeAsync(employeeIds, startDate, endDate);
            return Ok(appointments);
        }
    }
}
