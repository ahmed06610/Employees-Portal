namespace MeetingService.DTOs
{
    public class AvailableSlotDTO
    {
        public DateTime Date { get; set; } // The date for which the slots are available
        public List<TimeSlotDTO> TimeSlots { get; set; } // List of available time slots for the date
    }

    public class TimeSlotDTO
    {
        public TimeSpan StartTime { get; set; } // Start time of the available slot
        public TimeSpan EndTime { get; set; } // End time of the available slot
    }

    public class RecommendationDTO
    {
        public List<AvailableSlotDTO> AvailableSlots { get; set; } // List of available slots grouped by date
    }

}
