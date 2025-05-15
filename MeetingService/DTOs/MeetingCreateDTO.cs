namespace MeetingService.DTOs
{
    public class MeetingCreateDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? VirtualMeeting { get; set; }
        public List<int> EmployeeIds { get; set; } // List of Employee IDs participating in the meeting
        public string Description { get; set; }
    }
}
