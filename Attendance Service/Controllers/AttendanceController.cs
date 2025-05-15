using Attendance_Service.Interfaces;
using EmployeesPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Attendance_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }
        [Authorize(Roles ="Employee")]
        [HttpPost("CheackIn")]
        public async Task<IActionResult> CreateCheakIn(int EmploeeId)
        {
            var attendance = await _attendanceService.CheckInAsync(EmploeeId);
            if (attendance == null)
                return BadRequest();
            return Ok(attendance);
        }
        [Authorize(Roles = "Employee")]
        [HttpPost("CheackOut")]
        public async Task<IActionResult> CreateCheakOut(int EmploeeId)
        {
            var attendance = await _attendanceService.CheckOutAsync(EmploeeId);
            if (attendance == null)
                return BadRequest();
            return Ok(attendance);
        }
        [Authorize(Roles = "Employee")]

        [HttpGet("GetAllAttendanceRecordsOfEmployeeId")]
        public async Task<IActionResult> GetAll(int EmployeeId)
        {
            var attendances= await _attendanceService.GetAttendanceRecordsAsync(EmployeeId);
            if (attendances != null && attendances.Count > 0)
            {
                return Ok(attendances);
            }
            return NotFound("No attendance records found for this employee.");
        }
    }
}
