namespace MeetingService.DTOs
{
    public class ScheduleCreateDTO
    {
        public int EmployeeID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; }
        public int ScheduleStatusId { get; set; }

    }
}
