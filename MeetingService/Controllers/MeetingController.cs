using MeetingService.DTOs;
using MeetingService.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeetingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Employee")]
    public class MeetingController : ControllerBase
    {
        private readonly IMeetingService _meetingService;

        public MeetingController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }

        [HttpGet("recommendation")]
        public async Task<IActionResult> GetRecommendation([FromQuery] List<int> employeeIds, DateTime intervalStart, DateTime intervalEnd, TimeSpan duration)
        {
            // Get available time slots and await the task
            var availableSlots = await _meetingService.GetAvailableTimeSlots(employeeIds, intervalStart, intervalEnd, duration);

            // Create the RecommendationDTO
            var recommendation = new RecommendationDTO
            {
                AvailableSlots = new List<AvailableSlotDTO>()
            };

            // Group available slots by date
            var groupedSlots = availableSlots
                .GroupBy(slot => slot.Start.Date) // Group by the date part of Start
                .Select(group => new AvailableSlotDTO
                {
                    Date = group.Key, // The grouped date
                    TimeSlots = group.Select(slot => new TimeSlotDTO
                    {
                        StartTime = slot.Start.TimeOfDay, // Get the TimeSpan for Start
                        EndTime = slot.End.TimeOfDay // Get the TimeSpan for End
                    }).ToList()
                }).ToList();

            recommendation.AvailableSlots.AddRange(groupedSlots);

            return Ok(recommendation);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMeeting([FromBody] MeetingCreateDTO dto)
        {
            var result = await _meetingService.CreateMeetingAsync(dto);
            if (result)
                return Ok("Meeting created successfully.");
            else
                return BadRequest("Could not create meeting. Conflicting schedules.");
        }

        [HttpPost("{meetingId}/add-employee/{employeeId}")]
        public async Task<IActionResult> AddEmployeeToMeeting(int meetingId, int employeeId)
        {
            var result = await _meetingService.AddEmployeeToMeetingAsync(meetingId, employeeId);
            if (result)
                return Ok("Employee added to meeting.");
            else
                return BadRequest("Employee is not available.");
        }

        [HttpDelete("{meetingId}/remove-employee/{employeeId}")]
        public async Task<IActionResult> RemoveEmployeeFromMeeting(int meetingId, int employeeId)
        {
            await _meetingService.RemoveEmployeeFromMeetingAsync(meetingId, employeeId);
            return Ok("Employee removed from meeting.");
        }

        [HttpDelete("{meetingId}/cancel")]
        public async Task<IActionResult> CancelMeeting(int meetingId)
        {
            await _meetingService.CancelMeetingAsync(meetingId);
            return Ok("Meeting canceled.");
        }

        [HttpGet("GetAllMeetings")]
        public async Task<IActionResult> GetAllMeetings()
        {
            var meetings = await _meetingService.GetAllMeetingsAsync();
            return Ok(meetings);
        }

        [HttpGet("{meetingId}")]
        public async Task<IActionResult> GetMeetingById(int meetingId)
        {
            var meeting = await _meetingService.GetMeetingByIdAsync(meetingId);
            if (meeting != null)
                return Ok(meeting);
            else
                return NotFound("Meeting not found.");
        }
    }

}
