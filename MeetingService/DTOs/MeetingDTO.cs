namespace MeetingService.DTOs
{
    public class MeetingDTO
    {
        public int MeetingId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? VirtualMeeting { get; set; }
        public List<EmployeeDTO> Employee { get; set; } = new List<EmployeeDTO>(); // List of employee IDs participating in the meeting
    }
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOff {  get; set; }
    }
}
